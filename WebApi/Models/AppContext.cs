using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdeaApp.Models
{
    public class IdeaDbContext: IdentityDbContext<User,IdentityRole<int>,int>{

        // public DbSet<UserProfile> UsersProfile { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public IdeaDbContext()
        {
            
        }
        public IdeaDbContext(DbContextOptions<IdeaDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
                  => options.UseNpgsql("Host=localhost;port=5432;Database=idea_app_db;Username=postgres;Password=qwerty;").EnableSensitiveDataLogging();
                //   options.UseSqlite(@"Data Source=appIdea.db");

      
    }

    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

}