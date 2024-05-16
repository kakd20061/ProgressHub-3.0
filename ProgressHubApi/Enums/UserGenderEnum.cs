
namespace ProgressHubApi.Enums;

public enum UserGenderEnum
{
    PreferNotToSay = 0,
    Male = 1,
    Female = 2,
    Other = 3
}

public static class UserGenderEnumExtensions
{
    public static string ToFriendlyString(this UserGenderEnum me)
    {
        return me switch
        {
            UserGenderEnum.PreferNotToSay => "Prefer not to say",
            UserGenderEnum.Male => "Male",
            UserGenderEnum.Female => "Female",
            UserGenderEnum.Other => "Other",
            _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
        };
    }
}
            