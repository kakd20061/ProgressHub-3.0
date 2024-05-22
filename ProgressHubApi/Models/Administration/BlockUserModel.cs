namespace ProgressHubApi.Models.Administration;

public class BlockUserModel
{
    public string email { get; set; }
    
    public DateTime? blockExpirationDate { get; set; }
    
    public string token { get; set; }
}