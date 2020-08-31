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
    public class CurrentUserController:ControllerBase{

        IUserRepository _userRepo = new UserRepository(new IdeaDbContext());
        private readonly ILogger<UserController> _logger;

        public CurrentUserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _logger=logger;
        }
        public IActionResult GetCurrentUser()
        {   
            if (!this.Request.Headers.ContainsKey("X-Access-Token"))
                return Unauthorized();
            
            var accessToken = this.Request.Headers["X-Access-Token"];

            _logger.LogInformation($"accessToken==>{accessToken}");
            // accessToken="WtDre5dY/X6z+AV0mqtdn7wt0dWNqRDVKHW5e1wQ2ZQ=";

            var refreshTokenObj=_userRepo.GetRefreshToken(accessToken.ToString());
                // "WtDre5dY/X6z+AV0mqtdn7wt0dWNqRDVKHW5e1wQ2ZQ=");//$"'{accessToken}'");

            if(refreshTokenObj==null){
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Invalid Token" });
            }

            if (DateTime.UtcNow > refreshTokenObj.Expiration)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Expired Token" });
            }
            
            return Ok(new {
                email=refreshTokenObj.User.Email,
                name=refreshTokenObj.User.FullName,
                avatar_url=""//to update
            });
            
        }


    }
    [Route("users")]
    [ApiController]
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
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                List<Claim> authClaims = GetClaims(user, userRoles);

                var jwtSecret = _configuration["JWT:Secret"];
                var minutesExpiration = 60 * 2;
                user = _userRepo.GetById(user.Id);//load the user record againg

                return Ok(new
                {
                    jwt = JwtUtils.GenerateJwtToken(jwtSecret, authClaims, minutesExpiration),
                    refresh_token = JwtUtils.GenerateRefreshToken(user, _userRepo)
                });
            }
            return Unauthorized();
        }

        private static List<Claim> GetClaims(User user, IList<string> userRoles)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name.ToString(), user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return authClaims;
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
                FullName=model.Name
                
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            var userRoles = await userManager.GetRolesAsync(user);
            List<Claim> authClaims = GetClaims(user, userRoles);

            var jwtSecret = _configuration["Jwt:Secret"];
            _logger.LogInformation($"mmg look ==>{jwtSecret}");
            var minutesExpiration = 60 * 2;

            user = _userRepo.GetById(user.Id);//load the user record againg
            return Ok(new
            {
                jwt = JwtUtils.GenerateJwtToken(jwtSecret, authClaims, minutesExpiration),
                refresh_token = JwtUtils.GenerateRefreshToken(user,_userRepo).Token
            });

        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User already exists!" });

            var user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole<int>(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new  { Status = "Success", Message = "User created successfully!" });
        }
    }

}