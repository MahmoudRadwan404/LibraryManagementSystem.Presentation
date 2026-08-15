using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.DTOs.SystemUser
{
    public class SystemUserDto:UpdateSystemUserDto
    {
        public Guid Id { get; set; }
    }

    public class CreateSystemUserDto:UpdateSystemUserDto
    {      
        public string Password { get; set; } = null!;   // raw password in, hashed by the service before persisting
    }

    public class UpdateSystemUserDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleType { get; set; } = null!;
    }
}
