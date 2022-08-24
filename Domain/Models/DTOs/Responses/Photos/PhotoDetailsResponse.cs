using Domain.Entities;
using Domain.Models.DTOs.Responses.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Photos
{
    public class PhotoDetailsResponse
    {
        public int PhotoId { get; set; }
        public string? PublicId { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? TakenAt { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long FileSize { get; set; }
        public string? Format { get; set; }
        public string? Camera { get; set; }
        public string? Software { get; set; }
        public int FacesDetected { get; set; }
        public string? Phash { get; set; }
        public bool SemiTransparent { get; set; }
        public bool Grayscale { get; set; }
        public bool IsFeatured { get; set; }
        public bool HasSensitiveContent { get; set; }
        public long Views { get; set; }
        public long Downloads { get; set; }
        public int Likes { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime ModifiedDate { get; set; }

        public UserInfoResponse? User { get; set; }

        public IList<PhotoTag>? PhotoTags { get; set; }

        public IList<PhotoColor>? PhotoColors { get; set; }
    }
}
