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

        private List<Claim> CreateClaims(string id)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, id));

            return claims;
        }

        public string GenerateJwtToken(string appSecret, IEnumerable<Claim> claims, double expirationInMinutes)
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

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        public RefreshToken GenerateRefreshToken(User user, IUserRepository userRepo)
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

    }



}