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

        public UnitOfWork(LofibayDbContext context)
        {
            _context = context;

            Photos = new PhotoRepository(_context);
            Users = new UserRepository(_context);
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
