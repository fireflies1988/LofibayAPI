using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Message { get; set; }
        public bool Read { get; set; } = false;
        public DateTime NotificationTime { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
