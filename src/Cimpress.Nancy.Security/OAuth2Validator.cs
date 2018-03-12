using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Cimpress.Nancy.Components;
using Cimpress.Nancy.Security.Data;
using Microsoft.IdentityModel.Tokens;
using Nancy;
using Newtonsoft.Json;

namespace Cimpress.Nancy.Security
{
    public class OAuth2Validator : IAuthValidator
    {
        private TokenValidationParameters _secretKeyValidationParameters;
        private TokenValidationParameters _jwksKeyValidationParameters;
        private readonly INancyLogger _log;
        private const string Bearer = "Bearer ";
        private readonly ConcurrentDictionary<string, ClaimsPrincipal> _userCache = new ConcurrentDictionary<string, ClaimsPrincipal>();

        private static readonly Regex Base64Regex = new Regex("^(?:[A-Za-z0-9-_]{4})*(?:[A-Za-z0-9-_]{2}==|[A-Za-z0-9-_]{3}=)?$");

        public OAuth2Validator(INancyLogger log)
        {
            _log = log;
        }

        public virtual string OAuth2Issuer => string.Empty;
        public virtual string OAuthSecretKey => string.Empty;
        public virtual string OAuth2ClientId => string.Empty;
        public virtual string OAuth2JwksLocation => string.Empty;
        public virtual string OAuth2JwksAudience => string.Empty;

        /*
         * - and _ are invalid base64 characters that may be URL encoded
         * + and / are the valid characters that replace them
         * http://stackoverflow.com/questions/1228701/code-for-decoding-encoding-a-modified-base64-url
         */
        private byte[] DecodeKey(string auth0SecretKey)
        {
            var convertedString = auth0SecretKey.Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(convertedString);
        }

        private void ConfigureSecretKeyValidationParameters()
        {
            var secretKey = OAuthSecretKey;
            var key = Base64Regex.IsMatch(secretKey) ? DecodeKey(secretKey) : Encoding.ASCII.GetBytes(secretKey);
            var auth0SigningKey = new SymmetricSecurityKey(key);

            _secretKeyValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = OAuth2Issuer,
                ValidAudience = OAuth2ClientId,
                IssuerSigningKey = auth0SigningKey
            };
        }

        private void ConfigureJwksKeyValidationParameters()
        {
            if (string.IsNullOrEmpty(OAuth2JwksLocation))
            {
                _log.Error(new { Message = "No Jwks file configured." });
                return;
            }

            var client = new HttpClient();
            string data;

            try
            {
                data = client.GetStringAsync(OAuth2JwksLocation).Result;
            }
            catch (Exception e)
            {
                _log.Error(new { Message = $"Unable to receive Jwks file from {OAuth2JwksLocation}. Exception: {e}" });
                return;
            }

            var jwks = JsonConvert.DeserializeObject<JwksFile>(data);
            var key = jwks.Keys?.FirstOrDefault()?.X5C?.FirstOrDefault();

            if (string.IsNullOrEmpty(key))
            {
                _log.Error(new { Message = "The public key was not found in the Jwks file." });
                return;
            }

            var certificate = new X509Certificate2(Convert.FromBase64String(key));
            var auth0SigningKey = new X509SecurityKey(certificate);

            _jwksKeyValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = OAuth2Issuer,
                ValidAudiences = new []{ OAuth2JwksAudience },
                IssuerSigningKey = auth0SigningKey,
                IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) => new List<X509SecurityKey> { new X509SecurityKey(certificate) }
            };
        }

        public ClaimsPrincipal GetUserFromContext(NancyContext ctx)
        {
            string jwt = string.Empty;
            try
            {
                jwt = ctx.Request.Headers.Authorization ?? string.Empty;
                if (jwt.StartsWith(Bearer))
                {
                    jwt = jwt.Substring(Bearer.Length);
                }
                //The Authorization header value should be removed, so it won't be logged
                ctx.Request.Headers.Authorization = "...obscured...";
            }
            catch (Exception e)
            {
                _log.Error(new { Message = $"Unable to parse Authorization header: {e}" });
            }

            return _userCache.GetOrAdd(jwt, ValidateUser);
        }

        private ClaimsPrincipal ValidateUser(string tokenString)
        {
            var user = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(tokenString))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();

                    var validationParameters = GetTokenValidationParameters(tokenString, tokenHandler);
                    if (validationParameters == null)
                    {
                        _log.Error(new { Message = "Unable to create validation parameters for the request." });
                        return user;
                    }

                    var validatedClaims = tokenHandler.ValidateToken(tokenString, validationParameters, out var validatedToken);
                    user = validatedClaims;
                }
                catch (SecurityTokenValidationException e)
                {
                    _log.Error(e);
                }
                catch (Exception e)
                {
                    _log.Error(e);// It's safe to swallow this exception since VersionModule will catch the authentication failure
                }
            }

            return user;
        }

        private TokenValidationParameters GetTokenValidationParameters(string tokenString, JwtSecurityTokenHandler tokenHandler)
        {
            var unvalidatedToken = tokenHandler.ReadJwtToken(tokenString);
            TokenValidationParameters validationParameters;
            if (string.IsNullOrEmpty(unvalidatedToken.Header.Kid))
            {
                if (_secretKeyValidationParameters == null)
                {
                    ConfigureSecretKeyValidationParameters();
                }
                validationParameters = _secretKeyValidationParameters;
            }
            else
            {
                if (_jwksKeyValidationParameters == null)
                {
                    ConfigureJwksKeyValidationParameters();
                }
                validationParameters = _jwksKeyValidationParameters;
            }
            return validationParameters;
        }
    }
}