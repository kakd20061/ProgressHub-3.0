using System;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Services;

namespace ProgressHubApi.Validators
{
	public interface IAccountSettingsValidator
	{
        public bool ValidatePasswords(string newPassword, string currentPassword, UserModel user);
        public bool ValidateNewPassword(string newPassword);
        public bool ValidateFileForAvatar(IFormFile filetype);
    }

    public class AccountSettingsValidator : IAccountSettingsValidator
    {
        private IMongoCollection<UserModel> _accounts;
        private readonly CommonService _commonService;

        public AccountSettingsValidator(IMongoClient client, CommonService commonService)
        {
            _commonService = commonService;
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
        }
        
        public bool ValidatePasswords(string newPassword, string currentPassword, UserModel user)
        {
            var userPasswordHash = user.Password;
            var currentPasswordHash = _commonService.HashPassword(currentPassword);
            var newPasswordHash = _commonService.HashPassword(newPassword);
            
            if (userPasswordHash != currentPasswordHash)
            {
                return false;
            }

            return ValidateNewPassword(newPassword);
        }
        
        public bool ValidateNewPassword(string newPassword)
        {
            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if(!validateGuidRegex.IsMatch(newPassword))
            {
                return false;
            }
            return true;
        }

        public bool ValidateFileForAvatar(IFormFile file)
        {
            return file is { Length: > 0, ContentType: "image/jpeg" or "image/png" };
        }
    }
}

