using System;
namespace ProgressHubApi.Models.Token
{
    public class TokenModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    
    public class ExternalTokenModel : TokenModel
    {
        public bool HasPassword { get; set; }
    }
}

