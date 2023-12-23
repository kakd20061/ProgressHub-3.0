using System;
namespace ProgressHubApi.Enums
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

