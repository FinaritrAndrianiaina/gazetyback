using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace GazetyBack.Models
{
    public class UserDto
    {
        public string Username { get; set; }

        public string Password { get; set; }

    }

    public class UserData
    {
        public long Id { get; set; }
        public string Username { get; set; }

    }

    public class UserModel
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual List<long> Articles { get; set; }

        public bool isAdmin { get; set; }

        public bool isAuthor { get; set; }
    }

    public class UserList
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool isAdmin { get; set; }

        public bool isAuthor { get; set; }
    }

    public class User : IEntity
    {
        [Key]
        public long Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual List<Article> Articles { get; set; } = new List<Article>();

        public bool isAdmin { get; set; }

        public bool isAuthor { get; set; }

    }
}