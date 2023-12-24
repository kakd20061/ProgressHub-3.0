using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using ProgressHubApi.Repositories;
using ProgressHubApi.Enums;
using ProgressHubApi.Models;

namespace ProgressHubApi.Services
{
	public interface ITokenService
	{
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        public Task<(string?,BasicResultEnum)> AddRefreshTokenToUser(string email);
        public Task<(TokenModel?, BasicResultEnum)> Refresh(TokenModel model);
    }
    public class TokenService : ITokenService
	{
        private readonly ITokenRepository _repository;

        public TokenService(ITokenRepository repository)
        {
            _repository = repository;
        }

        public async Task<(string?, BasicResultEnum)> AddRefreshTokenToUser(string email)
        {
            var refreshToken = GenerateRefreshToken();
            var result = await _repository.AddRefreshTokenToUser(email, refreshToken, DateTime.UtcNow.AddDays(7));
            return result;
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
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345")),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public async Task<(TokenModel?, BasicResultEnum)> Refresh(TokenModel model)
        {
            try
            {
                if (model == null)
                {
                    return (null, BasicResultEnum.Error);
                }
                var principal = GetPrincipalFromExpiredToken(model.AccessToken);
                var username = principal.Claims.FirstOrDefault(n=>n.Type == ClaimTypes.NameIdentifier).Value;
                var refreshToken = GenerateRefreshToken();

                var result = await _repository.Refresh(username, refreshToken, model);

                if(result.Item2 == BasicResultEnum.Success)
                {
                    var newAccessToken = CommonService.GenerateJwt(result.Item1);
                    return (new TokenModel() {
                        AccessToken = newAccessToken,
                        RefreshToken = refreshToken
                    }, BasicResultEnum.Success);
                }
                return (null, result.Item2);
            }catch(Exception e)
            {
                return (null, BasicResultEnum.Error);
            }
        }
    }
}

