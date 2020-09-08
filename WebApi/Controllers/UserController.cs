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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace IdeaApp.Controllers
{

    [Route("users")]
    [ApiController]
    // [EnableCors("builder.AllowAnyOrigin")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration,ILogger<UserController> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;

            _userRepo = new UserRepository(new IdeaDbContext());
        }

       
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User already exists!" });

            var user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                FullName=model.Name,
                GravatarImageUrl=GravatarUtil.GetImageHash(model.Email)
                
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded){
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                _logger.LogCritical(errors);
                
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = errors });
                
            }

            var userRoles = await userManager.GetRolesAsync(user);
            List<Claim> authClaims = JwtUtils.GetClaims(user, userRoles);

            var jwtSecret = _configuration["Jwt:Secret"];
            _logger.LogInformation($"mmg look ==>{jwtSecret}");
            var minutesExpiration = 60 * 2;

            user = _userRepo.GetById(user.Id);//load the user record againg
            return CreatedAtAction(
                nameof(Register),
                new
            {
                jwt = JwtUtils.GenerateJwtToken(jwtSecret, authClaims, minutesExpiration),
                refresh_token = JwtUtils.GenerateRefreshToken(user,_userRepo).Token
            });

        }

    }

}