using Domain.Entities;
using Domain.Models.DTOs.Responses.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Photos
{
    public class ViewYourUploadedPhotosResponse
    {
        public int PhotoId { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsFeatured { get; set; }
        public bool HasSensitiveContent { get; set; }

        public BasicUserInfoResponse? User { get; set; }
    }
}
