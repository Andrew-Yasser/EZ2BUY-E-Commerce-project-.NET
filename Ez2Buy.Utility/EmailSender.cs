using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.Utility
{
    public class EmailSender : IEmailSender
    {
        public string SendGridKey { get; set; }
        public EmailSender(IConfiguration _config)
        {
            SendGridKey = _config.GetValue<string>("SendGrid:ApiKey");   //get the api key from appsettings.json and assign it to the SendGridKey
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //logic to send email using SendGrid
            var client = new SendGridClient(SendGridKey);  //create a new instance of SendGridClient with the api key

            var from = new EmailAddress("info@ez2buy.tech","Ez2Buy");  //create a new instance of EmailAddress with the sender email and name

            var to = new EmailAddress(email);  //create a new instance of EmailAddress with the receiver email

            //message
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);  //create a new instance of SendGridMessage with the sender email, receiver email, subject and message

            return client.SendEmailAsync(msg);  //send the email asynchronously
        }
    }
}
