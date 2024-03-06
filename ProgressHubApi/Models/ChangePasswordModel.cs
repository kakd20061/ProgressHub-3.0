namespace ProgressHubApi.Models;

public class ChangePasswordModel
{
    public string email { get; set; }
    public string password { get; set; }
}

public class ChangePasswordModelWithCurrentPassword : ChangePasswordModel
{
    public string currentPassword { get; set; }
}