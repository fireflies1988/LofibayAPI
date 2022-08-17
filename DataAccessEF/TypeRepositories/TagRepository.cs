using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace DataAccessEF.TypeRepositories
{
    class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(LofibayDbContext context) : base(context)
        {
        }
    }
}
