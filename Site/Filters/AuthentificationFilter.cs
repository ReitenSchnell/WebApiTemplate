using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;
using Repository;
using Repository.DataServices;
using Repository.Models;

namespace Site.Filters
{
    public class AuthenticationFilter : IAuthorizationFilter
    {
        public const string TokenHeaderName = "X-Access-Token";

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync
            (HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            User user = null;
            string token = null;
            var anonymousAllowed = actionContext.ControllerOrActionMarkedWith<AllowAnonymousAttribute>();

            var request = actionContext.Request;
            if (request == null)
                return SetUnathorizedResponse(actionContext);

            IEnumerable<string> headerValues;
            var isHeaderFound = request.Headers.TryGetValues(TokenHeaderName, out headerValues);
            var authentHeader = isHeaderFound ? headerValues.FirstOrDefault() : GetTokenFromCookie(request);

            if (!String.IsNullOrEmpty(authentHeader))
            {
                var tokenProvider = actionContext.Request.GetDependencyScope().GetService(typeof(ITokenService)) as ITokenService;
                if (tokenProvider == null)
                    return SetUnathorizedResponse(actionContext);

                user = tokenProvider.GetUser(authentHeader);
                if (user != null)
                {
                    token = tokenProvider.RefreshToken(authentHeader);
                }
            }

            if (user == null)
            {
                if (anonymousAllowed)
                    user = new User();
                else
                {
                    return SetUnathorizedResponse(actionContext);
                }
            }

            SetUserToEveryContext(actionContext, new SitePrincipal(user));

            var response = await continuation();
            if (!string.IsNullOrWhiteSpace(token))
            {
                response.Headers.Add(TokenHeaderName, token);
                return response;
            }
            return response;
        }

        private static void SetUserToEveryContext(HttpActionContext actionContext, IPrincipal principal)
        {
            actionContext.ControllerContext.RequestContext.Principal = principal;
            var service = actionContext.Request.GetDependencyScope().GetService(typeof(IUserContext)) as IUserContext;
            if (service != null)
                service.User = principal;
        }

        private static string GetTokenFromCookie(HttpRequestMessage request)
        {
            var cookieHeaderValue = request.Headers.GetCookies(FormsAuthentication.FormsCookieName).FirstOrDefault();
            if (cookieHeaderValue == null)
                return null;
            var targetCookie = cookieHeaderValue.Cookies.FirstOrDefault(cookie => cookie.Name == FormsAuthentication.FormsCookieName);
            return targetCookie == null ? null : targetCookie.Value;
        }

        private static HttpResponseMessage SetUnathorizedResponse(HttpActionContext actionContext)
        {
            return actionContext.Request.CreateErrorResponse(
                HttpStatusCode.Unauthorized, "Authentication ticket was not specified or it is wrong.");
        }

        public bool AllowMultiple { get { return true; } }
    }
}