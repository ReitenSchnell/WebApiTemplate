using System;

namespace Common
{
    public interface IIdentityGenerator
    {
        Guid GetIdentity();
    }

    public class IdentityGenerator : IIdentityGenerator
    {
        public Guid GetIdentity()
        {
            return Guid.NewGuid();
        }
    }
}