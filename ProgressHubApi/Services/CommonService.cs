using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProgressHubApi.Enums;
using ProgressHubApi.Models;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Models.Token;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ProgressHubApi.Services
{
	public class CommonService
	{
        private readonly JwtSettingsModel _jwtSettings;

        public CommonService(IOptions<JwtSettingsModel> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
        public string? HashPassword(string password)
        {
            var sourceBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = SHA512.HashData(sourceBytes);
            var result = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            return result;
        }

        public BasicResultEnum UploadFile(IFormFile file, string path,string fileName, string previousFileName)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    if (previousFileName != "" && File.Exists(path+"/"+previousFileName))
                    {
                        File.Delete(path+"/"+previousFileName);
                    }
                    using (var image = Image.Load(file.OpenReadStream()))
                    {
                        var ratio = 400 / (double)image.Height;
                        var targetWidth = (int)(image.Width * ratio);
                        if(targetWidth < 200) targetWidth = 200;
                        image.Mutate(n=>n.Resize(targetWidth,400).Crop(new Rectangle((image.Width - 200) / 2, (image.Height -200) / 2,200,200)));

                        image.Save(path+"/"+fileName);
                    }
                    return BasicResultEnum.Success;
                }
                return BasicResultEnum.Error;
            }
            catch (Exception e)
            {
                return BasicResultEnum.Error;
            }
        }
        
        public BasicResultEnum DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return BasicResultEnum.Success;
                }
                return BasicResultEnum.Error;
            }
            catch (Exception e)
            {
                return BasicResultEnum.Error;
            }
        }
        
        public int GenerateVerificationCode()
        {
            Random random = new Random();
            int result = random.Next(1000, 10000);
            return result;
        }

        public string GenerateJwt(UserModel model)
        {
            var genderNum = (int)model.Gender;
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, model.Nickname),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Name, model.Name),
                new Claim(ClaimTypes.Surname, model.LastName),
                new Claim(ClaimTypes.Role, Enum.GetName(model.Role)),
                new Claim("Tags", JsonConvert.SerializeObject(model.Tags, Formatting.None)),
                new Claim("Avatar", model.Avatar ?? ""),
                new Claim(ClaimTypes.DateOfBirth, model.DateOfBirth?.ToString("yyyy-MM-dd") ?? ""),
                new Claim(ClaimTypes.Gender, genderNum.ToString()),
                new Claim("Weight", model.BodyParameters.Weight),
                new Claim("Height", model.BodyParameters.Height),
                new Claim("BodyFatPercentage", model.BodyParameters.BodyFatPercentage),
            };
            
            JwtSecurityToken tokeOptions;

            
            if(Environment.GetEnvironmentVariable("JWTSECRETKEY") != null && Environment.GetEnvironmentVariable("JWTISSUER") != null && Environment.GetEnvironmentVariable("JWTAUDIENCE") != null)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTSECRETKEY")));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                
                tokeOptions = new JwtSecurityToken(
                    issuer: Environment.GetEnvironmentVariable("JWTISSUER"),
                    audience: Environment.GetEnvironmentVariable("JWTAUDIENCE"),
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );
            }else
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                
                tokeOptions = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );
            }
            
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }
    }
}

