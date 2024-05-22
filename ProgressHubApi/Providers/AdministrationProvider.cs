using ProgressHubApi.Enums;
using ProgressHubApi.Models.Administration;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Repositories;

namespace ProgressHubApi.Providers;

public interface IAdministrationProvider
{
    public Task<(BasicResultEnum, ICollection<UserAdministrationModel>?)> GetAllUsers(string token);
    public Task<(BasicResultEnum, ICollection<TagDto>?)> GetAllTags(string token);
}

public class AdministrationProvider: IAdministrationProvider
{
    private readonly IAdministrationRepository _repository;
    
    public AdministrationProvider(IAdministrationRepository repository)
    {
        _repository = repository;
    }


    public async Task<(BasicResultEnum, ICollection<UserAdministrationModel>?)> GetAllUsers(string token)
    {
        var result = await _repository.GetAllUsers(token);
        if(result.Item1 == BasicResultEnum.Error)
            return (BasicResultEnum.Error,null);
        
        var dtos = result.Item2.Select(n=> new UserAdministrationModel(n)).ToList();
        return (BasicResultEnum.Success,dtos);
    }
    
    public async Task<(BasicResultEnum, ICollection<TagDto>?)> GetAllTags(string token)
    {
        var tags = await _repository.GetAllTags(token);
        if(tags.Item1 == BasicResultEnum.Error)
            return (BasicResultEnum.Error,null);
        
        var dtos = tags.Item2.Select(n=>n.ChangeToDto()).ToList();
        return (BasicResultEnum.Success,dtos);
    }
}