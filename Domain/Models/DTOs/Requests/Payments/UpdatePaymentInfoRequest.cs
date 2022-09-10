using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Requests.Payments
{
    public class UpdatePaymentInfoRequest
    {
        public IFormFile? MomoQRCodeImageFile { get; set; }
        public IFormFile? BankQRCodeImageFile { get; set; }
        public string? PaypalDonationLink { get; set; }
    }
}
