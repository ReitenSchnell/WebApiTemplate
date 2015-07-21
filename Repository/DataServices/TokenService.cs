using System;
using Common;
using Repository.Models;

namespace Repository.DataServices
{
    public interface ITokenService
    {
        string CreateToken(Guid userId, string login);
        string RefreshToken(string token);
        User GetUser(string token);
    }

    public class TokenService : ITokenService
    {
        public const int AuthenticationExpirationPeriod = 20;
        private readonly IEncryptor encryptor;
        private readonly IUserService userService;

        public TokenService(IEncryptor encryptor, IUserService userService)
        {
            this.encryptor = encryptor;
            this.userService = userService;
        }

        public string CreateToken(Guid userId, string login)
        {
            var token = new AuthenticationToken(userId, login, AuthenticationExpirationPeriod);
            return encryptor.Encrypt(token);
        }

        public string RefreshToken(string token)
        {
            var data = encryptor.Decrypt<AuthenticationToken>(token);
            if (data == null)
                return null;
            return data.Expired == DateTime.MaxValue ? token : CreateToken(data.UserId, data.Login);
        }

        public User GetUser(string token)
        {
            var authenticationData = encryptor.Decrypt<AuthenticationToken>(token);
            if (authenticationData == null || authenticationData.IsExpired())
            {
                return null;
            }

            return userService.GetUserByLogin(authenticationData.Login);
        }
    }
}