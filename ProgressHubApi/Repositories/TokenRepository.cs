using System;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Models;
using ProgressHubApi.Models.Token;
using ProgressHubApi.Validators;

namespace ProgressHubApi.Repositories
{
    public interface ITokenRepository
    {
        public Task<(string?,BasicResultEnum)> AddRefreshTokenToUser(string email, string refreshToken, DateTime? expiryTime);
        public Task<(UserModel?,BasicResultEnum)> Refresh(string email, string refreshToken, TokenModel model);
    }

    public class TokenRepository : ITokenRepository
    {
        private IMongoCollection<UserModel> _accounts;
        private IMongoCollection<UserModel> _tempAccounts;

        public TokenRepository(IMongoClient client)
        {
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
            _tempAccounts = mongoDatabase.GetCollection<UserModel>("TempUsers");
        }

        public async Task<(string?,BasicResultEnum)> AddRefreshTokenToUser(string email, string refreshToken, DateTime? expiryTime)
        {
            try
            {
                var accounts = await _accounts.Find(_ => true).ToListAsync();
                var account = accounts.FirstOrDefault(n => n.Email == email);
                account.Tokens.RefreshToken = refreshToken;
                if(expiryTime != null)
                {
                    account.Tokens.RefreshTokenExpiryTime = expiryTime.Value;
                }

                await _accounts.FindOneAndReplaceAsync(x => x.Email.Equals(email), account);

                return (refreshToken, BasicResultEnum.Success);
            }
            catch (Exception e)
            {
                return (null, BasicResultEnum.Error);
            }
        }

        public async Task<(UserModel?, BasicResultEnum)> Refresh(string email, string refreshToken, TokenModel model)
        {
            try
            {
                var accounts = await _accounts.Find(_ => true).ToListAsync();
                var account = accounts.FirstOrDefault(n => n.Email == email);
                if (account is null || account.Tokens.RefreshToken != model.RefreshToken || account.Tokens.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return (null,BasicResultEnum.Forbidden);

                account.Tokens.RefreshToken = refreshToken;

                await _accounts.FindOneAndReplaceAsync(x => x.Email.Equals(email), account);

                return (account, BasicResultEnum.Success);
            }
            catch (Exception e)
            {
                return (null, BasicResultEnum.Error);
            }
        }
    }
}
