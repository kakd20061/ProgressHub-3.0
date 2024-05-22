using ProgressHubApi.Enums;

namespace ProgressHubApi.Models.AccountSettings;

public class PersonalDataChangeModel
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Nickname { get; set; }
    public string? DateOfBirth { get; set; }
    public string Gender { get; set; }
    
    public string Token { get; set; }
}