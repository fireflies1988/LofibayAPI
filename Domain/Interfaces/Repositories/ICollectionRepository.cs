using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ICollectionRepository : IGenericRepository<Collection>
    {
        Task<Collection?> GetPhotosOfCollection(int collectionId);
        Task<Collection?> GetCollectionIncludingPhotosByIdAsync(int collectionId, int userId);
        Task<IEnumerable<Collection>> GetUserCollections(int userId);
    }
}
