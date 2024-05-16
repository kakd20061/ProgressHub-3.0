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
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _provider.GetAllUsers();

        return result.Item1 switch
        {
            Enums.BasicResultEnum.Success => Ok(result.Item2),
            Enums.BasicResultEnum.Error => BadRequest(),
            _ => Ok()
        };
    }
    
    [HttpGet("GetAllTags")]
    public async Task<IActionResult> GetAllTags()
    {
        var result = await _provider.GetAllTags();

        return result.Item1 switch
        {
            Enums.BasicResultEnum.Success => Ok(result.Item2),
            Enums.BasicResultEnum.Error => BadRequest(),
            _ => Ok()
        };
    }
    
    [HttpPost("AddTag")]
    public async Task<IActionResult> AddTag(string name)
    {
        var result = await _service.AddTag(name);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            _ => Ok()
        };
    }
    
    [HttpDelete("RemoveTag")]
    public async Task<IActionResult> RemoveTag(string name)
    {
        var result = await _service.RemoveTag(name);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            _ => Ok()
        };
    }
    
    [HttpPut("UpdateTag")]
    public async Task<IActionResult> UpdateTag(UpdateTagModel model)
    {
        var result = await _service.UpdateTag(model.oldName, model.newName);

        return result switch
        {
            Enums.BasicResultEnum.Success => Ok(),
            Enums.BasicResultEnum.Error => BadRequest(),
            _ => Ok()
        };
    }
}