using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace Common
{
    public class CommonModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(_ =>
             _.FromThisAssembly()
             .SelectAllClasses()
             .BindAllInterfaces()
             );
        }
    }
}