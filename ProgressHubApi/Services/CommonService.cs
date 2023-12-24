using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProgressHubApi.Models;

namespace ProgressHubApi.Services
{
	public class CommonService
	{

        public static string? HashPassword(string password)
        {
            string? result;
            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                result = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }
            return result;
        }

        public static int GenerateVerificationCode()
        {
            Random random = new Random();
            int result = random.Next(1000, 10000);
            return result;
        }

        public static string GenerateJwt(UserModel model)
        {
            //todo: move this to appsettings.json and .env
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, model.Nickname),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Name, model.Name),
                new Claim(ClaimTypes.Surname, model.LastName),
            };
            //todo: add roles
            //todo: check what is issuer and audience
            var tokeOptions = new JwtSecurityToken(
                issuer: "https://localhost:7034",
                audience: "https://localhost:7034",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }
    }
}

