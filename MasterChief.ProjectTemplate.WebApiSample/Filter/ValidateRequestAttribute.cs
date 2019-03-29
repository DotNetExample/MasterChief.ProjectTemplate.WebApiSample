using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using MasterChief.DotNet.ProjectTemplate.WebApi.Filter;
using MasterChief.DotNet.ProjectTemplate.WebApi.Model;
using MasterChief.DotNet4.Utilities.Result;

namespace MasterChief.ProjectTemplate.WebApiSample.Filter
{
    /// <summary>
    /// 请求参数
    /// </summary>
    public sealed class ValidateRequestAttribute : ValidateModelAttribute
    {
        public override void OnParameterIsNulling(HttpActionContext actionContext)
        {
            actionContext.Response =
                actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, OperatedResult<string>.Fail("请求参数非法。"));
        }

        public override void OnParameterInvaliding(HttpActionContext actionContext, ValidationFailedResult result)
        {
            var message = result.Data.FirstOrDefault()?.Message;
            actionContext.Response =
                actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, OperatedResult<string>.Fail(message));
        }
    }
}