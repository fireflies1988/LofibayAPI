using DataAccessEF.TypeRepository;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessEF.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LofibayDbContext _context;
        private readonly ITokenService _tokenService;

        public UnitOfWork(LofibayDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;

            Photos = new PhotoRepository(_context);
            Users = new UserRepository(_context, _tokenService);
            Tags = new TagRepository(_context);
        }

        public IPhotoRepository Photos { get; }
        public IUserRepository Users { get; }
        public ITagRepository Tags { get; }

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
