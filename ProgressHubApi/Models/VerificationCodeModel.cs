using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProgressHubApi.Models;

public class VerificationCodeModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("code")]
    public int Code { get; set; }

    [BsonElement("createdAt")]
    public BsonDateTime CreatedAt { get; set; }

    public VerificationCodeModel(string _Id, string Email, int Code)
    {
        this._Id = _Id;
        this.Email = Email;
        this.Code = Code;
        this.CreatedAt = BsonDateTime.Create(DateTime.UtcNow);
    }
}

