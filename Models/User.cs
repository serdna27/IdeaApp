using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IdeaApp.Models
{

    public class User : IdentityUser<int>
    {

        public string AvatarUrl { get; set; }
        public List<RefreshToken> Tokens { get; set; }
        //You can extend this class by adding addtional properties about the user
    }


}