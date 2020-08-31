using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdeaApp.Models
{
    public class IdeaDbContext: IdentityDbContext<User>{

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
                  => options.UseSqlite(@"Data Source=app.db");

      
    }

    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

}