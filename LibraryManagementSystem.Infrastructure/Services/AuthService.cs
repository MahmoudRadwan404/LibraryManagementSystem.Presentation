using LibraryManagementSystem.Application.DTOs.Auth;
using LibraryManagementSystem.Application.ErrorMessages;
using LibraryManagementSystem.Application.Errors.Exceptions;
using LibraryManagementSystem.Application.IRepositories.IAuth;
using LibraryManagementSystem.Application.IRepositories.IRefreshToken;
using LibraryManagementSystem.Application.IRepositories.ISystemuser;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.IAuth;
using LibraryManagementSystem.Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ISystemUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<SystemUser> _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ISystemUserRepository userRepo, IRefreshTokenRepository refreshTokenRepo,
                            IUnitOfWork unitOfWork, IPasswordHasher<SystemUser> passwordHasher,
                            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResultDto> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email)
                ?? throw new UnauthorizedException(ErrorMessages.InvalidCredentials);

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedException(ErrorMessages.InvalidCredentials);

            return await IssueTokensAsync(user);
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(tokenHash)
                ?? throw new UnauthorizedException(ErrorMessages.InvalidRefreshToken);

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException(ErrorMessages.RefreshTokenExpired);

            await _refreshTokenRepo.RevokeAsync(storedToken.Id);

            var user = await _userRepo.GetByIdAsync(storedToken.UserId)
                ?? throw new NotFoundException(ErrorMessages.UserNotFound);

            return await IssueTokensAsync(user);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(tokenHash);
            if (storedToken is not null)
            {
                await _refreshTokenRepo.RevokeAsync(storedToken.Id);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task<AuthResultDto> IssueTokensAsync(SystemUser user)
        {
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = HashToken(refreshTokenValue),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };
            await _refreshTokenRepo.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
