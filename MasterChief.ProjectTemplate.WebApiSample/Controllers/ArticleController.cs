using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using MasterChief.DotNet4.Utilities.Result;
using MasterChief.ProjectTemplate.WebApiSample.Filter;
using MasterChief.ProjectTemplate.WebApiSample.Models;
using MasterChief.ProjectTemplate.WebApiSample.Request;
using MasterChief.ProjectTemplate.WebApiSample.Services;

namespace MasterChief.ProjectTemplate.WebApiSample.Controllers
{
    public class ArticleController : ApiController
    {
        public ArticleController():this(new ArticleServices())
        {
        }

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
        public async Task<OperatedResult<Article>> Get([FromUri] ArticleRequest request)
        {
            var keyId = request.Id;

            return await Task.Run(() => OperatedResult<Article>.Success(_articleServices.Get(keyId)));
        }
    }
}