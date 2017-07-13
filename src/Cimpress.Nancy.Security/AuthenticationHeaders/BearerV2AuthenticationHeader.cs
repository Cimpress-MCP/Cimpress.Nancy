using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace Cimpress.Nancy.Security.AuthenticationHeaders
{
    public class BearerV2AuthenticationHeader : IAuthenticationHeader
    {
        private readonly string _aud;
        private readonly string _auth0OauthTokenUri;
        private readonly string _service;

        public BearerV2AuthenticationHeader(string aud, string auth0OauthTokenUri)
        {
            _aud = aud;
            _auth0OauthTokenUri = auth0OauthTokenUri;
        }

        public void AddHeader(Response response, Request request)
        {
            response.WithHeader("WWW-Authenticate", $"Bearer realm=\"{_aud}\", authorization_uri=\"{_auth0OauthTokenUri}\"");
            response.WithHeader("Link", $"{_auth0OauthTokenUri};rel=authorization_uri");
        }
    }
}
