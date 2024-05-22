using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProgressHubApi.Enums.AccountSettings;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Services;
using ProgressHubApi.Models.AccountSettings;
using ProgressHubApi.Providers;

namespace ProgressHubApi.Controllers;

[ApiController]
[Route("api/settings/account")]
public class AccountSettingsController : ControllerBase
{
    private readonly IAccountSettingsProvider _provider;
    private readonly IAccountSettingsService _service;

    public AccountSettingsController(IAccountSettingsProvider provider, IAccountSettingsService service)
    {
        _provider = provider;
        _service = service;
    }

    [HttpGet("GetAllTags")]
    public async Task<IActionResult> GetTags(string token)
    {
        var result = await _provider.GetAllTags(token);

        return result.Item1 switch
        {
            Enums.BasicResultEnum.Success => Ok(result.Item2),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked or unauthorized"),
            _ => Ok()
        };
    }
    
    [HttpPost("SaveTags")]
    public async Task<IActionResult> SaveTags(SaveTagsMogel tags)
    {
        var result = await _service.SaveTags(tags);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked or unauthorized"),
            _ => Ok()
        };
    }
    
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordModelWithCurrentPassword model)
    {
        var result = await _service.ChangePassword(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked or unauthorized"),
            _ => Ok()
        };
    }
    
    [HttpPost("ChangeAvatar")]
    public async Task<IActionResult> ChangeAvatar()
    {
        var formCollection = await Request.ReadFormAsync();
        IFormFile? file = null;
        if (formCollection["file"] != "")
        {
            file = formCollection.Files.First();
        }
        var model = new ChangeAvatarModel
        {
            Email = formCollection["email"],
            File = file,
            Token = formCollection["token"]
        };
        var result = await _service.ChangeAvatar(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked or unauthorized"),
            _ => Ok()
        };
    }
    
    [HttpPost("ChangePersonalData")]
    public async Task<IActionResult> ChangePersonalData(PersonalDataChangeModel model)
    {
        var result = await _service.ChangePersonalData(model);

        return result switch
        {
            PersonalDataChangeResultEnum.Success => Ok(),
            PersonalDataChangeResultEnum.Error => BadRequest(),
            PersonalDataChangeResultEnum.NicknameExists => BadRequest("Nickname already exists"),
            PersonalDataChangeResultEnum.Blocked => Unauthorized("User is blocked or unauthorized"),
            _ => Ok()
        };
    }
    
    [HttpPost("ChangeBodyParameters")]
    public async Task<IActionResult> ChangeBodyParameters(BodyParametersChangeModel model)
    {
        var result = await _service.ChangeBodyParameters(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked or unauthorized"),
            _ => Ok()
        };
    }
}

