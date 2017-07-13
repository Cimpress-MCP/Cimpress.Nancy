using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace Cimpress.Nancy.Security.AuthenticationHeaders
{
    public class SimpleAuthenticationHeader : IAuthenticationHeader
    {
        private readonly string _header;

        public SimpleAuthenticationHeader(string header)
        {
            _header = header;
        }

        public void AddHeader(Response response, Request request)
        {
            response.WithHeader("WWW-Authenticate", _header);
        }
    }
}
