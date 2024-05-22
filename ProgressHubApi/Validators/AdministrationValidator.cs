using System.Security.Claims;
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
    public Task<BasicResultEnum> ValidateToken(string token);
    public Task<BasicResultEnum> ValidateTokenForUserManagement(string token, string userEmail);

}

public class AdministrationValidator : IAdministrationValidator
{
    private IMongoCollection<UserModel> _accounts;
    private IMongoCollection<TagModel> _tags;
    private readonly CommonService _commonService;

    public AdministrationValidator(IMongoClient client, CommonService commonService)
    {
        _commonService = commonService;
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
    
    public async Task<BasicResultEnum> ValidateToken(string token)
    {
        try
        {
            var principals = _commonService.GetPrincipals(token);
            if (principals == null)
            {
                return BasicResultEnum.Error;
            }
            var principalEmail = principals.Claims.FirstOrDefault(n=>n.Type == ClaimTypes.Email).Value;
            
            var user = await _accounts.FindAsync(n => n.Email == principalEmail);
            var firstUser = await user.FirstOrDefaultAsync();
            if (firstUser == null)
            {
                return BasicResultEnum.Error;
            }
            
            if(firstUser.Role != UserRoleEnum.Admin && firstUser.Role != UserRoleEnum.Owner)
            {
                return BasicResultEnum.Error;
            }
            
            return firstUser.BanExpirationDate != null ? BasicResultEnum.Error : BasicResultEnum.Success;
        }
        catch(Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
    
    public async Task<BasicResultEnum> ValidateTokenForUserManagement(string token, string userEmail)
    {
        try
        {
            var principals = _commonService.GetPrincipals(token);
            if (principals == null)
            {
                return BasicResultEnum.Error;
            }
            var principalEmail = principals.Claims.FirstOrDefault(n=>n.Type == ClaimTypes.Email).Value;


            var user = await _accounts.FindAsync(n => n.Email == principalEmail);
            var firstUser = await user.FirstOrDefaultAsync();
            if (firstUser == null)
            {
                return BasicResultEnum.Error;
            }
            
            var changedUser = await _accounts.FindAsync(n => n.Email == userEmail);
            var userForEdition = await changedUser.FirstOrDefaultAsync();
            if (userForEdition == null)
            {
                return BasicResultEnum.Error;
            }
            
            if(firstUser.Role != UserRoleEnum.Admin && firstUser.Role != UserRoleEnum.Owner)
            {
                return BasicResultEnum.Error;
            }
            
            if(firstUser.Role == UserRoleEnum.Admin && userForEdition.Role is UserRoleEnum.Admin or UserRoleEnum.Owner)
            {
                return BasicResultEnum.Error;
            }
            
            return firstUser.BanExpirationDate != null ? BasicResultEnum.Error : BasicResultEnum.Success;
        }
        catch(Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
}