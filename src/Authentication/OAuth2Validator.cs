using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Cimpress.Nancy.Logging;
using log4net;
using Microsoft.IdentityModel.Tokens;
using Nancy;

namespace Cimpress.Nancy.Authentication
{
    public class OAuth2Validator : IAuthValidator
    {
        private readonly TokenValidationParameters _auth0TokenValidationParameters;
        private readonly ILog _log;
        private const string Name = "name";
        private const string Bearer = "Bearer ";
        private readonly IDictionary<string, UserIdentity> _userCache = new Dictionary<string, UserIdentity>();

        private static readonly Regex Base64Regex = new Regex("^(?:[A-Za-z0-9-_]{4})*(?:[A-Za-z0-9-_]{2}==|[A-Za-z0-9-_]{3}=)?$");

        public OAuth2Validator(ILog log)
        {
            var key = Base64Regex.IsMatch(OAuthSecretKey) ? DecodeKey(OAuthSecretKey) : Encoding.ASCII.GetBytes(OAuthSecretKey);
            var auth0SigningKey = new SymmetricSecurityKey(key);

            _auth0TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = OAuth2Issuer,
                ValidAudience = OAuth2ClientId,
                IssuerSigningKey = auth0SigningKey
            };
            _log = log;
        }

        public virtual string OAuth2Issuer => string.Empty;
        public virtual string OAuthSecretKey => string.Empty;
        public virtual string OAuth2ClientId => string.Empty;

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

        public UserIdentity GetUserFromContext(NancyContext ctx)
        {
            string jwt = string.Empty;
            try
            {
                jwt = ctx.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
                if (jwt.StartsWith(Bearer))
                {
                    jwt = jwt.Substring(Bearer.Length);
                }
            }
            catch (Exception e)
            {
                _log.Error(new BaseMessage { Message = $"Unable to parse Authorization header: {e}" });
            }
            UserIdentity user;
            var userInCache = _userCache.TryGetValue(jwt, out user);
            if (!userInCache)
            {
                user = ValidateUser(jwt);
                _userCache[jwt] = user;
            }
            if (user != null && user.Valid && user.ExpirationTime < DateTime.UtcNow)
            {
                user.Valid = false;
            }

            return user;
        }

        private UserIdentity ValidateUser(string tokenString)
        {
            var user = new UserIdentity { Valid = false };
            if (!string.IsNullOrEmpty(tokenString))
            {
                try
                {
                    SecurityToken validatedToken;
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validatedClaims = tokenHandler.ValidateToken(tokenString, _auth0TokenValidationParameters, out validatedToken);
                    var jwtSecurityToken = validatedToken as JwtSecurityToken;
                    
                    var auth0User = ParseUser(validatedClaims.Claims);
                    user.UserName = auth0User.Name;
                    user.UserId = jwtSecurityToken.Subject;
                    user.Valid = true;
                    user.ExpirationTime = jwtSecurityToken.ValidTo.ToUniversalTime();
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

        private Auth0User ParseUser(IEnumerable<Claim> claims)
        {
            var user = new Auth0User
            {
                Name = string.Empty
            };

            foreach (var claim in claims)
            {
                if (claim.Type == Name)
                {
                    user.Name = claim.Value;
                }
            }

            return user;
        }
    }
}