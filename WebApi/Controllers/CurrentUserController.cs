using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        IUserRepository _userRepo;

        private readonly IConfiguration _configuration;

        public CurrentUserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _logger = logger;
            _configuration=configuration;
            _userRepo= new UserRepository(new IdeaDbContext(),logger);
        }
        private readonly ILogger<UserController> _logger;
        
        public IActionResult GetCurrentUser()
        {
            if (!this.Request.Headers.ContainsKey("X-Access-Token"))
                return Unauthorized();

            var accessToken = this.Request.Headers["X-Access-Token"];

            _logger.LogInformation($"accessToken==>{accessToken}");
            
            
            RefreshToken refreshTokenObj =null;
            try
            {
                var jwtSecret = _configuration["Jwt:Secret"];
                var userName = JwtUtils.GetUserIdFromAccessToken(accessToken, jwtSecret);

                _logger.LogInformation($"user id ==>{userName}");
                var user = _userRepo.GetByUserName(userName);
                _logger.LogInformation($"user ==>{user.Id}");
                refreshTokenObj = user.Tokens.FirstOrDefault(tk=>DateTime.UtcNow < tk.Expiration); //get the first valid refresh token

            }
            catch (System.Exception ex)
            {
                _logger.LogCritical(ex,ex.Message);
                // log error here
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Invalid Token" });
            }
            

            if (refreshTokenObj == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Invalid Token" });
            }

            return Ok(new
            {
                email = refreshTokenObj.User.Email,
                name = refreshTokenObj.User.FullName,
                avatar_url = refreshTokenObj.User.GravatarImageUrl
            });

        }


    }


}