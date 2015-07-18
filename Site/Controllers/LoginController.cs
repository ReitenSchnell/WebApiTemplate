using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Common;
using Common.Contracts;
using Repository.DataServices;

namespace Site.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;
        private readonly ICryptographyService cryptographyService;

        public LoginController(IUserService userService, ITokenService tokenService, ICryptographyService cryptographyService)
        {
            this.userService = userService;
            this.tokenService = tokenService;
            this.cryptographyService = cryptographyService;
        }

        public object Post(Credentials credentials)
        {
            var user = userService.GetUserByLogin(credentials.Login);
            if (user != null &&
                cryptographyService.CheckPassword(user.PasswordHash, user.PasswordSalt, credentials.Password))
            {
                var token = tokenService.CreateToken(user.Id, user.Login);
                var response = new TokenResponse
                {
                    Id = user.Id,
                    Login = credentials.Login,
                    Token = token,
                };
                return response;
            }
            return "Unknown user";
        }
    }
}
