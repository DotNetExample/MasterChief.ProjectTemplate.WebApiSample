using System.Web.Http.Controllers;
using MasterChief.DotNet.Core.Cache;
using MasterChief.DotNet.ProjectTemplate.WebApi.Filter;
using MasterChief.DotNet.ProjectTemplate.WebApi.Helper;
using MasterChief.DotNet4.Utilities.Common;

namespace MasterChief.ProjectTemplate.WebApiSample.Filter
{
    public class RequestCacheAttribute : ControllerCacheAttribute
    {
        public RequestCacheAttribute(int cacheMinutes) : this(cacheMinutes, true, new LocalCacheProvider())
        {
        }

        public RequestCacheAttribute(int cacheMinutes, bool dependsOnIdentity, ICacheProvider cacheProvider) : base(
            cacheMinutes, dependsOnIdentity, cacheProvider)
        {
        }

        protected override bool CheckedResponseAvailable(HttpActionContext context, string responseText)
        {
            return !string.IsNullOrEmpty(responseText) && context != null;
        }

        protected override string GetIdentityToken(HttpActionContext actionContext)
        {
            return actionContext.Request.GetUriOrHeaderValue("Access_token").ToStringOrDefault(string.Empty);
        }
    }
}