using Cimpress.Nancy.Components;
using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace Cimpress.Nancy.Services
{
    public class ContentTypeVersionModuleExtender : IVersionModuleExtender
    {
        private readonly IEnumerable<string> _entityMethods = new List<string> {"put", "post", "patch"};

        public void After(NancyContext context)
        {

        }

        public Response Before(NancyContext context)
        {
            var methodType = context.Request.Method.ToLower();
            if (_entityMethods.Contains(methodType) && context.Request.Headers.ContentType != null)
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Content-Type Header not supplied"
                };
            }

            return null;
        }

        public int Priority => int.MaxValue; // Process this last
    }
}
