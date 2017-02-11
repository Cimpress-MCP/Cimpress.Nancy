using Nancy;

namespace Cimpress.Nancy.Security
{
    public interface IAuthValidator
    {
        UserIdentity GetUserFromContext(NancyContext context);
    }
}