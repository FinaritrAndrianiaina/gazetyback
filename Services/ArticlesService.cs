using GazetyBack.Controllers;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using GazetyBack.Models;
using GazetyBack.Utils;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace GazetyBack.Services
{
    public class ArticlesService
    {

        private DatabaseContext _ctx;
        private Crud<Article> _crudArticle;
        private Crud<User> _crudUser;

        public ArticlesService(DatabaseContext context)
        {
            _ctx = context;
            _crudArticle = new Crud<Article>(context);
            _crudUser = new Crud<User>(context);
        }


        public async Task<List<ArticleModel>> AllListModel()
        {
            var listArticle = await _crudArticle.GetAll();
            List<ArticleModel> listArticleModel=new List<ArticleModel>();
            listArticle.ForEach(article=>{
                var _article = ToArticleModel(article);
                listArticleModel.Add(_article);
            });
            return listArticleModel;
        }

        public  ArticleModel ToArticleModel(Article article)
        {
            var author = _ctx.Users.Single(user=>user.Id==article.AuthorId);
            return new ArticleModel
            {
                Id = article.Id,

                Titre = article.Titre,
                Contenu = article.Contenu,
                Description = article.Description,
                Cover = article.Cover,

                CreatedAt = article.CreatedAt,

                Author = new UserData
                {
                    Id= author.Id,
                    Username = author.Username
                }
            };
        }

        public List<Article> GetArticle()
        {
            var articles = _ctx.Articles.ToList<Article>();
            return articles;
        }
    }
}