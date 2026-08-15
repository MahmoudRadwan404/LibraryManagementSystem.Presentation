using LibraryManagementSystem.Application.DTOs.SystemUser;
using LibraryManagementSystem.Application.ErrorMessages;
using LibraryManagementSystem.Application.Errors.Exceptions;
using LibraryManagementSystem.Application.IRepositories.ISystemuser;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.ISystemUser;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Domain.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Services
{
    public class SystemUserService : ISystemUserService
    {
        private readonly ISystemUserRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<SystemUser> _passwordHasher;

        public SystemUserService(ISystemUserRepository repo, IUnitOfWork unitOfWork,
                                  IPasswordHasher<SystemUser> passwordHasher)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<SystemUserDto?> GetByIdAsync(Guid id)
        {
            var u = await _repo.GetByIdAsync(id);
            return u is null ? null : MapToDto(u);
        }

        public async Task<IEnumerable<SystemUserDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<SystemUserDto> CreateAsync(CreateSystemUserDto dto)
        {
            var existing = await _repo.GetByEmailAsync(dto.Email);
            if (existing is not null)
                throw new ConflictException(ErrorMessages.EmailAlreadyExists);

            var user = new SystemUser
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                RoleType = Enum.Parse<RoleType>(dto.RoleType),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _repo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(user);
        }

        public async Task UpdateAsync(Guid id, UpdateSystemUserDto dto)
        {
            var user = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException(ErrorMessages.UserNotFound);

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.RoleType = Enum.Parse<RoleType>(dto.RoleType);
            user.UpdatedAt = DateTime.UtcNow;

            _repo.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException(ErrorMessages.UserNotFound);

            user.IsDeleted = true;
            _repo.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        private static SystemUserDto MapToDto(SystemUser u) => new()
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            RoleType = u.RoleType.ToString()
        };
    }
}
