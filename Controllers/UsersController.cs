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
using GazetyBack.Services;

namespace GazetyBack.Controllers
{
    [ApiController]
    [Route("Users")]
    public class UsersController : ControllerBase
    {

        private UserService userService;
        private DatabaseContext _ctx;
        public IConfiguration Configuration { get; }

        private Crud<User> _crudUser;

        public UsersController(DatabaseContext context, IConfiguration config)
        {
            _ctx = context;
            userService = new UserService(context);
            _crudUser = userService.Crud;
            Configuration = config;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            var userList = _crudUser.GetRepos().ToList();
            List<UserModel> listUser = new List<UserModel>();
            userList.ForEach(user =>
            {
                var listArticles = userService.GetArticles(user);
                listUser.Add(userService.ToUserModel(user));
            });
            return Ok(listUser);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetOne(long id)
        {
            var data = await _crudUser.GetOne(id);
            if (data != null)
            {
                return Ok(userService.ToUserModel(data));
            }
            return NotFound(new
            {
                Message = $"Article with Id={id} not found"
            });
        }



        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult Login(UserDto _user)
        {
            User user = null;
            try
            {
                
                user = userService.GetUserInfo(_user);
            }
            catch (System.Exception)
            {}
            if (user != null)
            {
                return Ok(
                    new
                    {
                        AccessToken = userService.GenerateJwtToken(user, Configuration),
                        Id = user.Id
                    }
                );
            }
            return Unauthorized(new
            {
                Message = $"Les informations fournies sont erronées"
            });
        }


        [HttpPost("create")]
        public async Task<ActionResult> Insert(UserDto user)
        {
            var _user = userService.Crud.GetRepos().Where<User>(u => u.Username == user.Username).FirstOrDefault();
            if (_user != null)
            {
                return Unauthorized(new
                {
                    Message = "An user with your username already exists!"
                });
            }
            var new_user = await _crudUser.Insert(new User
            {
                Username = user.Username,
                Password = user.Password,
                CreatedAt = DateTime.Now,
                isAdmin = false,
                isAuthor = false
            });
            return Ok(userService.ToUserModel(new_user));
        }

        [HttpPut("setAuthor/{id}")]
        [Authorize]
        public async Task<ActionResult> SetAsAuthor(long Id)
        {
            var _user = HttpContext.User;
            User admin = userService.GetUserWithClaim(_user);
            var user = await _crudUser.GetOne(Id);
            if(!user.isAdmin)
            {
                return Unauthorized(new {
                    Message="Vous n'êtes pas autoriser à effectuer cette action"
                });
            }
            user.isAuthor = !user.isAuthor;
            var trackUser = _crudUser.GetRepos().Update(user);
            _ctx.SaveChanges();
            return Ok(userService.ToUserModel(user));
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var success = await _crudUser.Delete(id);
            if (success)
            {
                return Ok(new
                {
                    Message = "Removed Successfully"
                });
            }
            return NotFound(new
            {
                Message = $"An error occured when trying to delete User with Id={id}"
            });
        }
    }
}