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
using GazetyBack.Services;

namespace GazetyBack.Services
{
    public class UserService
    {

        private DatabaseContext _ctx;
        private Crud<User> _crudUser;
        
        public Crud<User> Crud {get;}

        public UserService(DatabaseContext context)
        {
            _ctx = context;
            _crudUser = new Crud<User>(context);
            Crud = _crudUser;
        }

        public User GetUserWithDto(UserDto userInfo)
        {
            return _crudUser.GetRepos().Single(user => user.Username == userInfo.Username && user.Password == userInfo.Password);
        }
        
        public User GetUserWithClaim(ClaimsPrincipal _user)
        {
            User user = null;
            if (_user.HasClaim(c => c.Type.Equals("Username")))
            {
                var username = _user.Claims.Single(claim => claim.Type == "Username");
                user = _crudUser.GetRepos().Single<User>(u => u.Username == username.Value);
            }
            return user;
        }

        public string GenerateJwtToken(User user,IConfiguration configuration)
        {
            var key = configuration["JwtSettings:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creditentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim("Id",user.Id.ToString()),
                new Claim("Username",user.Username),
            };
            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creditentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public UserModel ToUserModel(User user)
        {

            return new UserModel
            {
                Id=user.Id,
                Username=user.Username,
                Articles=GetArticles(user),
                isAdmin = user.isAdmin,
                isAuthor = user.isAuthor,
                CreatedAt = user.CreatedAt
            };
        }

        /*
        public Boolean UpdateUser(long Id)
        {
            var user = _crudUser.GetRepos().Single<User>(user=>user.Id==Id);
            _ctx.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            user.isAdmin = !user.Is
            _ctx.Update<User>()
        }*/

        public UserData GetUserData(User user)
        {
            return new UserData
            {
                Username = user.Username,
                Id= user.Id
            };
        }

        public List<long> GetArticles(User user)
        {
            _ctx.Entry<User>(user)
                    .Collection<Article>(u => u.Articles)
                    .Load();
            var listArticles = new List<long>();
            user.Articles.ForEach(article =>
            {
                listArticles.Add(article.Id);
            });
            return listArticles;
        }

        public User GetUserInfo(UserDto userDto)
        {
            try
            {
                return _crudUser.GetRepos().Single(user => user.Username == userDto.Username && user.Password == userDto.Password);
            }
            catch (System.Exception)
            {

                return null;
            }
        }
    }
}