using DataAccessEF.TypeRepositories;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataAccessEF.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LofibayDbContext _context;

        public UnitOfWork(LofibayDbContext context)
        {
            _context = context;

            Photos = new PhotoRepository(_context);
            Users = new UserRepository(_context);
            Tags = new TagRepository(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
            Colors = new ColorRepository(_context);
            Collections = new CollectionRepository(_context);
            LikedPhotos = new LikedPhotoRepository(_context);
        }

        public IPhotoRepository Photos { get; }
        public IUserRepository Users { get; }
        public ITagRepository Tags { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IColorRepository Colors { get; }
        public ICollectionRepository Collections { get; }
        public ILikedPhotoRepository LikedPhotos { get; }
        public DatabaseFacade Database => _context.Database;

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
