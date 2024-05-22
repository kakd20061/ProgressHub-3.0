using ProgressHubApi.Enums;
using ProgressHubApi.Models.Administration;
using ProgressHubApi.Repositories;

namespace ProgressHubApi.Services;

public interface IAdministrationService
{
    public Task<BasicResultEnum> AddTag(string name);
    public Task<BasicResultEnum> RemoveTag(string name);
    
    public Task<BasicResultEnum> UpdateTag(string oldName, string newName);
    
    public Task<BasicResultEnum> ChangeUserRole(ChangeUserRoleModel model);
    
    public Task<BasicResultEnum> BlockUser(BlockUserModel model);
    
    public Task<BasicResultEnum> UnblockUser(string email);
}

public class AdministrationService : IAdministrationService
{
    private readonly IAdministrationRepository _repository;
    public AdministrationService(IAdministrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<BasicResultEnum> AddTag(string name)
    {
        try
        {
            var res = await _repository.AddTag(name);
            return res;
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
            var res = await _repository.RemoveTag(name);
            return res;
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
            var res = await _repository.UpdateTag(oldName, newName);
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

    public async Task<BasicResultEnum> UnblockUser(string email)
    {
        try
        {
            return await _repository.UnblockUser(email);
        }
        catch (Exception e)
        {
            return BasicResultEnum.Error;
        }
    }
}