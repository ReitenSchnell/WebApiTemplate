using System.Security.Principal;

namespace Repository
{
    public interface IUserContext
    {
        IPrincipal User { get; set; }
    }

    public class UserContext : IUserContext
    {
        public IPrincipal User { get; set; }
    }
}