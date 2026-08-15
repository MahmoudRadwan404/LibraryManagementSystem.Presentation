using LibraryManagementSystem.Application.IRepositories.IGeneric;
using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IRepositories.IRefreshToken
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string tokenHash);
        Task RevokeAsync(Guid tokenId);
        Task RevokeAllForUserAsync(Guid userId);
    }
}
