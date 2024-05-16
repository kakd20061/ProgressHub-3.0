using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.AccountSettings;
using ProgressHubApi.Models;
using ProgressHubApi.Models.AccountSettings;
using ProgressHubApi.Services;

namespace ProgressHubApi.Validators;

public interface IAdministrationValidator
{
    public Task<BasicResultEnum> ValidateTag(TagModel tag);
}

public class AdministrationValidator : IAdministrationValidator
{
    private IMongoCollection<UserModel> _accounts;
    private IMongoCollection<TagModel> _tags;

    public AdministrationValidator(IMongoClient client)
    {
        var mongoDatabase = client.GetDatabase("ProgressHub");

        _accounts = mongoDatabase.GetCollection<UserModel>("Users");
        _tags = mongoDatabase.GetCollection<TagModel>("Tags");
    }

    public async Task<BasicResultEnum> ValidateTag(TagModel tag)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(tag.Name))
                return BasicResultEnum.Error;
            
            var tags = await _tags.Find(n => n.Name == tag.Name).ToListAsync();
            if (tags.Count > 0)
                return BasicResultEnum.Error;
            
            return BasicResultEnum.Success;
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
}