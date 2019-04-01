using System;
using System.Threading.Tasks;
using System.Web.Http;
using MasterChief.DotNet.Core.Config;
using MasterChief.DotNet.ProjectTemplate.WebApi;
using MasterChief.DotNet.ProjectTemplate.WebApi.Model;
using MasterChief.DotNet.ProjectTemplate.WebApi.Result;
using MasterChief.DotNet4.Utilities.Common;
using MasterChief.DotNet4.Utilities.Result;

namespace MasterChief.ProjectTemplate.WebApiSample.Controllers
{
    public class AccountController : AuthorizeController
    {
        public AccountController() : this(new JwtApiAuthorize(), new AppConfigService(new CacheConfigContext()))
        {

        }
        public AccountController(IApiAuthorize apiAuthorize, IAppConfigService appCfgService) : base(
            apiAuthorize,
            appCfgService)
        {
        }

        protected override CheckResult<IdentityUser> GetIdentityUser(string userId, string passWord)
        {
            if (userId == "2c96ff542072420bc8d33bdd73bb9488" && passWord == "0000")
                return CheckResult<IdentityUser>.Success(new IdentityUser
                { UserId = userId.ToGuidOrDefault(Guid.Empty), Password = passWord });
            return CheckResult<IdentityUser>.Fail("用户名称或密码错误。");
        }

        [HttpPost]
        public async Task<ApiResult<IdentityToken>> GetIdentityToken(string userId, string passWord,
            string signature, string timestamp, string nonce, Guid appid)
        {
            
            return await Task.Run(() => base.CreateIdentityToken(userId, passWord, signature, timestamp, nonce, appid));
        }
    }
}