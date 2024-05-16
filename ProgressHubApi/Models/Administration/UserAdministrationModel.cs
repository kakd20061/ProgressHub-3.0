using ProgressHubApi.Enums;

namespace ProgressHubApi.Models.Administration;

public class UserAdministrationModel
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Nickname { get; set; }
    public string Role { get; set; }
    public string Avatar { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public BodyParameters BodyParameters { get; set; }

    public UserAdministrationModel(UserModel user)
    {
        this.Name = user.Name;
        this.LastName = user.LastName;
        this.Email = user.Email;
        this.Nickname = user.Nickname;
        this.Role = UserRoleEnumExtensions.ToFriendlyString(user.Role);
        this.Avatar = user.Avatar;
        this.DateOfBirth = user.DateOfBirth;
        this.Gender = UserGenderEnumExtensions.ToFriendlyString(user.Gender);
        this.BodyParameters = user.BodyParameters;
    }
}