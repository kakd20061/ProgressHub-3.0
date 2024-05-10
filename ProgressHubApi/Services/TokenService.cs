using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using ProgressHubApi.Repositories;
using ProgressHubApi.Enums;
using ProgressHubApi.Models.Token;

namespace ProgressHubApi.Services
{
	public interface ITokenService
	{
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        public Task<(string?,BasicResultEnum)> AddRefreshTokenToUser(string email);
        public Task<(ExternalTokenModel?, BasicResultEnum)> Refresh(TokenModel model);
    }
    public class TokenService : ITokenService
	{
        private readonly ITokenRepository _repository;
        private readonly CommonService _commonService;
        private readonly JwtSettingsModel _jwtSettings;

        public TokenService(ITokenRepository repository, CommonService commonService, IOptions<JwtSettingsModel> jwtSettings)
        {
            _repository = repository;
            _commonService = commonService;
            _jwtSettings = jwtSettings.Value;
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
            TokenValidationParameters tokenValidationParameters;
            if(Environment.GetEnvironmentVariable("JWTSECRETKEY") != null)
            {
                tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTSECRETKEY"))),
                    ValidateLifetime = false
                };
            }else
            {
                tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                    ValidateLifetime = false
                };
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public async Task<(ExternalTokenModel?, BasicResultEnum)> Refresh(TokenModel model)
        {
            try
            {
                if (model == null)
                {
                    return (null, BasicResultEnum.Error);
                }
                var principal = GetPrincipalFromExpiredToken(model.AccessToken);
                var email = principal.Claims.FirstOrDefault(n=>n.Type == ClaimTypes.Email).Value;
                var refreshToken = GenerateRefreshToken();

                var result = await _repository.Refresh(email, refreshToken, model);

                if(result.Item2 == BasicResultEnum.Success)
                {
                    var newAccessToken = _commonService.GenerateJwt(result.Item1);
                    return (new ExternalTokenModel() {
                        AccessToken = newAccessToken,
                        RefreshToken = refreshToken,
                        HasPassword = result.Item1.Password != null
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

