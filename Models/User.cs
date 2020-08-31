using Microsoft.AspNetCore.Identity;

namespace IdeaApp.Models
{

    public class User : IdentityUser
    {

        public string AvatarUrl { get; set; }
        //You can extend this class by adding addtional properties about the user
    }


}