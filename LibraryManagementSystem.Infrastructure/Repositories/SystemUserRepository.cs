using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.ISystemuser;
using LibraryManagementSystem.Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Repositories
{
    public class SystemUserRepository : GenericRepository<SystemUser>, ISystemUserRepository
    {
        public SystemUserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<SystemUser?> GetByEmailAsync(string email) =>
            await _context.Set<SystemUser>().SingleOrDefaultAsync(u => u.Email == email);
    }
}
