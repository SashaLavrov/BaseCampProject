using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BCMyProject.Models;

namespace BCMyProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Coment> Coments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PhotoBoard>()
                .HasKey(t => new { t.BoardId, t.PhotoId });

            builder.Entity<UserRating>()
                .HasKey(t => new { t.UserId, t.RatingId });

            builder.Entity<PhotoBoard>()
                .HasOne(bp => bp.Board)
                .WithMany(b => b.PhotoBoard)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(bp => bp.BoardId);

            builder.Entity<PhotoBoard>()
                .HasOne(bp => bp.Photo)
                .WithMany(b => b.PhotoBoard)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(bp => bp.PhotoId);

            builder.Entity<Coment>()
                .HasOne(x => x.User)
                .WithMany(c => c.Coment)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Rating>()
                .Property(x => x.Value)
                .HasDefaultValue(0);
        }
    }
}
