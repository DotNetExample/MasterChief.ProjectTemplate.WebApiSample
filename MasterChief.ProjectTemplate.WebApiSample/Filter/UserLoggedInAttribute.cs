using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using MasterChief.DotNet.Core.Config;
using MasterChief.DotNet.ProjectTemplate.WebApi;
using MasterChief.DotNet.ProjectTemplate.WebApi.Filter;
using MasterChief.DotNet.ProjectTemplate.WebApi.Helper;
using MasterChief.DotNet4.Utilities.Common;

namespace MasterChief.ProjectTemplate.WebApiSample.Filter
{
    public sealed class UserLoggedInAttribute : AuthenticateAttribute
    {
        public UserLoggedInAttribute() : this(new JwtApiAuthenticate(), new AppConfigService(new CacheConfigContext()))
        {
        }


        public UserLoggedInAttribute(IApiAuthenticate apiAuthenticate, IAppConfigService appCfgService) : base(
            apiAuthenticate, appCfgService)
        {
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var token = actionContext.Request.GetUriOrHeaderValue("Access_token").ToStringOrDefault(string.Empty);
            var appId = actionContext.Request.GetUriOrHeaderValue("Access_appId").ToGuidOrDefault();
            var result = CheckIdentityToken(token, appId);
            if (!result.State)
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, result);
            else
                base.OnAuthorization(actionContext);
        }
    }
}