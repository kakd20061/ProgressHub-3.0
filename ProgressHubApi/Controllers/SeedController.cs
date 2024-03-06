using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Services;
using ProgressHubApi.Enums.Authentication;
namespace ProgressHubApi.Controllers;

[ApiController]
[Route("api/data")]
public class SeedController : ControllerBase
{
    private readonly ISeedService _service;

    public SeedController(ISeedService service)
    {
        _service = service;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        var result = await _service.SeedTags();

        switch (result.Item1)
        {
            case Enums.BasicResultEnum.Success:
                return Ok();
            case Enums.BasicResultEnum.Error:
                return BadRequest(result.Item2);
            default:
                return Ok();
        }
    }
}

