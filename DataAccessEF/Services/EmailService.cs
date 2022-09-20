using Common.Helpers;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Emails;
using Domain.Models.ResponseTypes;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;

namespace DataAccessEF.Services
{
    public class EmailService : IEmailService
    {
        public async Task<BaseResponse<object>> SendVerificationCode(VerificationEmailRequest verificationEmailRequest)
        {
            try
            {
                MailjetClient client = new MailjetClient(
                    Environment.GetEnvironmentVariable("MAILJET_API_KEY"),
                    Environment.GetEnvironmentVariable("MAILJET_SECRET_KEY"));

                // construct your email with builder
                var email = new TransactionalEmailBuilder()
                       .WithFrom(new SendContact(Environment.GetEnvironmentVariable("MAILJET_EMAIL"), "Lofibay"))
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
                    Environment.GetEnvironmentVariable("MAILJET_API_KEY"),
                    Environment.GetEnvironmentVariable("MAILJET_SECRET_KEY"));

                // construct your email with builder
                var email = new TransactionalEmailBuilder()
                       .WithFrom(new SendContact(Environment.GetEnvironmentVariable("MAILJET_EMAIL"), "Lofibay"))
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
