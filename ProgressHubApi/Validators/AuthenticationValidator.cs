using System;
using System.Text.RegularExpressions;
using Amazon.Auth.AccessControlPolicy;
using MongoDB.Driver;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace ProgressHubApi.Validators
{
	public interface IAuthenticationValidator
	{
        public Task<SignUpResultEnum> ValidateCreateUserAccount(UserModel user, string password);
        public Task<(string, BasicResultEnum)> ValidateResendVerificationCode(VerificationCodeModel code);

        public LoginResultEnum ValidateCheckUserAccount(LoginModel model, string? hashedPassword, UserModel userInDb);
        public Task<(VerificationCodeCheckEnum, UserModel?)> ValidateCheckVerificationCode(VerificationCodeModel code);
        public Task<string> GetUniqueUserName(string nickname);
    }

    public class AuthenticationValidator : IAuthenticationValidator
    {
        private IMongoCollection<UserModel> _accounts;
        private IMongoCollection<UserModel> _tempAccounts;
        private IMongoCollection<VerificationCodeModel> _verificationCodes;

        public AuthenticationValidator(IMongoClient client)
        {
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
            _tempAccounts = mongoDatabase.GetCollection<UserModel>("TempUsers");
            _verificationCodes = mongoDatabase.GetCollection<VerificationCodeModel>("VerificationCodes");
        }

        public async Task<SignUpResultEnum> ValidateCreateUserAccount(UserModel user, string password)
        {
            //validation if data is correct

            if(user == null || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.LastName) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Nickname) || string.IsNullOrWhiteSpace(user.Password))
            {
                return SignUpResultEnum.EmptyUser;
            }

            //password validation

            //Has minimum 8 characters in length
            //At least one uppercase English letter
            //At least one lowercase English letter
            //At least one digit
            //At least one special character

            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if(!validateGuidRegex.IsMatch(password))
            {
                return SignUpResultEnum.WeakPassword;
            }

            //validation if item exists in db

            var tempAccounts = await _tempAccounts.Find(_ => true).ToListAsync();
            var accounts = await _accounts.Find(_ => true).ToListAsync();
            var verificationCodes = await _verificationCodes.Find(_ => true).ToListAsync();

            var tempAccount = tempAccounts.FirstOrDefault(n => n.Email == user.Email);
            var account = accounts.FirstOrDefault(n => n.Email == user.Email);

            if (tempAccount != null || account != null)
            {
                return SignUpResultEnum.EmailExists;
            }

            tempAccount = tempAccounts.FirstOrDefault(n => n.Nickname == user.Nickname);
            account = accounts.FirstOrDefault(n => n.Nickname == user.Nickname);

            if (account != null || tempAccount != null)
            {
                return SignUpResultEnum.NicknameExists;
            }

            var verificationCode = verificationCodes.FirstOrDefault(n => n.Email == user.Email);

            if (verificationCode != null)
            {
                await _verificationCodes.DeleteOneAsync(n => n.Email == user.Email);
            }

            return SignUpResultEnum.Success;
        }

        public async Task<(string, BasicResultEnum)> ValidateResendVerificationCode(VerificationCodeModel code)
        {
            //validation if there is temp account with provided email

            var tempAccounts = await _tempAccounts.Find(_ => true).ToListAsync();
            var tempAccount = tempAccounts.FirstOrDefault(n => n.Email == code.Email);
            if (tempAccount == null)
            {
                return ("There is no account with entered email. Please sign up again with this email", BasicResultEnum.Error);
            }
            return ("Model is valid", BasicResultEnum.Success);
        }

        public LoginResultEnum ValidateCheckUserAccount(LoginModel model,string? hashedPassword, UserModel userInDb)
        {
            if(userInDb == null)
            {
                return LoginResultEnum.InvalidEmail;
            }

            if (userInDb.Password == null || !isValidPassword(userInDb.Password, hashedPassword))
            {
                return LoginResultEnum.InvalidPassword;
            }
            
            if(userInDb.BanExpirationDate != null && userInDb.BanExpirationDate > DateTime.UtcNow)
            {
                return LoginResultEnum.Blocked;
            }
            
            return LoginResultEnum.Success;
        }

        private bool isValidPassword(string? dbPassword, string? providedPassword)
        {
            return dbPassword == providedPassword ? true : false;
        }
        
        public async Task<(VerificationCodeCheckEnum,UserModel?)> ValidateCheckVerificationCode(VerificationCodeModel code)
        {
            var verificationCodes = await _verificationCodes.Find(_ => true).ToListAsync();
            var codeInDatabase = verificationCodes.FirstOrDefault(n => n.Email == code.Email);
            if (codeInDatabase != null)
            {
                if (codeInDatabase.Code == code.Code)
                {
                    var tempAccounts = await _tempAccounts.Find(_ => true).ToListAsync();
                    var tempAccount = tempAccounts.FirstOrDefault(n => n.Email == code.Email);
                    await _verificationCodes.DeleteOneAsync(n => n.Email == code.Email);

                    if (tempAccount != null)
                    {
                        return (VerificationCodeCheckEnum.Success,tempAccount);
                    }
                    else
                    {
                        return (VerificationCodeCheckEnum.NoAccount,null);
                    }
                }
                else
                {
                    return (VerificationCodeCheckEnum.InvalidCode, null);
                }
            }
            else
            {
                return (VerificationCodeCheckEnum.NoCode, null);
            }

        }

        public async Task<string> GetUniqueUserName(string nickname)
        {
            var accounts = await _accounts.Find(_ => true).ToListAsync();
            while(true){
                var username = accounts.FirstOrDefault(n => n.Nickname == nickname);
                if (username != null)
                {
                    nickname = nickname + " " + new Random().Next(0, 1000);
                }
                else
                {
                    return nickname;
                }
            }
        }
    }
}

