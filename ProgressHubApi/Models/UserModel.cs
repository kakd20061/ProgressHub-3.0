using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProgressHubApi.Models;

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("lastname")]
    public string LastName { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("nickname")]
    public string Nickname { get; set; }

    [BsonElement("password")]
    public string? Password { get; set; }

    [BsonElement("activity")]
    public UserActivity Activity { get; set; }

    [BsonElement("tokens")]
    public Tokens Tokens { get; set; }

    public UserModel(string _Id, string Name, string LastName, string Email, string Nickname, string? Password, DateTime? LastLoggedAt)
    {
        this._Id = _Id;
        this.Name = Name;
        this.LastName = LastName;
        this.Email = Email;
        this.Nickname = Nickname;
        this.Password = Password;
        Activity = new UserActivity
        {
            CreatedAt = DateTime.UtcNow,
            LastLoggedAt = LastLoggedAt == null ? DateTime.UtcNow : LastLoggedAt.Value
        };
        Tokens = new Tokens
        {
            RefreshToken = "",
            RefreshTokenExpiryTime = DateTime.Now
        };
    }

    public UserModel(UserModel model, DateTime LastLoggedAt)
    {
        _Id = model._Id;
        Name = model.Name;
        LastName = model.LastName;
        Email = model.Email;
        Nickname = model.Nickname;
        Password = model.Password;
        Activity = new UserActivity
        {
            CreatedAt = model.Activity.CreatedAt,
            LastLoggedAt = LastLoggedAt
        };
        Tokens = new Tokens
        {
            RefreshToken = "",
            RefreshTokenExpiryTime = DateTime.Now
        };
    }
}

public class UserActivity
{
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("lastLoggedAt")]
    public DateTime LastLoggedAt { get; set; }
}

public class Tokens
{
    [BsonElement("refreshToken")]
    public string? RefreshToken { get; set; }

    [BsonElement("refreshTokenExpiryTime")]
    public DateTime RefreshTokenExpiryTime { get; set; }
}

