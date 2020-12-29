using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using GazetyBack.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using GazetyBack.Utils;
using GazetyBack.Services;

namespace GazetyBack.Controllers
{
    public class FileDto
    {
        public string Title {get;set;}
        public IFormFile image {get;set;}
    }


    [ApiController]
    [Route("file")]
    public class FileController : ControllerBase
    {
        private DatabaseContext _ctx;
        private UserService userService;
        public FileController(DatabaseContext context)
        {
            _ctx = context;
            userService = new UserService(context);
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            return Ok(_ctx.Images.ToList<Image>());
        }

        [HttpPost("upload")]
        //[Authorize]
        public async Task<ActionResult> PostFile(IFormFile image)
        {
            /*var _user = HttpContext.User;
            User user = userService.GetUserWithClaim(_user);
            if (!user.isAuthor)
            {
                return Unauthorized(new
                {
                    Message = "You are not allowed to upload any image"
                });
            }*/
            var fileId = Guid.NewGuid().ToString();
            var fileName = new String(fileId.Replace('-','_')+"_" + image.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),"uploads",fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await image.CopyToAsync(stream);
            }
            var new_image = new Image
            {
                Id = fileId,
                Title = fileId,
                Filename = image.FileName,
                Size = image.Length,
                Path = Path.Combine("uploads", fileName)
            };
            var insertion = await _ctx.Images.AddAsync(new_image);
            await _ctx.SaveChangesAsync();
            return Ok(insertion.Entity);
        }
    }
}