using Nancy;

namespace Cimpress.Nancy.Modules
{
    public class SwaggerModule : NancyModule
    {
        public SwaggerModule() : base("/Swagger")
        {
            Get("/", _ => Swagger(), name: "Index");
        }

        public dynamic Swagger()
        {
            return Negotiate.WithView("index");
        }
    }
}
