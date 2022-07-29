using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserAddress
    {
        public int UserAddressId { get; set; }
        public string? Address { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
