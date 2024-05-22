using Microsoft.AspNetCore.Mvc;
using ProgressHubApi.Models.Administration;
using ProgressHubApi.Providers;
using ProgressHubApi.Services;

namespace ProgressHubApi.Controllers;

[ApiController]
[Route("api/administration")]
public class AdministrationController : ControllerBase
{
    private readonly IAdministrationService _service;
    private readonly IAdministrationProvider _provider;

    
    public AdministrationController(IAdministrationProvider provider, IAdministrationService service)
    {
        _provider = provider;
        _service = service;
    }
    
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers(string token)
    {
        var result = await _provider.GetAllUsers(token);

        return result.Item1 switch
        {
            Enums.BasicResultEnum.Success => Ok(result.Item2),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked"),
            _ => Ok()
        };
    }
    
    [HttpGet("GetAllTags")]
    public async Task<IActionResult> GetAllTags(string token)
    {
        var result = await _provider.GetAllTags(token);

        return result.Item1 switch
        {
            Enums.BasicResultEnum.Success => Ok(result.Item2),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked"),
            _ => Ok()
        };
    }
    
    [HttpPost("AddTag")]
    public async Task<IActionResult> AddTag(AddTagModel model)
    {
        var result = await _service.AddTag(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked"),
            _ => Ok()
        };
    }
    
    [HttpPost("RemoveTag")]
    public async Task<IActionResult> RemoveTag(RemoveTagModel model)
    {
        var result = await _service.RemoveTag(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked"),
            _ => Ok()
        };
    }
    
    [HttpPut("UpdateTag")]
    public async Task<IActionResult> UpdateTag(UpdateTagModel model)
    {
        var result = await _service.UpdateTag(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked"),
            _ => Ok()
        };
    }
    
    [HttpPost("ChangeUserRole")]
    public async Task<IActionResult> ChangeUserRole(ChangeUserRoleModel model)
    {
        var result = await _service.ChangeUserRole(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked"),
            _ => Ok()
        };
    }
    
    [HttpPost("BlockUser")]
    public async Task<IActionResult> BlockUser(BlockUserModel model)
    {
        var result = await _service.BlockUser(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked"),
            _ => Ok()
        };
    }
    
    [HttpPost("UnblockUser")]
    public async Task<IActionResult> UnblockUser(UnblockUserModel model)
    {
        var result = await _service.UnblockUser(model);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            Enums.BasicResultEnum.Blocked => Unauthorized("User is blocked"),
            _ => Ok()
        };
    }
}