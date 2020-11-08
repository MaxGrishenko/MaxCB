using Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Repo
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Post <-> ApplicationUser [Many to Many]
            modelBuilder.Entity<PostUser>().HasKey(x => new { x.PostId, x.UserId });
            modelBuilder.Entity<PostUser>().HasOne(q => q.User).WithMany(w => w.Posts).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PostUser>().HasOne(p => p.Post).WithMany(p => p.Users).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Restrict);

            // Report <-> ApplicationUser [Many to Many]
            modelBuilder.Entity<ReportUser>().HasKey(x => new { x.ReportId, x.UserId });
            modelBuilder.Entity<ReportUser>().HasOne(q => q.User).WithMany(w => w.Reports).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReportUser>().HasOne(p => p.Report).WithMany(p => p.Users).HasForeignKey(x => x.ReportId).OnDelete(DeleteBehavior.Restrict);

            // Post -> Recipe [One to One]
            modelBuilder.Entity<Post>().HasOne(q => q.Recipe).WithOne(w => w.Post).HasForeignKey<Post>(x => x.RecipeId);

            // Post <- Rating/Comment [One to Many]
            modelBuilder.Entity<Post>().HasMany(q => q.Ratings).WithOne(w => w.Post).HasForeignKey(x => x.PostId);
            modelBuilder.Entity<Post>().HasMany(q => q.Comments).WithOne(w => w.Post).HasForeignKey(x => x.PostId);

            // User <- Rating/Comment [One to Many]
            modelBuilder.Entity<ApplicationUser>().HasMany(q => q.Ratings).WithOne(w => w.User).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<ApplicationUser>().HasMany(q => q.Comments).WithOne(w => w.User).HasForeignKey(x => x.UserId);

            // Recipe -> User [One to One]
            modelBuilder.Entity<Recipe>().HasOne(q => q.User).WithMany(w => w.Recipes).HasForeignKey(x => x.UserId);

            // Recipe <- Ingredient/Method/Tip [One to Many]
            modelBuilder.Entity<Recipe>().HasMany(q => q.Ingredients).WithOne(w => w.Recipe).HasForeignKey(e => e.RecipeId);
            modelBuilder.Entity<Recipe>().HasMany(q => q.Methods).WithOne(w => w.Recipe).HasForeignKey(e => e.RecipeId);
            modelBuilder.Entity<Recipe>().HasMany(q => q.Tips).WithOne(w => w.Recipe).HasForeignKey(e => e.RecipeId);

        }
    }
}
