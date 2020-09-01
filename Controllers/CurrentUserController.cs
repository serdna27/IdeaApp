using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdeaApp.Models;
using IdeaApp.Models.Repo;
using IdeaApp.Utils;
using IdeaApp.ViewModels;
using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;


namespace IdeaApp.Controllers
{

    [Route("me")]
    [ApiController]
    public class CurrentUserController : ControllerBase
    {

        IUserRepository _userRepo = new UserRepository(new IdeaDbContext());
        private readonly ILogger<UserController> _logger;

        public CurrentUserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _logger = logger;
        }
        public IActionResult GetCurrentUser()
        {
            if (!this.Request.Headers.ContainsKey("X-Access-Token"))
                return Unauthorized();

            var accessToken = this.Request.Headers["X-Access-Token"];

            _logger.LogInformation($"accessToken==>{accessToken}");
            // accessToken="WtDre5dY/X6z+AV0mqtdn7wt0dWNqRDVKHW5e1wQ2ZQ=";

            var refreshTokenObj = _userRepo.GetRefreshToken(accessToken.ToString());
            // "WtDre5dY/X6z+AV0mqtdn7wt0dWNqRDVKHW5e1wQ2ZQ=");//$"'{accessToken}'");

            if (refreshTokenObj == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Invalid Token" });
            }

            if (DateTime.UtcNow > refreshTokenObj.Expiration)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Expired Token" });
            }

            return Ok(new
            {
                email = refreshTokenObj.User.Email,
                name = refreshTokenObj.User.FullName,
                avatar_url = ""//to update
            });

        }


    }


}