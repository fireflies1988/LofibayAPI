using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessEF.TypeRepositories
{
    public class CollectionRepository : GenericRepository<Collection>, ICollectionRepository
    {
        public CollectionRepository(LofibayDbContext context) : base(context)
        {
        }

        public async Task<Collection?> GetCollectionIncludingPhotosByIdAsync(int collectionId)
        {
            return await Context.Collections!
                .Include(c => c.PhotoCollections)!
                    .ThenInclude(pc => pc.Photo)
                        .ThenInclude(p => p.PhotoTags)
                .FirstOrDefaultAsync(c => c.CollectionId == collectionId && c.IsPrivate == false);
        }

        public async Task<Collection?> GetCollectionIncludingPhotosByIdAsync(int collectionId, int userId)
        {
            return await Context.Collections!
                .Include(c => c.PhotoCollections)!
                    .ThenInclude(pc => pc.Photo)
                        .ThenInclude(p => p.PhotoTags)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CollectionId == collectionId && c.IsPrivate == false);
        }
    }
}
