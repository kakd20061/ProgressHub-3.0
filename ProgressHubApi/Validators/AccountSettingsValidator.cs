using System;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.AccountSettings;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.AccountSettings;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Services;

namespace ProgressHubApi.Validators
{
	public interface IAccountSettingsValidator
	{
        public bool ValidatePasswords(string newPassword, string currentPassword, UserModel user);
        public bool ValidateNewPassword(string newPassword);
        public bool ValidateFileForAvatar(IFormFile filetype);
        public Task<PersonalDataChangeResultEnum> ValidatePersonalData(PersonalDataChangeModel model);
    }

    public class AccountSettingsValidator : IAccountSettingsValidator
    {
        private IMongoCollection<UserModel> _accounts;
        private IMongoCollection<UserModel> _tempAccounts;
        private readonly CommonService _commonService;

        public AccountSettingsValidator(IMongoClient client, CommonService commonService)
        {
            _commonService = commonService;
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
            _tempAccounts = mongoDatabase.GetCollection<UserModel>("TempUsers");
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

        public async Task<PersonalDataChangeResultEnum> ValidatePersonalData(PersonalDataChangeModel model)
        {
            var tempAccounts = await _tempAccounts.Find(_ => true).ToListAsync();
            var accounts = await _accounts.Find(_ => true).ToListAsync();

            if (tempAccounts.FirstOrDefault(n=>n.Nickname == model.Nickname && n.Email != model.Email) != null || accounts.FirstOrDefault(n=>n.Nickname == model.Nickname&& n.Email != model.Email) != null)
            {
                return PersonalDataChangeResultEnum.NicknameExists;
            }
            if (String.IsNullOrWhiteSpace(model.Name) || model.Name.Length < 2 || model.Name.Length > 50)
            {
                return PersonalDataChangeResultEnum.Error;
            }
            if (String.IsNullOrWhiteSpace(model.LastName) || model.LastName.Length < 2 || model.LastName.Length > 50)
            {
                return PersonalDataChangeResultEnum.Error;
            }
            if(!Enum.TryParse<UserGenderEnum>(model.Gender, out _))
            {
                return PersonalDataChangeResultEnum.Error;
            }

            if (String.IsNullOrWhiteSpace(model.DateOfBirth)) return PersonalDataChangeResultEnum.Success;
            if(DateTime.Parse(model.DateOfBirth) > DateTime.Now || DateTime.Parse(model.DateOfBirth) < new DateTime(1900, 1, 1))
            {
                return PersonalDataChangeResultEnum.Error;
            }
            return PersonalDataChangeResultEnum.Success;
        }
    }
}

