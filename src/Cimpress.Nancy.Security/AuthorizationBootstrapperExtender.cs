using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Components;
using Nancy;
using Nancy.TinyIoc;

namespace Cimpress.Nancy.Security
{
    public class AuthorizationBootstrapperExtender : IBootstrapperExtender
    {
        private readonly IAuthValidator _authValidator;

        public AuthorizationBootstrapperExtender(IAuthValidator authValidator)
        {
            _authValidator = authValidator;
            Priority = 0;
        }

        public void Initialize(NancyServiceBootstrapper bootstrapper, TinyIoCContainer container)
        {
        }

        public void OnAfterRequest(NancyContext context, IDictionary<string, object> logData)
        {
        }

        public Response OnBeforeRequest(NancyContext context, IDictionary<string, object> logData)
        {
            var currentUser = _authValidator.GetUserFromContext(context);
            context.CurrentUser = currentUser;
            logData["User"] = currentUser;
            return null;
        }

        public void OnError(NancyContext context, Exception ex, Response newResponse, IDictionary<string, object> logData)
        {
        }

        public int Priority { get; }
    }
}
