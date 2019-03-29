using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using MasterChief.DotNet.ProjectTemplate.WebApi.Filter;
using MasterChief.DotNet4._5.Utilities.Model;

namespace MasterChief.ProjectTemplate.WebApiSample.Filter
{
    public sealed class ExceptionLogAttribute : ControllerExceptionAttribute
    {
        public override void OnActionExceptioning(HttpActionExecutedContext actionExecutedContext, string actionName,
            HttpStatusCode statusCode,
            HttpRequestRaw requestRaw)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent("发生故障，请稍后重试！"),
                StatusCode = statusCode
            };
            actionExecutedContext.Response = response;
        }
    }
}