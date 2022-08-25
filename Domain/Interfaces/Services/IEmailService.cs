using Domain.Models.DTOs.Requests.Emails;
using Domain.Models.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task<BaseResponse<object>> SendEmailAsync(EmailRequest emailRequest);
        Task<BaseResponse<object>> SendVerificationCode(VerificationEmailRequest verificationEmailRequest);
    }
}
