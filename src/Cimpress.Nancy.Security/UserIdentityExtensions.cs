using System;
using System.Linq;
using System.Security.Claims;
using Nancy.Security;

namespace Cimpress.Nancy.Security
{
    public static class UserIdentityExtensions
    {
        private const string Name = "name";
        private const string Expiration = "exp";
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string GetUserName(this ClaimsPrincipal cp)
        {
            return cp.Claims.FirstOrDefault(x => x.Type == Name)?.Value ?? string.Empty;
        }

        public static string GetUserId(this ClaimsPrincipal cp)
        {
            return cp.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        public static bool IsValid(this ClaimsPrincipal cp)
        {
            return cp.IsAuthenticated() && cp.GetExpirationTime() > DateTime.UtcNow;
        }

        public static DateTime GetExpirationTime(this ClaimsPrincipal cp)
        {
            var expirationTimeClaim = cp.Claims.FirstOrDefault(x => x.Type == Expiration);
            if (expirationTimeClaim != null)
            {
                if (!int.TryParse(expirationTimeClaim.Value, out var expirationTime))
                {
                    throw new ArgumentException($"Expiration time could not be parsed: {expirationTimeClaim.Value}");
                }
                return Epoch.AddSeconds(expirationTime);
            }
            return DateTime.MinValue;
        }
    }
}