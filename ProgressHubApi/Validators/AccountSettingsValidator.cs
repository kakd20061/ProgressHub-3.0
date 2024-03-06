using System;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;

namespace ProgressHubApi.Validators
{
	public interface IAccountSettingsValidator
	{
    }

    public class AccountSettingsValidator : IAccountSettingsValidator
    {
        private IMongoCollection<UserModel> _accounts;

        public AccountSettingsValidator(IMongoClient client)
        {
            var mongoDatabase = client.GetDatabase("ProgressHub");

            _accounts = mongoDatabase.GetCollection<UserModel>("Users");
        }

    }
}

