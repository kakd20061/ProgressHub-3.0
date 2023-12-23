using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProgressHubApi.Models.DTOs;

public class VerificationCodeDto
{
    public string Email { get; set; }

    public int Code { get; set; }
}

