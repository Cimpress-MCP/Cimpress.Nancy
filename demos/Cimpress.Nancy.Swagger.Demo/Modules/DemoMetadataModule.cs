using System;
using Cimpress.Nancy.Components;
using Cimpress.Nancy.Modules;
using Nancy;
using Nancy.Swagger;
using Nancy.Swagger.Modules;
using Nancy.Swagger.Services;
using Nancy.Swagger.Services.RouteUtils;

namespace Cimpress.Nancy.Swagger.Demo.Modules
{
    public class DemoMetadataModule : SwaggerMetadataModule
    {
        public DemoMetadataModule(ISwaggerModelCatalog modelCatalog, ISwaggerTagCatalog tagCatalog) : base(modelCatalog, tagCatalog)
        {
            RouteDescriber.DescribeRoute("Demo", string.Empty, string.Empty, new []
            {
                new HttpResponseMetadata {Code = 200, Message = "OK"}, 
            }, null);
        }
    }
}
