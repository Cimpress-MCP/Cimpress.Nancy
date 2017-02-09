using Nancy;

namespace Cimpress.Nancy.Authentication
{
    public interface IAuthValidator
    {
        UserIdentity GetUserFromContext(NancyContext context);
    }
}