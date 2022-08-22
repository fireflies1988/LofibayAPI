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
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync();
    }
}
