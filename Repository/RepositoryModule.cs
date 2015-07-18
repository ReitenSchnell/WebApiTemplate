using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Ninject.Web.Common;

namespace Repository
{
    public class RepositoryModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(_ =>
              _.FromThisAssembly()
              .SelectAllClasses()
              .BindAllInterfaces()
              );
            Kernel.Rebind<IUserContext>().To<UserContext>().InRequestScope();
        }
    }
}