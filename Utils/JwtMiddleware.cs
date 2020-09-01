using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeaApp.Models.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdeaApp.Utils
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        private readonly string _secret;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration,ILogger<JwtMiddleware> logger)
        {
            _next = next;
            
            _configuration=configuration;
            _secret=configuration["Jwt:Secret"];
            _logger=logger;
        }

        public async Task Invoke(HttpContext context, IUserRepository userRepo)
        {
            var token = context.Request.Headers["X-Access-Token"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await attachAccountToContext(context, userRepo, token);

            await _next(context);
        }

        private async Task attachAccountToContext(HttpContext context, IUserRepository userRepo, string token)
        {
            try
            {
                var userName=JwtUtils.GetUserIdFromAccessToken(token,_secret);
                context.Items["User"]=userRepo.GetByUserName(userName);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
            }
        }
    }
    
}