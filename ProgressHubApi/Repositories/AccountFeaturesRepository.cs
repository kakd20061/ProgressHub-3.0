using System;
using System.Text.Encodings.Web;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Services;
using ProgressHubApi.Validators;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace ProgressHubApi.Repositories
{
	public interface IAccountFeaturesRepository
	{
        public Task<(string, BasicResultEnum)> ResetPasswordCode(VerificationCodeModel code, MailRequestModel model);
        public Task<ChangePasswordEnum> ChangePassword(ChangePasswordModel model,string? hashedPassword);
        
        public Task<VerificationCodeCheckEnum> CheckVerificationCode(VerificationCodeModel code);
    }

    public class AccountFeaturesRepository : IAccountFeaturesRepository
    {
        private IMongoCollection<UserModel> _accounts;
        private IMongoCollection<UserModel> _tempAccounts;
        private IMongoCollection<VerificationCodeModel> _verificationCodes;
        private IMailService _mailService;
        private IAccountFeaturesValidator _validator;

        public AccountFeaturesRepository(IMongoClient client, IMailService mailService, IAccountFeaturesValidator validator)
        {
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
            _tempAccounts = mongoDatabase.GetCollection<UserModel>("TempUsers");
            _verificationCodes = mongoDatabase.GetCollection<VerificationCodeModel>("VerificationCodes");
            _mailService = mailService;
            _validator = validator;
        }

        public async Task<ChangePasswordEnum> ChangePassword(ChangePasswordModel model,string?  hashedPassword)
        {
            try
            {
                //validating
                var isValid = _validator.ValidateChangePassword(model.password);
                if (isValid != ChangePasswordEnum.Success)
                {
                    return isValid;
                }

                var accounts = await _accounts.Find(_ => true).ToListAsync();
                var account = accounts.FirstOrDefault(n=>n.Email.Equals(model.email));
                account.Password = hashedPassword;

                await _accounts.FindOneAndReplaceAsync(x => x.Email.Equals(model.email), account);

                return ChangePasswordEnum.Success;
            }
            catch (Exception e)
            {
                return ChangePasswordEnum.Error;
            }
        }

        public async Task<VerificationCodeCheckEnum> CheckVerificationCode(VerificationCodeModel code)
        {
            try
            {
                //validating
                var isValid = await _validator.ValidateCheckVerificationCode(code);
                
                return isValid;
            }
            catch (Exception e)
            {
                return VerificationCodeCheckEnum.Error;
            }
        }

        public async Task<(string, BasicResultEnum)> ResetPasswordCode(VerificationCodeModel code, MailRequestModel model)
        {
            try
            {
                //validating
                var isValid = await _validator.ValidateResetPasswordCode(code);
                if (isValid.Item2 == BasicResultEnum.Error)
                {
                    return isValid;
                }

                //resending
                await _mailService.SendEmailAsync(model);
                await _verificationCodes.DeleteOneAsync(n => n.Email == code.Email);
                await _verificationCodes.InsertOneAsync(code);
                return ("New code is successfully resent", BasicResultEnum.Success);
            }
            catch (Exception e)
            {
                return (e.ToString(), BasicResultEnum.Error);
            }
        }
    }
}

