using System;
using System.Security.Principal;
using Repository.Models;

namespace Site.Filters
{
    public class SitePrincipal : IPrincipal
    {
        public SitePrincipal(User user)
        {
            Identity = new SiteIdentity(user);
        }

        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity { get; private set; }
    }

    public class SiteIdentity : IIdentity
    {
        public SiteIdentity(User user)
        {
            Name = user.Login;
            Id = user.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AuthenticationType
        {
            get { return "Forms authentication thourgh HTTP headers."; }
        }

        public bool IsAuthenticated
        {
            get { return Id != Guid.Empty; }
        }
    }
}