using System;
using System.ComponentModel.DataAnnotations;

namespace GazetyBack.Models
{
    public interface IEntity
    {
        long Id { get; set; }

        DateTime CreatedAt { get; set; }
    }
}