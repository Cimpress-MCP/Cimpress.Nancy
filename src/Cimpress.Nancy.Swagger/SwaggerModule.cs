using Cimpress.Nancy.Config;
using Nancy;

namespace Cimpress.Nancy.Swagger
{
    public class SwaggerModule : NancyModule
    {
        public static readonly string SwaggerUiEndpointUrlKey = "SwaggerUiEndpointUrl";
        public static readonly string SwaggerUiRedirectUrlKey = "SwaggerUiRedirectUrl";
        private readonly string _swaggerUiEndpointUrl;
        private readonly string _swaggerUiRedirectUrl;

        public SwaggerModule(IConfiguration configuration) : base(string.Empty)
        {
            var parameters = configuration?.OptionalParameters;

            var containsEndpointUrl = parameters?.ContainsKey(SwaggerUiEndpointUrlKey) ?? false;
            _swaggerUiEndpointUrl = containsEndpointUrl ? parameters[SwaggerUiEndpointUrlKey] : "/swagger/";

            var containsRedirectUrl = parameters?.ContainsKey(SwaggerUiRedirectUrlKey) ?? false;
            _swaggerUiRedirectUrl = containsRedirectUrl ? parameters[SwaggerUiRedirectUrlKey] : null;

            Get(_swaggerUiEndpointUrl, _ => GetSwaggerUi(), name: "GetSwaggerUi");
        }

        public dynamic GetSwaggerUi()
        {
            var redirectUrl = _swaggerUiRedirectUrl;
            if (string.IsNullOrEmpty(redirectUrl))
            {
                var port = Request.Url.Port ?? 80;
                var host = Request.Url.HostName ?? "localhost";
                redirectUrl = $"http://petstore.swagger.io/?url=http://{host}:{port}/api-docs";
            }

            return Response.AsRedirect(redirectUrl);
        }
    }
}
