using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessEF.TypeRepository
{
    class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(LofibayDbContext context) : base(context)
        {
        }
    }
}
