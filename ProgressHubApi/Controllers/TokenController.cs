using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Services;

namespace ProgressHubApi.Controllers;

[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenModel model)
    {
        var result = await _tokenService.Refresh(model);

        switch (result.Item2)
        {
            case Enums.BasicResultEnum.Success:
                return Ok(result.Item1);
            case Enums.BasicResultEnum.Error:
                return BadRequest("Unexpected error");
            case Enums.BasicResultEnum.Forbidden:
                return Forbid();
            default:
                return Ok();
        }
    }
}
