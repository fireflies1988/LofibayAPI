using Domain.Entities;
using Domain.Models.DTOs.Responses.Users;

namespace Domain.Models.DTOs.Responses.Photos
{
    public class BasicPhotoInfoResponse
    {
        public int PhotoId { get; set; }
        public string? PhotoUrl { get; set; }
        public string? DownloadUrl { get; set; }
        public bool IsFeatured { get; set; }
        public bool HasSensitiveContent { get; set; }

        public BasicUserInfoResponse? User { get; set; }

        public IList<LikedPhoto>? LikedPhotos { get; set; }
    }
}
