using System.ComponentModel.DataAnnotations;

namespace IdeaApp.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class RefreshModel
    {
        [Required(ErrorMessage = "Refresh Token is required")]
        public string RefreshToken { get; set; }

    }

    

}