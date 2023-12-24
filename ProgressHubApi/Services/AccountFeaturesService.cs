using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Amazon.Auth.AccessControlPolicy;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Repositories;

namespace ProgressHubApi.Services
{
	public interface IAccountFeaturesService
    {
        public Task<(string, BasicResultEnum)> ResetPasswordCode(string email);
        public Task<ChangePasswordEnum> ChangePassword(ChangePasswordModel model);
        public Task<VerificationCodeCheckEnum> CheckVerificationCode(VerificationCodeDto code);

    }

    public class AccountFeaturesService : IAccountFeaturesService
    {
        private readonly IAccountFeaturesRepository _accountFeaturesRepository;

        public AccountFeaturesService(IAccountFeaturesRepository accountFeaturesRepository)
        {
            _accountFeaturesRepository = accountFeaturesRepository;
        }

        public async Task<VerificationCodeCheckEnum> CheckVerificationCode(VerificationCodeDto code)
        {
            var verificationCodeModel = new VerificationCodeModel(null, code.Email, code.Code); 
            return await _accountFeaturesRepository.CheckVerificationCode(verificationCodeModel);
        }
        
        public async Task<ChangePasswordEnum> ChangePassword(ChangePasswordModel model)
        {
            var hashedPassword = CommonService.HashPassword(model.password);
            return await _accountFeaturesRepository.ChangePassword(model, hashedPassword);
        }

        public async Task<(string, BasicResultEnum)> ResetPasswordCode(string email)
        {
            var verificationCodeModel = new VerificationCodeModel(null, email, CommonService.GenerateVerificationCode());
            var mailRequestModel = new MailRequestModel()
            {
                ToEmail = email,
                Subject = "ProgressHub - Reset your password",
                Body = $"This is your 4 digit code: {verificationCodeModel.Code}. Use it to change your password - {email}"
            };
            return await _accountFeaturesRepository.ResetPasswordCode(verificationCodeModel, mailRequestModel);
        }
    }
}

