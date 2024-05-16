using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Models;
using ProgressHubApi.Models.AccountSettings;
using ProgressHubApi.Services;
using ProgressHubApi.Validators;

namespace ProgressHubApi.Repositories;

public interface IAdministrationRepository
{
    public Task<(BasicResultEnum, ICollection<UserModel>?)> GetAllUsers();
    public Task<(BasicResultEnum, ICollection<TagModel>?)> GetAllTags();
    public Task<BasicResultEnum> AddTag(string name);
    
    public Task<BasicResultEnum> RemoveTag(string name);
    
    public Task<BasicResultEnum> UpdateTag(string oldName, string newName);
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

    public async Task<(BasicResultEnum, ICollection<UserModel>?)> GetAllUsers()
    {
        try
        {
            var users = await _accounts.Find(n => true).ToListAsync();
            return (BasicResultEnum.Success, users);
        }catch(Exception e)
        {
            return (BasicResultEnum.Error, null);
        }
    }
    
    public async Task<(BasicResultEnum, ICollection<TagModel>?)> GetAllTags()
    {
        try
        {
            var tags = await _tags.Find(n => true).ToListAsync();
            return (BasicResultEnum.Success, tags);
        }catch(Exception e)
        {
            return (BasicResultEnum.Error, null);
        }
    }

    public async Task<BasicResultEnum> AddTag(string name)
    {
        try
        {
            var task = new TagModel
            {
                Name = name
            };
            
            var result = await _validator.ValidateTag(task);
            if (result != BasicResultEnum.Success)
                return result;
            
            await _tags.InsertOneAsync(task);
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
    
    public async Task<BasicResultEnum> RemoveTag(string name)
    {
        try
        {
            var result = await _tags.DeleteOneAsync(n => n.Name == name);
            if (result.DeletedCount == 0)
                return BasicResultEnum.Error;
            
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
    
    public async Task<BasicResultEnum> UpdateTag(string oldName, string newName)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(newName))
                return BasicResultEnum.Error;
            var result = await _tags.UpdateOneAsync(n => n.Name == oldName, Builders<TagModel>.Update.Set(n => n.Name, newName));
            
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
}