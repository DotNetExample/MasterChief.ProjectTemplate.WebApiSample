using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using MasterChief.DotNet.ProjectTemplate.WebApi.Component;
using MasterChief.ProjectTemplate.WebApiSample.Filter;
using MasterChief.ProjectTemplate.WebApiSample.Services;
using Newtonsoft.Json;

namespace MasterChief.ProjectTemplate.WebApiSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            #region Autofac IOC

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).AsSelf().PropertiesAutowired();
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            builder.RegisterWebApiModelBinderProvider();
            builder.RegisterInstance<IArticleServices>(new ArticleServices());
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            #endregion Autofac IOC

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{keyId}",
                new {keyId = RouteParameter.Optional}
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
            config.Filters.Add(new UserLoggedInAttribute());
        }
    }
}