using System;
using Newtonsoft.Json;

namespace Common
{
    public class AuthenticationToken
    {
        [JsonProperty]
        public Guid UserId { get; private set; }

        [JsonProperty]
        public string Login { get; private set; }

        [JsonProperty]
        public DateTime Expired { get; private set; }

        public AuthenticationToken()
        {
        }

        public AuthenticationToken(Guid id, string login, int expirationPeriod)
        {
            Login = login;
            UserId = id;
            Expired = DateTime.UtcNow.AddMinutes(expirationPeriod);
        }

        public bool IsExpired()
        {
            return Expired < DateTime.UtcNow;
        }
    }
}