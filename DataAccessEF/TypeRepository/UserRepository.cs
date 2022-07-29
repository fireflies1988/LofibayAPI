using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessEF.TypeRepository
{
    class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(LofibayDbContext context) : base(context)
        {
        }
    }
}
