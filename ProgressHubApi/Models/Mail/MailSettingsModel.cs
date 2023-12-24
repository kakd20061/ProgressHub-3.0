using System;
namespace ProgressHubApi.Models.Mail
{
	public class MailSettingsModel
	{
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}

