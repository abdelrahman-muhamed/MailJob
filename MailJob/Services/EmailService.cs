using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MimeKit;

namespace MailJob.Services
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _approveLink;
        private readonly string _rejectLink;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
            _smtpUser = configuration["EmailSettings:SmtpUser"];
            _smtpPass = configuration["EmailSettings:SmtpPass"];
            _fromEmail = configuration["EmailSettings:FromEmail"];
            _fromName = configuration["EmailSettings:FromName"];
            _approveLink = configuration["EmailSettings:ApproveLink"];
            _rejectLink = configuration["EmailSettings:RejectLink"];
        }

        public async Task SendEmail(string toEmail, string subject, string htmlContent)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress("",toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlContent
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendApprovalRequest(string toEmail)
        {
            string subject = "Please Approve or Reject";

            string htmlContent = $@"
                <html>
                <body>
                    <p>Please respond:</p>
                    <a href='{_approveLink}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: white; background-color: green; text-align: center; text-decoration: none; border-radius: 5px;'>Approve</a>
                    <a href='{_rejectLink}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: white; background-color: red; text-align: center; text-decoration: none; border-radius: 5px;'>Reject</a>
                </body>
                </html>";

            await SendEmail(toEmail, subject, htmlContent);
        }
    }
}
