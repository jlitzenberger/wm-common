using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;

namespace WM.Common.Utils
{
    public class SmtpEmailer
    {
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
        public MailPriority Priority { get; set; }
        public List<Attachment> Attachments { get; set; }
        private readonly SmtpClient _client;

        public SmtpEmailer()
        {
            _client = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"], Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]));
        }

        public SmtpEmailer(string smtpHost, int smtpPort)
        {
            _client = new SmtpClient(smtpHost, smtpPort);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public void SendEmail()
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.Subject = Subject;
            mailMessage.IsBodyHtml = IsBodyHtml;
            mailMessage.Priority = Priority;
            mailMessage.Body = Body;
            mailMessage.From = new MailAddress(From);

            foreach (var to in To.Split(';'))
            {
                mailMessage.To.Add(to.Trim());
            }

            if (!string.IsNullOrWhiteSpace(CC))
            {
                foreach (var cc in CC.Split(';'))
                {
                    mailMessage.CC.Add(cc.Trim());
                }
            }

            if (Attachments != null && Attachments.Any())
            {
                foreach (var attachment in Attachments)
                {
                    mailMessage.Attachments.Add(attachment);
                }
            }

            _client.Send(mailMessage);
        }
    }
}
