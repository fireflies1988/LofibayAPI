using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPhotoRepository Photos { get; }
        IUserRepository Users { get; }
        ITagRepository Tags { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IColorRepository Colors { get; }
        ICollectionRepository Collections { get; }
        ILikedPhotoRepository LikedPhotos { get; }
        INotificationRepository Notifications { get; }
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync();
    }
}
