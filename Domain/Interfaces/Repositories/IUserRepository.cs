using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
    }
}
