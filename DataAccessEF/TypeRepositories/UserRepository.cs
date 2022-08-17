using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace DataAccessEF.TypeRepositories
{
    class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(LofibayDbContext context) : base(context)
        {
        }
    }
}
