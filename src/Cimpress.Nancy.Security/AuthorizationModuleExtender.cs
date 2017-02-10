using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Components;
using Nancy;

namespace Cimpress.Nancy.Security
{
    public class AuthorizationModuleExtender : IVersionModuleExtender
    {
        private readonly IAuthVerifier _authVerifier;

        public AuthorizationModuleExtender(IAuthVerifier authVerifier)
        {
            _authVerifier = authVerifier;
            Priority = 0;
        }

        public void After(NancyContext context)
        {
            _authVerifier.HandleAfter(context);
        }

        public Response Before(NancyContext context)
        {
            return _authVerifier.HandleBefore(context);
        }

        public int Priority { get; }
    }
}
