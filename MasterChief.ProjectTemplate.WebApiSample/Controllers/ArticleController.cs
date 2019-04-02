using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using MasterChief.DotNet.ProjectTemplate.WebApi.Result;
using MasterChief.ProjectTemplate.WebApiSample.Filter;
using MasterChief.ProjectTemplate.WebApiSample.Models;
using MasterChief.ProjectTemplate.WebApiSample.Request;
using MasterChief.ProjectTemplate.WebApiSample.Services;

namespace MasterChief.ProjectTemplate.WebApiSample.Controllers
{
    public class ArticleController : ApiController
    {
        private readonly IArticleServices _articleServices;

        public ArticleController(IArticleServices articleServices)
        {
            _articleServices = articleServices;
        }

        /// <summary>
        ///     获取单个文章
        /// </summary>
        /// <param name="request">ArticleRequest</param>
        /// <returns>Article</returns>
        [HttpPost]
        [ValidateRequest]
        [UserLoggedIn]
        [RequestCache(30)]
        public async Task<ApiResult<Article>> Get([ModelBinder] ArticleRequest request)
        {
            var keyId = request.Id;
            // throw new ArgumentException("exception test"); 异常测试
            return await Task.Run(() => ApiResult<Article>.Success(_articleServices.Get(keyId)));
        }
    }
}