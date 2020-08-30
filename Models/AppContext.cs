using System;
using Microsoft.EntityFrameworkCore;

namespace IdeaApp.Models
{
    public class IdeaDbContext: DbContext{

        public DbSet<UserProfile> UsersProfile { get; set; }
        public DbSet<Idea> Ideas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
                  => options.UseSqlite("Data Source=app.db");

      
    }

    public class UserProfile
    {
        public int Id {get; set;}
        public string Email { get; set; }
        public string FullName { get; set; }

        public string AvatarUrl { get; set; }
    }
    
    public class Idea
    {
        public int Id { get; set; }
        
        public string Content { get; set; }

        public int Impact { get; set; }

        public int Ease { get; set; }

        public int Confidence { get; set; }

        public DateTime CreationDate { get; set; }
        
        public DateTime? ModificationDate { get; set; }
     
    
    }
}