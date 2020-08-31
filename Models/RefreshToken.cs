using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeaApp.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User{ get; set; }
    }

}