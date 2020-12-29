using System;
using System.ComponentModel.DataAnnotations;

namespace GazetyBack.Models
{
    public class Image
    {
        [Key]
        public string Id { get; set; }

        public string Title {get;set;}
        
        public string Filename { get; set; }

        public long Size {get;set;}

        public string Path { get; set; }
    }
}