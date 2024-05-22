using ProgressHubApi.Enums;
using ProgressHubApi.Models.Administration;
using ProgressHubApi.Repositories;

namespace ProgressHubApi.Services;

public interface IAdministrationService
{
    public Task<BasicResultEnum> AddTag(AddTagModel model);
    public Task<BasicResultEnum> RemoveTag(RemoveTagModel model);
    
    public Task<BasicResultEnum> UpdateTag(UpdateTagModel model);
    
    public Task<BasicResultEnum> ChangeUserRole(ChangeUserRoleModel model);
    
    public Task<BasicResultEnum> BlockUser(BlockUserModel model);
    
    public Task<BasicResultEnum> UnblockUser(UnblockUserModel model);
}

public class AdministrationService : IAdministrationService
{
    private readonly IAdministrationRepository _repository;
    public AdministrationService(IAdministrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<BasicResultEnum> AddTag(AddTagModel model)
    {
        try
        {
            var res = await _repository.AddTag(model);
            return res;
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
            var res = await _repository.RemoveTag(model);
            return res;
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
            var res = await _repository.UpdateTag(model);
            return res;
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
            return await _repository.ChangeUserRole(model);
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
            return await _repository.BlockUser(model);    
        }
        catch(Exception e)
        {
            return BasicResultEnum.Error;
        }
    }

    public async Task<BasicResultEnum> UnblockUser(UnblockUserModel model)
    {
        try
        {
            return await _repository.UnblockUser(model);
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
}