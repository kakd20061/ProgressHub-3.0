using System;
using System.Text.Encodings.Web;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.AccountSettings;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Services;
using ProgressHubApi.Validators;

namespace ProgressHubApi.Repositories
{
	public interface ISeedRepository
	{
        public Task<(BasicResultEnum,string)> SeedTags();
    }

    public class SeedRepository : ISeedRepository
    {
        private IMongoCollection<UserModel> _accounts;
        private IMongoCollection<UserModel> _tempAccounts;
        private IMongoCollection<VerificationCodeModel> _verificationCodes;
        private IMongoCollection<TagModel> _tags;

        public SeedRepository(IMongoClient client)
        {
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
            _tempAccounts = mongoDatabase.GetCollection<UserModel>("TempUsers");
            _verificationCodes = mongoDatabase.GetCollection<VerificationCodeModel>("VerificationCodes");
            _tags = mongoDatabase.GetCollection<TagModel>("Tags");
        }

        public async Task<(BasicResultEnum,string)> SeedTags()
        {
            try
            {
                var tag1 = new TagModel
                {
                    Name = "Strongman",
                };
                var tag2 = new TagModel
                {
                    Name = "Powerlifting",
                };
                var tag3 = new TagModel
                {
                    Name = "Bodybuilding",
                };
                var tag4 = new TagModel
                {
                    Name = "Crossfit",
                };
                var tag5 = new TagModel
                {
                    Name = "Weightlifting",
                };
                
                if(_tags.Find(_ => true).ToList().Count > 0)
                    return (BasicResultEnum.Success,"");
                
                await _tags.InsertOneAsync(tag1);
                await _tags.InsertOneAsync(tag2);
                await _tags.InsertOneAsync(tag3);
                await _tags.InsertOneAsync(tag4);
                await _tags.InsertOneAsync(tag5);

                return (BasicResultEnum.Success,"");
            }
            catch (Exception e)
            {
                return (BasicResultEnum.Error, e.Message);
            }
        }
    }
}

