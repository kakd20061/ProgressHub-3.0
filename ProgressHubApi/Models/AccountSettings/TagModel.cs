using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProgressHubApi.Models.DTOs;

namespace ProgressHubApi.Models.AccountSettings;

public class TagModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }
    
    public TagDto ChangeToDto()
    {
        return new TagDto
        {
            _Id = _Id,
            Name = Name
        };
    }
}