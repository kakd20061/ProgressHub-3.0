using Org.BouncyCastle.Tls;
using ProgressHubApi.Enums;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Repositories;

namespace ProgressHubApi.Providers;

public interface IAccountSettingsProvider
{
    public Task<(BasicResultEnum, ICollection<TagDto>?)> GetAllTags(string token);
}
public class AccountSettingsProvider: IAccountSettingsProvider
{
    private readonly IAccountSettingsRepository _repository;
    
    public AccountSettingsProvider(IAccountSettingsRepository repository)
    {
        _repository = repository;
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