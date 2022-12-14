using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Requests.Users
{
    public class UpdateUserAddressRequest
    {
        public string? Address { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
    }
}
