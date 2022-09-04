using Domain.Entities;
using Domain.Models.DTOs.Responses.Photos;
using Domain.Models.DTOs.Responses.Users;

namespace Domain.Models.DTOs.Responses.Collections
{
    public class ViewUserCollectionsResponse
    {
        public int CollectionId { get; set; }
        public string? CollectionName { get; set; }
        public string? Description { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPrivate { get; set; }
        public int Views { get; set; }
        public int NumOfPhotos { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public IList<BasicPhotoInfoResponse?>? Thumbnails { get; set; }

        public BasicUserInfoResponse? User { get; set; }

        public IList<PhotoCollectionResponse>? PhotoCollections { get; set; }
    }
}
