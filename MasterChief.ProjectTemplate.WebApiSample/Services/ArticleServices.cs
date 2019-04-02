using System;
using System.Collections.Generic;
using MasterChief.ProjectTemplate.WebApiSample.Models;

namespace MasterChief.ProjectTemplate.WebApiSample.Services
{
    public  class ArticleServices : IArticleServices
    {
        private readonly List<Article> _articles = new List<Article>();

        public ArticleServices()
        {
            //添加演示数据
            Add(new Article
            {
                Id = 1, Title = "DotNet.SampleWebApi1", Content = "DotNet.SampleWebApi", Author = "churenyouzi",
                CreateTime = DateTime.Now
            });
            Add(new Article
            {
                Id = 2, Title = "DotNet.SampleWebApi2", Content = "DotNet.SampleWebApi", Author = "churenyouzi",
                CreateTime = DateTime.Now
            });
            Add(new Article
            {
                Id = 3, Title = "DotNet.SampleWebApi2", Content = "DotNet.SampleWebApi", Author = "churenyouzi",
                CreateTime = DateTime.Now
            });
        }

        public IEnumerable<Article> GetAll()
        {
            return _articles;
        }

        public Article Get(int id)
        {
            //throw new Exception("获取单个文章测试");
            return _articles.Find(p => p.Id == id);
        }

        public Article Add(Article item)
        {
            if (item == null) throw new ArgumentNullException("item");

            _articles.Add(item);
            return item;
        }

        public bool Update(Article item)
        {
            if (item == null) throw new ArgumentNullException("item");

            var index = _articles.FindIndex(p => p.Id == item.Id);

            if (index == -1) return false;

            _articles.RemoveAt(index);
            _articles.Add(item);
            return true;
        }

        public bool Delete(int id)
        {
            _articles.RemoveAll(p => p.Id == id);
            return true;
        }
    }
}