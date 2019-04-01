using MasterChief.DotNet.Core.Config;
using MasterChief.DotNet.ProjectTemplate.WebApi;
using MasterChief.DotNet.ProjectTemplate.WebApi.Filter;

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
    }
}