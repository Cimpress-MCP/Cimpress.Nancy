using Nancy;

namespace Cimpress.Nancy.Components
{
    public interface IVersionModuleExtender
    {
        void After(NancyContext context);
        Response Before(NancyContext context);
        int Priority { get; }
    }
}
