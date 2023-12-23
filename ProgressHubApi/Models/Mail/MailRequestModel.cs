﻿using System;
namespace ProgressHubApi.Models.Mail
{
	public class MailRequestModel
	{
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}

