using LibraryManagementSystem.Application.DTOs.SystemUser;
using LibraryManagementSystem.Application.IServices.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IServices.ISystemUser
{
    public interface ISystemUserService : IGenericService<SystemUserDto, CreateSystemUserDto, UpdateSystemUserDto> { }
}
