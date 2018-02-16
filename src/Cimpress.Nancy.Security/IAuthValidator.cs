using System.Security.Claims;
using Nancy;

namespace Cimpress.Nancy.Security
{
    public interface IAuthValidator
    {
        ClaimsPrincipal GetUserFromContext(NancyContext context);
    }
}