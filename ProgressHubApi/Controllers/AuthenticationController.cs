using Microsoft.AspNetCore.Mvc;
using ProgressHubApi.Enums;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Services;

namespace ProgressHubApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _service;
    private readonly ITokenService _tokenService;

    public AuthenticationController(IAuthenticationService service, ITokenService tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> CreateUserAccount(UserDto user)
    {
        var result = await _service.CreateUserAccount(user);

        switch(result)
        {
            case SignUpResultEnum.Success:
                return Ok();
            case SignUpResultEnum.NicknameExists:
                return BadRequest("This nickname is already used");
            case SignUpResultEnum.EmailExists:
                return BadRequest("This email is already used");
            case SignUpResultEnum.EmptyUser:
                return BadRequest("Data is incorrect");
            case SignUpResultEnum.WeakPassword:
                return BadRequest("Your password is too weak");
            case SignUpResultEnum.Error:
                return BadRequest("Unexpected error");
            default:
                return Ok();
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> CheckVerificationCode(VerificationCodeDto code)
    {
        var result = await _service.CheckVerificationCode(code);

        switch (result)
        {
            case VerificationCodeCheckEnum.Success:
                return Ok();
            case VerificationCodeCheckEnum.InvalidCode:
                return BadRequest("Entered code is wrong");
            case VerificationCodeCheckEnum.NoCode:
                return BadRequest("Code for this user doesn't exist. Please regenerate code");
            case VerificationCodeCheckEnum.NoAccount:
                return BadRequest("Your account has been deleted due to inactivity. Please create account again");
            case VerificationCodeCheckEnum.Error:
                return BadRequest("Unexpected error");
            default:
                return Ok();
        }
    }

    [HttpPost("resend")]
    public async Task<IActionResult> ResendVerificationCode(string email)
    {
        var result = await _service.ResendVerificationCode(email);

        switch (result.Item2)
        {
            case BasicResultEnum.Success:
                return Ok();
            case BasicResultEnum.Error:
                return BadRequest(result.Item1);
            default:
                return Ok(result.Item1);
        }
    }

    [HttpPost("signin")]
    public async Task<IActionResult> CheckUserAccount(LoginModel model)
    {
        var result = await _service.CheckUserAccount(model);

        switch (result.Item2)
        {
            case LoginResultEnum.Success:
                var token = await _tokenService.AddRefreshTokenToUser(model.Email);
                if(token.Item2 == BasicResultEnum.Success)
                {
                    return Ok(new TokenModel
                    {
                        AccessToken = result.Item1,
                        RefreshToken = token.Item1
                    });
                }

                return BadRequest("Unexpected Error");
            case LoginResultEnum.InvalidEmail:
                return BadRequest("There is no account with given email");
            case LoginResultEnum.InvalidPassword:
                return BadRequest("Password is invalid");
            case LoginResultEnum.Error:
                return BadRequest("Unexpected Error");
            default:
                return Ok();
        }
    }
    
    [HttpPost("external")]
    public async Task<IActionResult> ExternalLogin(ExternalAuthModel model)
    {
        var result = await _service.ExternalLogin(model);

        switch (result.Item3)
        {
            case BasicResultEnum.Success:
                if (result.Item1 == null) return BadRequest("Unexpected Error");
                var token = await _tokenService.AddRefreshTokenToUser(result.Item1);
                if (token.Item2 == BasicResultEnum.Success)
                {
                    return Ok(new TokenModel
                    {
                        AccessToken = result.Item2,
                        RefreshToken = token.Item1
                    });
                }

                return BadRequest("Unexpected Error");
            case BasicResultEnum.Error:
                return BadRequest("Unexpected Error");
            default:
                return Ok();
        }
    }
}

