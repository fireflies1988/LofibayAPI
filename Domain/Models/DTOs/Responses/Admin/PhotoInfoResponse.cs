using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Admin
{
    public class PhotoInfoResponse
    {
        public int PhotoId { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsFeatured { get; set; }
        public bool HasSensitiveContent { get; set; }
        public int FacesDetected { get; set; }
        public int PhotoStateId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime? UploadedAt { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
