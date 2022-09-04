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

        public async Task<Collection?> GetPhotosOfCollection(int collectionId)
        {
            return await Context.Collections!
                .Include(c => c.PhotoCollections)!
                    .ThenInclude(pc => pc.Photo)
                        .ThenInclude(p => p.LikedPhotos)
                .Include(c => c.PhotoCollections)!
                    .ThenInclude(pc => pc.Photo)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.CollectionId == collectionId);
        }

        // not used
        public async Task<Collection?> GetCollectionIncludingPhotosByIdAsync(int collectionId, int userId)
        {
            return await Context.Collections!
                .Include(c => c.PhotoCollections)!
                    .ThenInclude(pc => pc.Photo)
                        .ThenInclude(p => p.PhotoTags)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CollectionId == collectionId && c.IsPrivate == false);
        }

        public async Task<IEnumerable<Collection>> GetUserCollectionsAsync(int userId)
        {
            var collections = await Context.Collections!
                .Include(c => c.PhotoCollections)!
                    .ThenInclude(pc => pc.Photo)
                .Include(c => c.User)
                .Where(c => c.UserId == userId).ToListAsync();
            Random random = new Random();
            collections.ForEach(c => c.PhotoCollections?.OrderBy(pc => random.Next())?.Take(3));
            return collections;
        }

        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync()
        {
            var collections = await Context.Collections!
                .Include(c => c.PhotoCollections)!
                    .ThenInclude(pc => pc.Photo)
                .Include(c => c.User)
                .Where(c => c.IsPrivate == false).ToListAsync();
            return collections;
        }
    }
}
