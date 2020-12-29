using Microsoft.EntityFrameworkCore;
using GazetyBack.Models;

namespace GazetyBack
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions option):base(option)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<Article>(user=>user.Articles)
                .WithOne(article=>article.Author)
                .HasForeignKey(article=>article.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #region DbSets
        public DbSet<Article> Articles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Image> Images { get; set; }
        #endregion

    }
}