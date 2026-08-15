using LibraryManagementSystem.Application.IRepositories.IGeneric;
using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IRepositories.ISystemuser
{
    public interface ISystemUserRepository : IGenericRepository<SystemUser>
    {
        Task<SystemUser?> GetByEmailAsync(string email);
    }
}
