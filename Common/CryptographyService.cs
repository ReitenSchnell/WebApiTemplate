using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public interface ICryptographyService
    {
        string GetPasswordHash(string password, Guid salt);
        bool CheckPassword(string passwordHash, Guid passwordSalt, string password);
    }

    public class CryptographyService : ICryptographyService
    {
        public string GetPasswordHash(string password, Guid salt)
        {
            using (var sha1 = new SHA1Managed())
            {
                var bytes = Encoding.UTF8.GetBytes(password + salt);
                var hash = sha1.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public bool CheckPassword(string passwordHash, Guid passwordSalt, string password)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                return false;
            }
            var hash = GetPasswordHash(password, passwordSalt);
            return passwordHash == hash;
        }
    }
}
