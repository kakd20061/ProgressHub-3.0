namespace ProgressHubApi.Enums
{
    public enum UserRoleEnum
    {
        Admin,
        User
    }

    public static class UserRoleEnumExtensions
    {
        public static string ToFriendlyString(this UserRoleEnum me)
        {
            return me switch
            {
                UserRoleEnum.Admin => "Admin",
                UserRoleEnum.User => "User",
                _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
            };
        }
    }
}
