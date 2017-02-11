using Nancy;

namespace Cimpress.Nancy.Security
{
    public interface IAuthVerifier
    {
        Response HandleBefore(NancyContext context);
        void HandleAfter(NancyContext context);
    }
}