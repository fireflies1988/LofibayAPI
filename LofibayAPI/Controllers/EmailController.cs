using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Emails;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromForm]EmailRequest emailRequest)
        {
            try
            {
                await _emailService.SendEmailAsync(emailRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.ToString() });
            }
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode(VerificationEmailRequest verificationEmailRequest)
        {
            try
            {
                await _emailService.SendVerificationCode(verificationEmailRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.ToString() });
            }
        }
    }
}
