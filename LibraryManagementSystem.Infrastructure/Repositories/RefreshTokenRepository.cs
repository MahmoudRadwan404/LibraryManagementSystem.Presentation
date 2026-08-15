using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.IRefreshToken;
using LibraryManagementSystem.Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string tokenHash) =>
            await _context.Set<RefreshToken>().SingleOrDefaultAsync(r => r.Token == tokenHash && !r.IsRevoked);

        public async Task RevokeAsync(Guid tokenId)
        {
            var token = await _context.Set<RefreshToken>().FindAsync(tokenId);
            if (token is not null) token.IsRevoked = true;
        }

        public async Task RevokeAllForUserAsync(Guid userId)
        {
            var tokens = await _context.Set<RefreshToken>().Where(r => r.UserId == userId && !r.IsRevoked).ToListAsync();
            foreach (var t in tokens) t.IsRevoked = true;
        }
    }
}
