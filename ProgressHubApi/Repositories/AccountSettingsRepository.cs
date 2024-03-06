using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.AccountSettings;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Services;
using ProgressHubApi.Validators;

namespace ProgressHubApi.Repositories;

public interface IAccountSettingsRepository
{
    public Task<(BasicResultEnum,ICollection<TagModel>?)> GetAllTags();
    public Task<BasicResultEnum> SaveTags(SaveTagsMogel model);
    public Task<BasicResultEnum> ChangePassword(ChangePasswordModelWithCurrentPassword model);
}

public class AccountSettingsRepository : IAccountSettingsRepository
    {
        private IMongoCollection<UserModel> _accounts;
        private IMongoCollection<TagModel> _tags;
        private IAccountSettingsValidator _validator;
        private readonly CommonService _commonService;

        public AccountSettingsRepository(IMongoClient client, IAccountSettingsValidator validator, CommonService commonService)
        {
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
            _tags = mongoDatabase.GetCollection<TagModel>("Tags");
            _validator = validator;
            _commonService = commonService;
        }

        public async Task<(BasicResultEnum, ICollection<TagModel>?)> GetAllTags()
        {
            try
            {
                var tags = await _tags.Find(_ => true).ToListAsync();
                return (BasicResultEnum.Success,tags);
            }
            catch (Exception e)
            {
                return (BasicResultEnum.Error,null);
            }
        }

        public async Task<BasicResultEnum> SaveTags(SaveTagsMogel model)
        {
            try
            {
                var findUser = await _accounts.FindAsync(x => x.Email == model.Email);
                var user = await findUser.FirstOrDefaultAsync();
                if (user == null)
                {
                    return BasicResultEnum.Error;
                }

                if (model.TagsIds.Length == 0 || model.TagsIds == null)
                {
                    user.Tags = new List<TagModel>();
                }
                else
                {
                    user.Tags = new List<TagModel>();
                    await _tags.Find(x => true).ForEachAsync(x =>
                    {
                        if (model.TagsIds.Contains(x._Id))
                        {
                            user.Tags.Add(x);
                        }
                    });
                    
                }
                await _accounts.ReplaceOneAsync(x => x.Email == model.Email, user);
                return BasicResultEnum.Success;
            }
            catch (Exception e)
            {
                return BasicResultEnum.Error;

            }
        }
        
        public async Task<BasicResultEnum> ChangePassword(ChangePasswordModelWithCurrentPassword model)
        {
            try
            {
                var findUser = await _accounts.FindAsync(x => x.Email == model.email);
                var user = await findUser.FirstOrDefaultAsync();
                if (user == null)
                {
                    return BasicResultEnum.Error;
                }
                if(user.Password == null && _validator.ValidateNewPassword(model.password))
                {
                    user.Password = _commonService.HashPassword(model.password);
                    await _accounts.ReplaceOneAsync(x => x.Email == model.email, user);
                    return BasicResultEnum.Success;
                }
                if (!_validator.ValidatePasswords(model.password, model.currentPassword, user))
                {
                    return BasicResultEnum.Error;
                }
                user.Password = _commonService.HashPassword(model.password);
                await _accounts.ReplaceOneAsync(x => x.Email == model.email, user);
                return BasicResultEnum.Success;
            }
            catch (Exception e)
            {
                return BasicResultEnum.Error;
            }
        }
    }