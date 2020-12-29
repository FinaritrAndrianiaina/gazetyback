using System;
using System.ComponentModel.DataAnnotations;


namespace GazetyBack.Models
{
    public class ArticleDto
    {
        public string Titre { get; set; }
        public string Contenu { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
    }

    public class ArticleModel
    {
        public long Id { get; set; }

        public string Titre { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
        public string Contenu { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserData Author { get; set; }

    }

    public class Article : IEntity
    {
        [Key]
        public long Id { get; set; }

        public string Titre { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
        public string Contenu { get; set; }

        public DateTime CreatedAt { get; set; }

        public User Author { get; set; }
        public long AuthorId {get;set;}

    }
}