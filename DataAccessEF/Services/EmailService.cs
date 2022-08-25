using Common.Helpers;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Emails;
using Domain.Models.ResponseTypes;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace DataAccessEF.Services
{
    public class EmailService : IEmailService
    {
        public async Task<BaseResponse<object>> SendVerificationCode(VerificationEmailRequest verificationEmailRequest)
        {
            try
            {
                MailjetClient client = new MailjetClient(
                ConfigurationHelper.Configuration!["Mailjet:ApiKey"],
                ConfigurationHelper.Configuration!["Mailjet:SecretKey"]);

                // construct your email with builder
                var email = new TransactionalEmailBuilder()
                       .WithFrom(new SendContact(ConfigurationHelper.Configuration!["Mailjet:Email"], "Lofibay"))
                       .WithSubject("Lofibay - Verify your account")
                       .WithHtmlPart(string.Format(EmailTemplateHelper.VerificationEmail, verificationEmailRequest.VerificationCode))
                       .WithTo(new SendContact(verificationEmailRequest.To))
                       .Build();

                // invoke API to send email
                var response = await client.SendTransactionalEmailAsync(email);
                return new SuccessResponse
                {
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponse { Message = ex.Message };
            }
        }

        public async Task<BaseResponse<object>> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                MailjetClient client = new MailjetClient(
                ConfigurationHelper.Configuration!["Mailjet:ApiKey"],
                ConfigurationHelper.Configuration!["Mailjet:SecretKey"]);

                // construct your email with builder
                var email = new TransactionalEmailBuilder()
                       .WithFrom(new SendContact(ConfigurationHelper.Configuration!["Mailjet:Email"], "Lofibay"))
                       .WithSubject(emailRequest.Subject)
                       .WithHtmlPart(emailRequest.Body)
                       .WithTo(new SendContact(emailRequest.To))
                       .Build();

                // invoke API to send email
                var response = await client.SendTransactionalEmailAsync(email);
                return new SuccessResponse { Data = response };
            }
            catch (Exception ex)
            {
                return new ErrorResponse { Message = ex.Message };
            }
        }
    }
}
