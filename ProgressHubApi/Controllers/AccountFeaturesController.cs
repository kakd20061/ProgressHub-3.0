using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Services;

namespace ProgressHubApi.Controllers;

[ApiController]
[Route("api/features")]
public class AccountFeaturesController : ControllerBase
{
    private readonly IAccountFeaturesService _service;

    public AccountFeaturesController(IAccountFeaturesService service)
    {
        _service = service;
    }

    [HttpPost("resend")]
    public async Task<IActionResult> ResetPasswordCode(string email)
    {
        var result = await _service.ResetPasswordCode(email);

        switch (result.Item2)
        {
            case Enums.BasicResultEnum.Success:
                return Ok();
            case Enums.BasicResultEnum.Error:
                return BadRequest(result.Item1);
            default:
                return Ok();
        }
    }

    [HttpPost("change")]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        var result = await _service.ChangePassword(model);

        switch (result)
        {
            case Enums.ChangePasswordEnum.Success:
                return Ok();
            case Enums.ChangePasswordEnum.WeakPassword:
                return BadRequest("Your new password is too weak");
            case Enums.ChangePasswordEnum.Error:
                return BadRequest("Unexpected error");
            default:
                return Ok();
        }
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckVerificationCode(VerificationCodeDto code)
    {
        var result = await _service.CheckVerificationCode(code);

        switch (result)
        {
            case Enums.VerificationCodeCheckEnum.Success:
                return Ok();
            case Enums.VerificationCodeCheckEnum.Error:
                return BadRequest("Unexpected error");
            case Enums.VerificationCodeCheckEnum.InvalidCode:
                return BadRequest("Entered code is wrong");
            case Enums.VerificationCodeCheckEnum.NoCode:
                return BadRequest("Code for this user doesn't exist. Please regenerate code");
            case Enums.VerificationCodeCheckEnum.NoAccount:
                return BadRequest("There is no account with provided email");
            default:
                return Ok();
        }
    }
}

