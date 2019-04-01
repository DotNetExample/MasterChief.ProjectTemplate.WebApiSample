using System.Net.Http.Formatting;
using System.Web.Http;
using MasterChief.DotNet.ProjectTemplate.WebApi.Component;
using MasterChief.ProjectTemplate.WebApiSample.Filter;
using Newtonsoft.Json;

namespace MasterChief.ProjectTemplate.WebApiSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );

            var jsonFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat //,
                    //ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                }
            };

            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            //config.EnableSystemDiagnosticsTracing();
            config.Filters.Add(new ValidateRequestAttribute());
            config.Filters.Add(new ExceptionLogAttribute());
        }
    }
}