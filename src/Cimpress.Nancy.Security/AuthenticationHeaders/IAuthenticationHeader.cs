using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace Cimpress.Nancy.Security.AuthenticationHeaders
{
    public interface IAuthenticationHeader
    {
        void AddHeader(Response response, Request request);
    }
}
