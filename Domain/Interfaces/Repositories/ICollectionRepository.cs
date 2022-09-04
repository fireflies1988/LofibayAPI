using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ICollectionRepository : IGenericRepository<Collection>
    {
        Task<Collection?> GetPhotosOfCollection(int collectionId);
        Task<Collection?> GetCollectionIncludingPhotosByIdAsync(int collectionId, int userId);
        Task<IEnumerable<Collection>> GetUserCollectionsAsync(int userId);
        Task<IEnumerable<Collection>> GetAllCollectionsAsync();
    }
}
