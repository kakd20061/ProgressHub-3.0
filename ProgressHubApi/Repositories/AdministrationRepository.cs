using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Models;
using ProgressHubApi.Models.AccountSettings;
using ProgressHubApi.Models.Administration;
using ProgressHubApi.Services;
using ProgressHubApi.Validators;

namespace ProgressHubApi.Repositories;

public interface IAdministrationRepository
{
    public Task<(BasicResultEnum, ICollection<UserModel>?)> GetAllUsers(string token);
    public Task<(BasicResultEnum, ICollection<TagModel>?)> GetAllTags(string token);
    public Task<BasicResultEnum> AddTag(AddTagModel model);
    
    public Task<BasicResultEnum> RemoveTag(RemoveTagModel model);
    
    public Task<BasicResultEnum> UpdateTag(UpdateTagModel model);
    
    public Task<BasicResultEnum> ChangeUserRole(ChangeUserRoleModel model);
    
    public Task<BasicResultEnum> BlockUser(BlockUserModel model);
    
    public Task<BasicResultEnum> UnblockUser(UnblockUserModel modelsa);
}

public class AdministrationRepository : IAdministrationRepository
{
    private IMongoCollection<UserModel> _accounts;
    private IMongoCollection<TagModel> _tags;
    private IAdministrationValidator _validator;
    
    public AdministrationRepository(IMongoClient client, IAdministrationValidator validator)
    {
        _validator = validator;
        var mongoDatabase = client.GetDatabase("ProgressHub");

        _accounts = mongoDatabase.GetCollection<UserModel>("Users");
        _tags = mongoDatabase.GetCollection<TagModel>("Tags");
        
    }

    public async Task<(BasicResultEnum, ICollection<UserModel>?)> GetAllUsers(string token)
    {
        try
        {
            var tokenResult = await _validator.ValidateToken(token);
            if (tokenResult != BasicResultEnum.Success)
            {
                return (BasicResultEnum.Blocked,null);
            }
            var users = await _accounts.Find(n => true).ToListAsync();
            return (BasicResultEnum.Success, users);
        }catch(Exception e)
        {
            return (BasicResultEnum.Error, null);
        }
    }
    
    public async Task<(BasicResultEnum, ICollection<TagModel>?)> GetAllTags(string token)
    {
        try
        {
            var tokenResult = await _validator.ValidateToken(token);
            if (tokenResult != BasicResultEnum.Success)
            {
                return (BasicResultEnum.Blocked, null);
            }
            var tags = await _tags.Find(n => true).ToListAsync();
            return (BasicResultEnum.Success, tags);
        }catch(Exception e)
        {
            return (BasicResultEnum.Error, null);
        }
    }

    public async Task<BasicResultEnum> AddTag(AddTagModel model)
    {
        try
        {
            var tokenResult = await _validator.ValidateToken(model.Token);
            if (tokenResult != BasicResultEnum.Success)
            {
                return BasicResultEnum.Blocked;
            }
            var tag = new TagModel
            {
                Name = model.Name
            };
            
            var result = await _validator.ValidateTag(tag);
            if (result != BasicResultEnum.Success)
                return result;
            
            await _tags.InsertOneAsync(tag);
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
    
    public async Task<BasicResultEnum> RemoveTag(RemoveTagModel model)
    {
        try
        {
            var tokenResult = await _validator.ValidateToken(model.Token);
            if (tokenResult != BasicResultEnum.Success)
            {
                return BasicResultEnum.Blocked;
            }
            var result = await _tags.DeleteOneAsync(n => n.Name == model.Name);
            if (result.DeletedCount == 0)
                return BasicResultEnum.Error;
            
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
    
    public async Task<BasicResultEnum> UpdateTag(UpdateTagModel model)
    {
        try
        {
            var tokenResult = await _validator.ValidateToken(model.token);
            if (tokenResult != BasicResultEnum.Success)
            {
                return BasicResultEnum.Blocked;
            }
            if(String.IsNullOrWhiteSpace(model.newName))
                return BasicResultEnum.Error;
            var result = await _tags.UpdateOneAsync(n => n.Name == model.oldName, Builders<TagModel>.Update.Set(n => n.Name, model.newName));
            
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }

    public async Task<BasicResultEnum> ChangeUserRole(ChangeUserRoleModel model)
    {
        try
        {
            var tokenResult = await _validator.ValidateTokenForUserManagement(model.token, model.email);
            if (tokenResult != BasicResultEnum.Success)
            {
                return BasicResultEnum.Blocked;
            }
            
            var user = await _accounts.Find(n => n.Email == model.email).FirstOrDefaultAsync();
            if (user == null)
                return BasicResultEnum.Error;
            if (user.Role == UserRoleEnum.Owner)
                return BasicResultEnum.Error;
            user.Role = Enum.Parse<UserRoleEnum>(model.role);
            await _accounts.ReplaceOneAsync(n => n.Email == model.email, user);
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }

    public async Task<BasicResultEnum> BlockUser(BlockUserModel model)
    {
        try
        {
            var tokenResult = await _validator.ValidateTokenForUserManagement(model.token, model.email);
            if (tokenResult != BasicResultEnum.Success)
            {
                return BasicResultEnum.Blocked;
            }
            var user = await _accounts.Find(n => n.Email == model.email).FirstOrDefaultAsync();
            if (user == null)
                return BasicResultEnum.Error;
            if (user.Role == UserRoleEnum.Owner)
                return BasicResultEnum.Error;
            if(model.blockExpirationDate != null && model.blockExpirationDate < DateTime.UtcNow)
                return BasicResultEnum.Error;
            if(model.blockExpirationDate == null)
                user.BanExpirationDate = DateTime.UtcNow.AddYears(100);
            else
                user.BanExpirationDate = model.blockExpirationDate?.AddDays(1);
            
            await _accounts.ReplaceOneAsync(n => n.Email == model.email, user);
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
    
    public async Task<BasicResultEnum> UnblockUser(UnblockUserModel model)
    {
        try
        {
            var tokenResult = await _validator.ValidateTokenForUserManagement(model.token, model.email);
            if (tokenResult != BasicResultEnum.Success)
            {
                return BasicResultEnum.Blocked;
            }
            var user = await _accounts.Find(n => n.Email == model.email).FirstOrDefaultAsync();
            if (user == null)
                return BasicResultEnum.Error;
            if (user.Role == UserRoleEnum.Owner)
                return BasicResultEnum.Error;
            user.BanExpirationDate = null;
            
            await _accounts.ReplaceOneAsync(n => n.Email == model.email, user);
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
}