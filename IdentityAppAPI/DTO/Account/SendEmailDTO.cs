﻿namespace IdentityAppAPI.DTO.Account
{
    public class SendEmailDTO
    {
       
        public SendEmailDTO(string to, string subject , string body)
        {
            To = to;
            Subject=subject;
            Body=body;
        }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
