using System;
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
using ProgressHubApi.Models.AccountSettings;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Repositories;

namespace ProgressHubApi.Services
{
	public interface IAccountSettingsService
    {
        //functions from repositories
        public Task<BasicResultEnum> SaveTags(SaveTagsMogel model);
        public Task<BasicResultEnum> ChangePassword(ChangePasswordModelWithCurrentPassword model);
        public Task<BasicResultEnum> ChangeAvatar(ChangeAvatarModel model);
    }

	public class AccountSettingsService : IAccountSettingsService
    {
        private readonly IAccountSettingsRepository _repository;
        public AccountSettingsService(IAccountSettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BasicResultEnum> SaveTags(SaveTagsMogel model)
        {
            return await _repository.SaveTags(model);
        }
        
        public async Task<BasicResultEnum> ChangePassword(ChangePasswordModelWithCurrentPassword model)
        {
            return await _repository.ChangePassword(model);
        }

        public Task<BasicResultEnum> ChangeAvatar(ChangeAvatarModel model)
        {
            return _repository.ChangeAvatar(model);
        }
    }
}

