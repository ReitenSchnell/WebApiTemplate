using System.Linq;
using System.Web.Http.Controllers;

namespace Site.Filters
{
    public static class FilterExtensions
    {
        public static bool ControllerOrActionMarkedWith<TAttr>(this HttpActionContext actionContext)
            where TAttr : class
        {
            return ActionHasAttribute<TAttr>(actionContext) || ControllerHasAttribute<TAttr>(actionContext);
        }

        public static bool ControllerHasAttribute<TAttribute>(this HttpActionContext actionCtx)
            where TAttribute : class
        {
            var attributes = actionCtx.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<TAttribute>();
            return attributes != null && attributes.Any();
        }

        public static bool ActionHasAttribute<TAttribute>(this HttpActionContext actionCtx)
            where TAttribute : class
        {
            var attributes = actionCtx.ActionDescriptor.GetCustomAttributes<TAttribute>();
            return attributes != null && attributes.Any();
        }
    }
}