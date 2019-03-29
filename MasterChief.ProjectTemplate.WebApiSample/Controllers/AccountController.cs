using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterChief.DotNet.ProjectTemplate.WebApi;
using MasterChief.DotNet.ProjectTemplate.WebApi.Model;
using MasterChief.DotNet4.Utilities.Result;
using MasterChief.DotNet4.Utilities.Common;
namespace MasterChief.ProjectTemplate.WebApiSample.Controllers
{
    public class AccountController : AuthorizeController
    {


        public AccountController(IApiAuthorize apiAuthorize, IAppConfigService appCfgService) : base(apiAuthorize, appCfgService)
        {
        }

        public AccountController(IAppConfigService appConfig) : base(appConfig)
        {
        }

        protected override CheckResult<IdentityUser> GetIdentityUser(string userId, string passWord)
        {
            if (userId == "0000" && passWord == "0000")
                return CheckResult<IdentityUser>.Success(new IdentityUser() { UserId = userId.ToGuidOrDefault(Guid.Empty), Password = passWord });
            return CheckResult<IdentityUser>.Fail("用户名称或密码错误。");
        }

        protected override OperatedResult<IdentityToken> CreateIdentityToken(string userId, string passWord, string signature, string timestamp, string nonce, Guid appid)
        {
            return base.CreateIdentityToken(userId, passWord, signature, timestamp, nonce, appid);
        }
    }
}