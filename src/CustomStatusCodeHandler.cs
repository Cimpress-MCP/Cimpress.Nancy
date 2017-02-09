using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ErrorHandling;

namespace Cimpress.Nancy
{
    public class CustomStatusCodeHandler : IStatusCodeHandler
    {
        private IEnumerable<HttpStatusCode> statusCodes = new[]{ HttpStatusCode.NotFound, HttpStatusCode.InternalServerError };

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCodes.Contains(statusCode);
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {

        }
    }
}
