using System;
using System.ComponentModel.DataAnnotations;

namespace MasterChief.ProjectTemplate.WebApiSample.Models
{
    /// <summary>
    ///     文章实体类
    /// </summary>
    public class Article
    {
        /// <summary>
        ///     文章ID
        /// </summary>
        [Required(ErrorMessage = "文章ID")]
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     文章标题
        /// </summary>
        [Required(ErrorMessage = "文章标题")]
        public string Title { get; set; }

        /// <summary>
        ///     文章作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     文章内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Required(ErrorMessage = "创建时间")]
        public DateTime CreateTime { get; set; }
    }
}