using System.Collections.Generic;
using MasterChief.ProjectTemplate.WebApiSample.Models;

namespace MasterChief.ProjectTemplate.WebApiSample.Services
{
    /// <summary>
    ///     文章接口
    /// </summary>
    public interface IArticleServices
    {
        /// <summary>
        ///     获取全部文章
        /// </summary>
        /// <returns>集合</returns>
        IEnumerable<Article> GetAll();

        /// <summary>
        ///     通过ID获取文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Article Get(int id);

        /// <summary>
        ///     添加文章
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Article Add(Article item);

        /// <summary>
        ///     更新文章
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Update(Article item);

        /// <summary>
        ///     删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
    }
}