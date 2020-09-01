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


    [ApiController]
    [Route("access-tokens")]
    public class AccessTokensController : ControllerBase
    {

        IUserRepository _userRepo = new UserRepository(new IdeaDbContext());
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;

        private readonly IConfiguration _configuration;
        public AccessTokensController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;

        }


        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] Dictionary<string, string> data)
        {
            if (!data.ContainsKey("refresh_token"))
            {
                return BadRequest(
                    new
                    {
                        errors = new
                        {
                            refresh_token = "refresh_token is required"
                        }
                    }
                );
            }
            var refreshTokenObj = _userRepo.GetRefreshToken(data["refresh_token"]);//model.RefreshToken);

            if (DateTime.UtcNow > refreshTokenObj.Expiration)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Expired Token" });
            }

            var userRoles = await _userManager.GetRolesAsync(refreshTokenObj.User);
            List<Claim> authClaims = JwtUtils.GetClaims(refreshTokenObj.User, userRoles);

            var jwtSecret = _configuration["JWT:Secret"];
            var minutesExpiration = JwtUtils.MinutesExpiration;


            return Ok(new
            {
                jwt = JwtUtils.GenerateJwtToken(jwtSecret, authClaims, minutesExpiration),

            });

        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                List<Claim> authClaims = JwtUtils.GetClaims(user, userRoles);

                var jwtSecret = _configuration["JWT:Secret"];
                var minutesExpiration = JwtUtils.MinutesExpiration;
                user = _userRepo.GetById(user.Id);//load the user record againg

                return CreatedAtAction(
                    nameof(Login),
                    new
                    {
                        jwt = JwtUtils.GenerateJwtToken(jwtSecret, authClaims, minutesExpiration),
                        refresh_token = JwtUtils.GenerateRefreshToken(user, _userRepo).Token
                    });
            }
            return Unauthorized();
        }

        [HttpDelete]
        public async Task<IActionResult> LogOut([FromBody] Dictionary<string, string> data)
        {
            if (!data.ContainsKey("refresh_token"))
            {
                return BadRequest(
                    new
                    {
                        errors = new
                        {
                            refresh_token = "refresh_token is required"
                        }
                    }
                );
            }
            var refreshTokenObj = _userRepo.GetRefreshToken(data["refresh_token"]);//model.RefreshToken);

            if (DateTime.UtcNow > refreshTokenObj.Expiration)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Expired Token" });
            }

            _userRepo.ExpireToken(refreshTokenObj);
            
            return new NoContentResult();
            

        }



    }


}