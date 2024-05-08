namespace ProgressHubApi.Models.AccountSettings;

public class ChangeAvatarModel
{
    public string Email { get; set; }
    public IFormFile? File { get; set; }
}