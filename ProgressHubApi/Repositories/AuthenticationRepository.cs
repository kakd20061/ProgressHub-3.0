﻿using Google.Apis.Auth;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Services;
using ProgressHubApi.Validators;

namespace ProgressHubApi.Repositories
{
	public interface IAuthenticationRepository
	{
        //registration
        public Task<SignUpResultEnum> CreateUserAccount(UserModel user, VerificationCodeModel code, MailRequestModel model, string password);
        public Task<VerificationCodeCheckEnum> CheckVerificationCode(VerificationCodeModel code);
        public Task<(string, BasicResultEnum)> ResendVerificationCode(VerificationCodeModel code, MailRequestModel model);
        //login
        public Task<(UserModel?, LoginResultEnum)> CheckUserAccount(LoginModel model, string? hashedPassword);
        
        //external
        public Task<(UserModel?,BasicResultEnum)> ExternalLogin(GoogleJsonWebSignature.Payload payload);
    }

    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IMongoCollection<UserModel> _accounts;
        private readonly IMongoCollection<UserModel> _tempAccounts;
        private readonly IMongoCollection<VerificationCodeModel> _verificationCodes;
        private readonly IMailService _mailService;
        private readonly IAuthenticationValidator _validator;

        public AuthenticationRepository(IMongoClient client, IMailService mailService, IAuthenticationValidator validator)
        {
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
            _tempAccounts = mongoDatabase.GetCollection<UserModel>("TempUsers");
            _verificationCodes = mongoDatabase.GetCollection<VerificationCodeModel>("VerificationCodes");
            _mailService = mailService;
            _validator = validator;
        }

        //registration--------------
        public async Task<SignUpResultEnum> CreateUserAccount(UserModel user, VerificationCodeModel code, MailRequestModel model, string password)
        {
            try
            {
                //validating
                var isValid = await _validator.ValidateCreateUserAccount(user, password);

                if(isValid != SignUpResultEnum.Success)
                {
                    return isValid;
                }

                //creating account
                await _mailService.SendEmailAsync(model);
                await _tempAccounts.InsertOneAsync(user);
                await _verificationCodes.InsertOneAsync(code);
                return SignUpResultEnum.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return SignUpResultEnum.Error;
            }
        }

        public async Task<VerificationCodeCheckEnum> CheckVerificationCode(VerificationCodeModel code)
        {
            try
            {
                //validating
                var isValid = await _validator.ValidateCheckVerificationCode(code);
                if(isValid.Item1 != VerificationCodeCheckEnum.Success)
                {
                    return isValid.Item1;
                }

                //creating normal account
                var tempAccount = isValid.Item2;
                tempAccount._Id = null;
                await _accounts.InsertOneAsync(tempAccount);
                await _tempAccounts.DeleteOneAsync(n => n.Email == tempAccount.Email);

                return VerificationCodeCheckEnum.Success;
            }
            catch (Exception e)
            {
                return VerificationCodeCheckEnum.Error;
            }
        }

        public async Task<(string,BasicResultEnum)> ResendVerificationCode(VerificationCodeModel code, MailRequestModel model)
        {
            try
            {
                //validating
                var isValid = await _validator.ValidateResendVerificationCode(code);
                if (isValid.Item2 == BasicResultEnum.Error)
                {
                    return isValid;
                }

                //resending
                await _mailService.SendEmailAsync(model);
                await _verificationCodes.DeleteOneAsync(n => n.Email == code.Email);
                await _verificationCodes.InsertOneAsync(code);
                return ("New code is successfully resent",BasicResultEnum.Success);
            }
            catch (Exception e)
            {
                return (e.ToString(),BasicResultEnum.Error);
            }
        }

        //login--------------

        public async Task<(UserModel?, LoginResultEnum)> CheckUserAccount(LoginModel model, string? hashedPassword)
        {
            var accounts = await _accounts.Find(_ => true).ToListAsync();
            var account = accounts.FirstOrDefault(n => n.Email == model.Email);

            var isValid = _validator.ValidateCheckUserAccount(model,hashedPassword,account);
            if (isValid == LoginResultEnum.Success)
            {
                var updatedModel = new UserModel(account, DateTime.UtcNow);
                await _accounts.FindOneAndReplaceAsync(x => x.Email.Equals(updatedModel.Email), updatedModel);
                return (updatedModel, isValid);
            }
            return (null, isValid);
        }
        
        //external--------------
        
        public async Task<(UserModel?,BasicResultEnum)> ExternalLogin(GoogleJsonWebSignature.Payload payload)
        {
            try
            {
                var accounts = await _accounts.Find(_ => true).ToListAsync();
                var account = accounts.FirstOrDefault(n => n.Email == payload.Email);
                if (account != null)
                {
                    var updatedModel = new UserModel(account, DateTime.UtcNow);
                    await _accounts.FindOneAndReplaceAsync(x => x.Email.Equals(updatedModel.Email), updatedModel);
                    return (account, BasicResultEnum.Success);
                }
                payload.FamilyName ??= "";
                payload.GivenName ??= "";

                var user = new UserModel(null, payload.GivenName, payload.FamilyName, payload.Email, payload.Name, null, null);
                await _accounts.InsertOneAsync(user);
                return (user, BasicResultEnum.Success);
            }
            catch (Exception e)
            {
                return (null,BasicResultEnum.Error);
            }
        }
    }
}

