using System;

namespace Common.Contracts
{
    public class TokenResponse
    {
        public string Login { get; set; }
        public Guid Id { get; set; }
        public string Token { get; set; }
    }
}