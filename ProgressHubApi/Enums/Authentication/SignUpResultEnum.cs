using System;
namespace ProgressHubApi.Enums.Authentication
{
	public enum SignUpResultEnum
	{
		Success,
        EmailExists,
        NicknameExists,
        EmptyUser,
        WeakPassword,
        Error
    }
}

