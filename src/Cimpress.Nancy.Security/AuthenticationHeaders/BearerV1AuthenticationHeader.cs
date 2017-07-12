using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace Cimpress.Nancy.Security.AuthenticationHeaders
{
    public class BearerV1AuthenticationHeader : IAuthenticationHeader
    {
        private readonly string _realm;
        private readonly string _clientId;
        private readonly string _service;

        public BearerV1AuthenticationHeader(string realm, string clientId, string service = null)
        {
            _realm = realm;
            _clientId = clientId;
            _service = service;
        }

        public void AddHeader(Response response, Request request)
        {
            var service = _service;
            if (string.IsNullOrEmpty(_service))
            {
                service = $"{request.Url.SiteBase}";
            }
            var header = $"Bearer realm=\"{_realm}\", scope=\"client_id={_clientId} service={service}\"";
            response.WithHeader("WWW-Authenticate", header);
        }
    }
}
