using MasterChief.DotNet.ProjectTemplate.WebApi;

namespace MasterChief.ProjectTemplate.WebApiSample.Filter
{
    public sealed class UserLoggedInAttribute : ApiAuthenticateAttribute
    {
        public UserLoggedInAttribute(IApiAuthenticate apiAuthenticate, IAppConfigService appCfgService) : base(
            new JwtApiAuthenticate(), new AppConfigService(new CacheConfigContext()))
        {
        }
    }
}