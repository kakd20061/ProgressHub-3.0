using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProgressHubApi.Enums;
using ProgressHubApi.Models.AccountSettings;

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
    
    [BsonElement("role")]
    public UserRoleEnum Role { get; set; }
    
    [BsonElement("tags")]
    public List<TagModel>? Tags { get; set; }
    
    [BsonElement("avatar")]
    public string Avatar { get; set; }
    
    [BsonElement("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }
    
    [BsonElement("gender")]
    public UserGenderEnum Gender { get; set; }
    
    [BsonElement("bodyParameters")]
    public BodyParameters BodyParameters { get; set; }

    public UserModel(string _Id, string Name, string LastName, string Email, string Nickname, string? Password, DateTime? LastLoggedAt, string Avatar = "")
    {
        this._Id = _Id;
        this.Name = Name;
        this.LastName = LastName;
        this.Email = Email;
        this.Nickname = Nickname;
        this.Password = Password;
        Tags = new List<TagModel>();
        Role = UserRoleEnum.User;
        this.Avatar = Avatar;
        DateOfBirth = null;
        Gender = UserGenderEnum.PreferNotToSay;
        Activity = new UserActivity
        {
            CreatedAt = DateTime.UtcNow,
            LastLoggedAt = LastLoggedAt ?? DateTime.UtcNow
        };
        Tokens = new Tokens
        {
            RefreshToken = "",
            RefreshTokenExpiryTime = DateTime.UtcNow
        };
        BodyParameters = new BodyParameters
        {
            Weight = "",
            Height = "",
            BodyFatPercentage = ""
        };
    }

    public UserModel(UserModel model, DateTime lastLoggedAt)
    {
        _Id = model._Id;
        Name = model.Name;
        LastName = model.LastName;
        Email = model.Email;
        Nickname = model.Nickname;
        Password = model.Password;
        Role = model.Role;
        Tags = model.Tags;
        Avatar = model.Avatar;
        DateOfBirth = model.DateOfBirth;
        Gender = model.Gender;
        Activity = new UserActivity
        {
            CreatedAt = model.Activity.CreatedAt,
            LastLoggedAt = lastLoggedAt
        };
        Tokens = new Tokens
        {
            RefreshToken = "",
            RefreshTokenExpiryTime = DateTime.UtcNow
        };
        BodyParameters = model.BodyParameters;
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

public class BodyParameters
{
    [BsonElement("weight")] 
    public string Weight { get; set; }

    [BsonElement("height")] 
    public string Height { get; set; }
    [BsonElement("bodyFatPercentage")]
    public string BodyFatPercentage { get; set; }
}

