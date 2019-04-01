using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterChief.ProjectTemplate.WebApiSample.Request
{
    public sealed class ArticleRequest
    {
        [Required(ErrorMessage = "缺少文章ID")]
        public int Id
        {
            get;
            set;
        }

    }
}