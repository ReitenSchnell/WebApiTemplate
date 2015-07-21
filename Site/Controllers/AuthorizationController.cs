using System.Web.Http;
using System.Web.Http.Cors;
using Common;
using Common.Contracts;
using Repository.DataServices;

namespace Site.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/authorization")]
    public class AuthorizationController : ApiController
    {
        public const string UnknownUserMessage = "Unknown user";
        public const string UserAlreadyExistsMessage = "User already exists";
        private readonly IUserService userService;
        private readonly ITokenService tokenService;
        private readonly ICryptographyService cryptographyService;

        public AuthorizationController(IUserService userService, ITokenService tokenService, ICryptographyService cryptographyService)
        {
            this.userService = userService;
            this.tokenService = tokenService;
            this.cryptographyService = cryptographyService;
        }

        [Route("signin")]
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
            return UnknownUserMessage;
        }

        [Route("signup")]
        public object Post(RegistrationInfo registrationInfo)
        {
            var user = userService.GetUserByLogin(registrationInfo.Login);
            if (user != null)
                return UserAlreadyExistsMessage;
            var userId = userService.CreateUser(registrationInfo);
            var token = tokenService.CreateToken(userId, registrationInfo.Login);
            var response = new TokenResponse
            {
                Id = userId,
                Login = registrationInfo.Login,
                Token = token,
            };
            return response;
        }
    }
}
