using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccessEF.TypeRepositories
{
    class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(LofibayDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserProfileDetails(int id)
        {
            return await Context.Users!
                .Include(u => u.Address)
                .Include(u => u.Gender)
                .Include(u => u.Photos)
                .Include(u => u.Collections)
                .Include(u => u.LikedPhotos)!
                    .ThenInclude(lp => lp.Photo)
                .FirstOrDefaultAsync(u => u.UserId == id && !u.DeletedDate.HasValue);
        }
    }
}
