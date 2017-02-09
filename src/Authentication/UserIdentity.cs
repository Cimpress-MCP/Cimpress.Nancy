using System;
using System.Security.Claims;

namespace Cimpress.Nancy.Authentication
{
    public class UserIdentity : ClaimsPrincipal
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public bool Valid { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}