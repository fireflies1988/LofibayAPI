using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public string? TokenHash { get; set; }
        public string? TokenSalt { get; set; }
        public DateTime ExpirationDate { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
