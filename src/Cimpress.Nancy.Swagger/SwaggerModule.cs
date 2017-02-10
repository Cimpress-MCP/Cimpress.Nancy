using Nancy;

namespace Cimpress.Nancy.Swagger
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
