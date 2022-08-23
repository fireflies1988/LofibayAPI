using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ICollectionRepository : IGenericRepository<Collection>
    {
        Task<Collection?> GetCollectionIncludingPhotosByIdAsync(int collectionId);
        Task<Collection?> GetCollectionIncludingPhotosByIdAsync(int collectionId, int userId);
    }
}
