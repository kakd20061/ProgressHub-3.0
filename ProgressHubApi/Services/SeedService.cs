using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Amazon.Auth.AccessControlPolicy;
using MongoDB.Driver;
using ProgressHubApi.Enums;
using ProgressHubApi.Enums.Authentication;
using ProgressHubApi.Models;
using ProgressHubApi.Models.DTOs;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Repositories;

namespace ProgressHubApi.Services
{
	public interface ISeedService
    {
        public Task<(BasicResultEnum,string)> SeedTags();
    }

    public class SeedService : ISeedService
    {
        private readonly ISeedRepository _repository;

        public SeedService(ISeedRepository repository)
        {
            _repository = repository;
        }

        public Task<(BasicResultEnum,string)> SeedTags()
        {
            return _repository.SeedTags();
        }
    }
}

