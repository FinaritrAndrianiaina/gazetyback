using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using GazetyBack.Models;
using GazetyBack.Utils;
using GazetyBack.Services;

namespace GazetyBack.Controllers
{
    [ApiController]
    [Route("Article")]
    public class ArticleController : ControllerBase
    {

        private readonly DatabaseContext _ctx;
        private Crud<Article> _crud;

        private UserService userService;
        private ArticlesService articleService;
        public ArticleController(DatabaseContext context)
        {
            _ctx = context;
            userService = new UserService(context);
            articleService = new ArticlesService(context);
            _crud = new Crud<Article>(_ctx);
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await articleService.AllListModel());
        }


        [HttpGet("{id}")]
        public ActionResult GetOne(long id)
        {
            var data = _ctx.Set<Article>().Where(article => article.Id == id).First();
            if (data != null)
            {
                return Ok(articleService.ToArticleModel(data));
            }
            return NotFound(new
            {
                Message = $"Article with Id={id} not found"
            });
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateOne(long id, ArticleDto article)
        {
            var data = _ctx.Set<Article>().Where(article => article.Id == id).First();
            if (data != null)
            {
                var _user = HttpContext.User;
                User user = userService.GetUserWithClaim(_user);
                data.Titre = article.Titre;
                data.Contenu = article.Contenu;
                data.Cover = article.Cover;
                data.Description = article.Description;
                var entry = _ctx.Set<Article>().Update(data);
                var updatedData = articleService.ToArticleModel(entry.Entity);
                if (!user.isAuthor && (!user.Id.Equals(updatedData.Author.Id)))
                {
                    return Unauthorized(new
                    {
                        Message = "You are not allowed to publish an article"
                    });
                }
                await _ctx.SaveChangesAsync();
                return Ok(updatedData);
            }
            return NotFound(new
            {
                Message = $"Article with Id={id} not found"
            });
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult> Insert(ArticleDto article)
        {
            var _user = HttpContext.User;
            User user = userService.GetUserWithClaim(_user);
            if (!user.isAuthor)
            {
                return Unauthorized(new
                {
                    Message = "You are not allowed to publish an article"
                });
            }
            var new_article = await _crud.Insert(new Article
            {
                Titre = article.Titre,
                Contenu = article.Contenu,
                Description = article.Description,
                Cover = article.Cover,
                CreatedAt = DateTime.Now,
                Author = user
            });
            return Ok(new
            {
                Titre = new_article.Titre,
                Contenu = new_article.Contenu,
                Description = article.Description,
                AuthorId = new_article.Id,
                CreatedAt = new_article.CreatedAt
            }
            );
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var success = await _crud.Delete(id);
            if (success)
            {
                return Ok(new
                {
                    Message = "Removed Successfully"
                });
            }
            return NotFound(new
            {
                Message = $"An error occured when trying to delete Article with Id={id}"
            });
        }
    }
}