using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IRepositories.IAuth
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(SystemUser user);
        string GenerateRefreshToken();
    }
}
