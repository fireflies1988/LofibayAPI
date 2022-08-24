using Domain.Models.DTOs.Responses.Users;

namespace Domain.Models.DTOs.Responses.Photos
{
    public class ViewYourLikedPhotosResponse
    {
        public int PhotoId { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsFeatured { get; set; }
        public bool HasSensitiveContent { get; set; }

        public BasicUserInfoResponse? User { get; set; }
    }
}
