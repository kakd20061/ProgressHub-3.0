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
        public Task<(BasicResultEnum, string)> SeedUsers();
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

        public async Task<(BasicResultEnum, string)> SeedTags()
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
                var tag6 = new TagModel
                {
                    Name = "Olympic Weightlifting",
                };
                var tag7 = new TagModel
                {
                    Name = "Calisthenics",
                };
                var tag8 = new TagModel
                {
                    Name = "Gymnastics",
                };
                var tag9 = new TagModel
                {
                    Name = "Yoga",
                };
                var tag10 = new TagModel
                {
                    Name = "Pilates",
                };
                var tag11 = new TagModel
                {
                    Name = "Running",
                };
                var tag12 = new TagModel
                {
                    Name = "Cycling",
                };
                var tag13 = new TagModel
                {
                    Name = "Swimming",
                };
                var tag14 = new TagModel
                {
                    Name = "Rowing",
                };
                var tag15 = new TagModel
                {
                    Name = "Boxing",
                };
                var tag16 = new TagModel
                {
                    Name = "MMA",
                };
                var tag17 = new TagModel
                {
                    Name = "Kickboxing",
                };
                var tag18 = new TagModel
                {
                    Name = "Muay Thai",
                };
                var tag19 = new TagModel
                {
                    Name = "Jiu Jitsu",
                };
                var tag20 = new TagModel
                {
                    Name = "Karate",
                };
                var tag21 = new TagModel
                {
                    Name = "Taekwondo",
                };
                var tag22 = new TagModel
                {
                    Name = "Judo",
                };
                var tag23 = new TagModel
                {
                    Name = "Wrestling",
                };
                var tag24 = new TagModel
                {
                    Name = "Aikido",
                };
                var tag25 = new TagModel
                {
                    Name = "Kendo",
                };
                var tag26 = new TagModel
                {
                    Name = "Fencing",
                };
                var tag27 = new TagModel
                {
                    Name = "Archery",
                };
                var tag28 = new TagModel
                {
                    Name = "Arm wrestling",
                };



                if (_tags.Find(_ => true).ToList().Count > 0)
                    return (BasicResultEnum.Success, "");

                await _tags.InsertOneAsync(tag1);
                await _tags.InsertOneAsync(tag2);
                await _tags.InsertOneAsync(tag3);
                await _tags.InsertOneAsync(tag4);
                await _tags.InsertOneAsync(tag5);
                await _tags.InsertOneAsync(tag6);
                await _tags.InsertOneAsync(tag7);
                await _tags.InsertOneAsync(tag8);
                await _tags.InsertOneAsync(tag9);
                await _tags.InsertOneAsync(tag10);
                await _tags.InsertOneAsync(tag11);
                await _tags.InsertOneAsync(tag12);
                await _tags.InsertOneAsync(tag13);
                await _tags.InsertOneAsync(tag14);
                await _tags.InsertOneAsync(tag15);
                await _tags.InsertOneAsync(tag16);
                await _tags.InsertOneAsync(tag17);
                await _tags.InsertOneAsync(tag18);
                await _tags.InsertOneAsync(tag19);
                await _tags.InsertOneAsync(tag20);
                await _tags.InsertOneAsync(tag21);
                await _tags.InsertOneAsync(tag22);
                await _tags.InsertOneAsync(tag23);
                await _tags.InsertOneAsync(tag24);
                await _tags.InsertOneAsync(tag25);
                await _tags.InsertOneAsync(tag26);
                await _tags.InsertOneAsync(tag27);
                await _tags.InsertOneAsync(tag28);

                return (BasicResultEnum.Success, "");
            }
            catch (Exception e)
            {
                return (BasicResultEnum.Error, e.Message);
            }
        }

        public async Task<(BasicResultEnum, string)> SeedUsers()
        {
            try
            {
                //Password: Test123!
                var owner = new UserModel(default, "Kajetan", "Dąbrowski", "kajetan.dabrowski@vp.pl", "kakd20061",
                    "D787A3C0317FD70DE01DA92B71D1808BF3D4AE75FF6693DAE9289B5FE9997D24BF2F1D4810526B9F12EA38FFE7FD526B53811BF8B9DF567C2AC9FA177A09B0D8",
                    null);

                if (_accounts.Find(_ => true).ToList().Any(n => n.Email == owner.Email))
                    return (BasicResultEnum.Success, "");
                
                owner.Role = UserRoleEnum.Owner;
                await _accounts.InsertOneAsync(owner);
                
                return (BasicResultEnum.Success, "");
            }
            catch (Exception e)
            {
                return (BasicResultEnum.Error, e.Message);
            }
        }
    }
}

