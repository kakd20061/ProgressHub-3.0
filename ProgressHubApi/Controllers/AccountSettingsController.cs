using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Services;
using ProgressHubApi.Enums.Authentication;
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
    public async Task<IActionResult> GetTags()
    {
        var result = await _provider.GetAllTags();

        switch (result.Item1)
        {
            case Enums.BasicResultEnum.Success:
                return Ok(result.Item2);
            case Enums.BasicResultEnum.Error:
                return BadRequest();
            default:
                return Ok();
        }
    }
    
    [HttpPost("SaveTags")]
    public async Task<IActionResult> SaveTags(SaveTagsMogel tags)
    {
        var result = await _service.SaveTags(tags);
    
        switch (result)
        {
        case Enums.BasicResultEnum.Success:
            return Ok();
        case Enums.BasicResultEnum.Error:
            return BadRequest();
        default:
            return Ok();
        }
    }
    
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordModelWithCurrentPassword model)
    {
        var result = await _service.ChangePassword(model);
    
        switch (result)
        {
        case Enums.BasicResultEnum.Success:
            return Ok();
        case Enums.BasicResultEnum.Error:
            return BadRequest();
        default:
            return Ok();
        }
    }
}

