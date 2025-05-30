﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Amazon.Auth.AccessControlPolicy;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Repositories;

namespace ProgressHubApi.Services
{
	public interface IAuthenticationService
    {
        //functions from repositories
        public Task<SignUpResultEnum> CreateUserAccount(UserDto user);
        public Task<VerificationCodeCheckEnum> CheckVerificationCode(VerificationCodeDto code);
        public Task<(string, BasicResultEnum)> ResendVerificationCode(string email);

        public Task<(string?, LoginResultEnum)> CheckUserAccount(LoginModel model);
        public Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(ExternalAuthModel externalAuth);
        public Task<(string?,string?, BasicResultEnum)> ExternalLogin(ExternalAuthModel model);
    }

	public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _repository;
        private readonly CommonService _commonService;
        private readonly GoogleSettingsModel _googleSettings;
        public AuthenticationService(IAuthenticationRepository repository, CommonService commonService, IOptions<GoogleSettingsModel> googleSettings)
        {
            _repository = repository;
            _commonService = commonService;
            _googleSettings = googleSettings.Value;
        }

        public async Task<SignUpResultEnum> CreateUserAccount(UserDto user)
        {
            var encryptedPassword = _commonService.HashPassword(user.Password);
            var userModel = new UserModel(null, user.Name, user.Lastname, user.Email, user.NickName, encryptedPassword, null);
            var verificationCodeModel = new VerificationCodeModel(null, user.Email, _commonService.GenerateVerificationCode());

            var mailRequestModel = new MailRequestModel()
            {
                ToEmail = user.Email,
                Subject = "ProgressHub - Confirm your email",
                Body = $"This is your 4 digit code: {verificationCodeModel.Code}. Use it to confirm your account - {user.Email}"
            };

            return await _repository.CreateUserAccount(userModel, verificationCodeModel, mailRequestModel, user.Password);
        }


        public async Task<VerificationCodeCheckEnum> CheckVerificationCode(VerificationCodeDto code)
        {
            var verificationCodeModel = new VerificationCodeModel(null, code.Email, code.Code); 
            return await _repository.CheckVerificationCode(verificationCodeModel);
        }

        public async Task<(string, BasicResultEnum)> ResendVerificationCode(string email)
        {
            var verificationCodeModel = new VerificationCodeModel(null, email, _commonService.GenerateVerificationCode());
            var mailRequestModel = new MailRequestModel()
            {
                ToEmail = email,
                Subject = "ProgressHub - Confirm your email",
                Body = $"This is your 4 digit code: {verificationCodeModel.Code}. Use it to confirm your account - {email}"
            };
            return await _repository.ResendVerificationCode(verificationCodeModel,mailRequestModel);
        }

        public async Task<(string?, LoginResultEnum)> CheckUserAccount(LoginModel model)
        {
            var hashedPassword = _commonService.HashPassword(model.Password);
            var result = await _repository.CheckUserAccount(model,hashedPassword);
            if (result.Item2 != LoginResultEnum.Success) return (null, result.Item2);
            var token = _commonService.GenerateJwt(result.Item1);
            return (token,result.Item2);
        }

        public async Task<(string?,string?, BasicResultEnum)> ExternalLogin(ExternalAuthModel model)
        {
            var payload =  await VerifyGoogleToken(model);
            if(payload == null) return (null,null, BasicResultEnum.Error);
            var result = await _repository.ExternalLogin(payload);

            if (result.Item2 != BasicResultEnum.Success) return (null, null, result.Item2);
            var token = _commonService.GenerateJwt(result.Item1);
            return (payload.Email, token, result.Item2);
        }
        
        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(ExternalAuthModel externalAuth)
        {
            try
            {
                GoogleJsonWebSignature.ValidationSettings settings;
                if (Environment.GetEnvironmentVariable("GOOGLEID") != null)
                {
                    settings = new GoogleJsonWebSignature.ValidationSettings()
                    {
                        Audience = new List<string>() { Environment.GetEnvironmentVariable("GOOGLEID") }
                    };
                }
                else
                {
                    settings = new GoogleJsonWebSignature.ValidationSettings()
                    {
                        Audience = new List<string>() { _googleSettings.Id }
                    };
                }
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

