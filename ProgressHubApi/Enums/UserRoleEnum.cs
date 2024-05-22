namespace ProgressHubApi.Enums
{
    public enum UserRoleEnum
    {
        Admin,
        User,
        Owner
    }

    public static class UserRoleEnumExtensions
    {
        public static string ToFriendlyString(this UserRoleEnum me)
        {
            return me switch
            {
                UserRoleEnum.Admin => "Admin",
                UserRoleEnum.User => "User",
                UserRoleEnum.Owner => "Owner",
                _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
            };
        }
    }
}
