using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProgressHubApi.Models.AccountSettings;

namespace ProgressHubApi.Models.DTOs;

public class UserDto
{
    public string Name { get; set; }

    public string Lastname { get; set; }

    public string Email { get; set; }

    public string NickName { get; set; }

    public string Password { get; set; }
    public List<TagModel>? Tags { get; set; }
}

