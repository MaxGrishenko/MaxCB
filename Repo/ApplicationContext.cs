using Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Repo
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>().HasMany(q => q.Ingredients).WithOne(w => w.Recipe).HasForeignKey(e => e.RecipeId);
            modelBuilder.Entity<Recipe>().HasMany(q => q.Methods).WithOne(w => w.Recipe).HasForeignKey(e => e.RecipeId);
            modelBuilder.Entity<Recipe>().HasMany(q => q.Tips).WithOne(w => w.Recipe).HasForeignKey(e => e.RecipeId);
        }
    }
}
