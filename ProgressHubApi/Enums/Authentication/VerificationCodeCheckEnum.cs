using System;
namespace ProgressHubApi.Enums.Authentication
{
    public enum VerificationCodeCheckEnum
    {
        Success,
        InvalidCode,
        NoCode,
        NoAccount,
        Error
    }
}