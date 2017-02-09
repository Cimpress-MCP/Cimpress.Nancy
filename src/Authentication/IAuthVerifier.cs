using Nancy;

namespace Cimpress.Nancy.Authentication
{
    public interface IAuthVerifier
    {
        Response HandleBefore(NancyContext context);
        void HandleAfter(NancyContext context);
    }
}