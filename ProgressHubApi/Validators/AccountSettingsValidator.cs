using System;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.AccountSettings;
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
        public bool ValidateBodyParameters(BodyParametersChangeModel model);
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

        public bool ValidateBodyParameters(BodyParametersChangeModel model)
        {
            if(model.WeightUnit != "kg" && model.WeightUnit != "lbs")
            {
                return false;
            }
            if(model.HeightUnit != "cm" && model.HeightUnit != "ft in")
            {
                return false;
            }
            if(model.Weight != "")
            {
                switch (model.WeightUnit)
                {
                    case "kg" when (double.Parse(model.Weight.Replace('.',',')) < 20 || double.Parse(model.Weight.Replace('.',',')) > 300 || !Regex.IsMatch(model.Weight, @"^\d+(?:\.\d{1,2})?$|^$")):
                    case "lbs" when (double.Parse(model.Weight.Replace('.',',')) < 50 || double.Parse(model.Weight.Replace('.',',')) > 600 || !Regex.IsMatch(model.Weight, @"^\d+(?:\.\d{1,2})?$|^$")):
                        return false;
                }
            }

            if(model.BodyFatPercentage != "")
            {
                var bodyFatPercentage = double.Parse(model.BodyFatPercentage.Replace('.',','));
                if (bodyFatPercentage < 0 || bodyFatPercentage > 90 || !Regex.IsMatch(model.BodyFatPercentage, @"^\d+(?:\.\d{1,2})?$|^$"))
                {
                    return false;
                }
            }
            
            if (model.Height == "") return true;
            
            switch (model.HeightUnit)
            {
                case "cm" when (int.Parse(model.Height) < 100 || int.Parse(model.Height) > 250 ||
                                !Regex.IsMatch(model.Height, @"^(1[0-9]{2}|2[0-4][0-9]|250)?$")):
                case "ft in" when !Regex.IsMatch(model.Height,  "^[3-8]{1}\\'([0-9]{1}|0[0-9]{1}|1[0-1]{1})(\")$"):
                    return false;
            }
            return true;
        }
    }
}

