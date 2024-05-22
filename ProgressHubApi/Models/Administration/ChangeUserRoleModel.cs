using ProgressHubApi.Enums;

namespace ProgressHubApi.Models.Administration;

public class ChangeUserRoleModel
{
    public string email { get; set; }
    public string role { get; set; }
}