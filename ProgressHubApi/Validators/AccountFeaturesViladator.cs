using System;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;

namespace ProgressHubApi.Validators
{
	public interface IAccountFeaturesValidator
	{
        public Task<(string, BasicResultEnum)> ValidateResetPasswordCode(VerificationCodeModel code);
        public ChangePasswordEnum ValidateChangePassword(string password);
        
        public Task<VerificationCodeCheckEnum> ValidateCheckVerificationCode(VerificationCodeModel code);
    }

    public class AccountFeaturesValidator : IAccountFeaturesValidator
    {
        private IMongoCollection<UserModel> _accounts;
        private IMongoCollection<UserModel> _tempAccounts;
        private IMongoCollection<VerificationCodeModel> _verificationCodes;

        public AccountFeaturesValidator(IMongoClient client)
        {
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
            _tempAccounts = mongoDatabase.GetCollection<UserModel>("TempUsers");
            _verificationCodes = mongoDatabase.GetCollection<VerificationCodeModel>("VerificationCodes");
        }

        public ChangePasswordEnum ValidateChangePassword(string password)
        {
            //password validation

            //Has minimum 8 characters in length
            //At least one uppercase English letter
            //At least one lowercase English letter
            //At least one digit
            //At least one special character

            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if (!validateGuidRegex.IsMatch(password))
            {
                return ChangePasswordEnum.WeakPassword;
            }
            return ChangePasswordEnum.Success;
        }

        public async Task<VerificationCodeCheckEnum> ValidateCheckVerificationCode(VerificationCodeModel code)
        {
            var verificationCodes = await _verificationCodes.Find(_ => true).ToListAsync();
            var codeInDatabase = verificationCodes.FirstOrDefault(n => n.Email == code.Email);
            if (codeInDatabase == null) return VerificationCodeCheckEnum.NoCode;
            
            if (codeInDatabase.Code != code.Code) return VerificationCodeCheckEnum.InvalidCode;
            var accounts = await _accounts.Find(_ => true).ToListAsync();
            var account = accounts.FirstOrDefault(n => n.Email == code.Email);
            await _verificationCodes.DeleteOneAsync(n => n.Email == code.Email);

            return account != null ? VerificationCodeCheckEnum.Success : VerificationCodeCheckEnum.NoAccount;
        }

        public async Task<(string, BasicResultEnum)> ValidateResetPasswordCode(VerificationCodeModel code)
        {
            //validation if there is account with provided email

            var accounts = await _accounts.Find(_ => true).ToListAsync();
            var account = accounts.FirstOrDefault(n => n.Email == code.Email);
            if (account == null)
            {
                return ("There is no account with entered email", BasicResultEnum.Error);
            }
            return ("Model is valid", BasicResultEnum.Success);
        }
    }
}

