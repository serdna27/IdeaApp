using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdeaApp.Models;
using IdeaApp.Models.Repo;
using IdeaApp.Models.Repo.Base;
using Microsoft.IdentityModel.Tokens;

namespace IdeaApp.Utils
{


    public class JwtUtils{

        public static readonly int MinutesExpiration=60*2;

        public static List<Claim> GetClaims(User user, IList<string> userRoles)
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



        public static string GenerateJwtToken(string appSecret, IEnumerable<Claim> claims, double expirationInMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSecret);

            var claimsDictionary = new Dictionary<string, object>();

            foreach (var claim in claims)
            {
                claimsDictionary.Add(claim.Type, claim.Value);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Claims = claimsDictionary,
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        public static RefreshToken GenerateRefreshToken(User user, IUserRepository userRepo)
        {
            // Create the refresh token
            var refreshToken = new RefreshToken()
            {
                Token = GenerateRefreshToken(),
                Expiration = DateTime.UtcNow.AddMinutes(35),//35 mins
                UserId=user.Id,
                User=user
            };

            return userRepo.SaveRefreshToken(user,refreshToken);
        }

        public static string GetUserIdFromAccessToken(string accessToken,string appSecret)
        {
            var tokenValidationParamters = new TokenValidationParameters
            {
                ValidateAudience = false, 
                ValidateIssuer = false,
                ValidateActor = false,
                ValidateLifetime = false, 
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(appSecret)
                    )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParamters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token!");
            }

            var userId = principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new SecurityTokenException($"Missing claim: {ClaimTypes.Name}!");
            }

            return userId;
        }

    }



}