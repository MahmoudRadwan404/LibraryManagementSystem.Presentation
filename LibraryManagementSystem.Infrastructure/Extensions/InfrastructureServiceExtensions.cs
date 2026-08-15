using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.IAuditLog;
using LibraryManagementSystem.Application.IRepositories.IAuthor;
using LibraryManagementSystem.Application.IRepositories.IBook;
using LibraryManagementSystem.Application.IRepositories.ICategory;
using LibraryManagementSystem.Application.IRepositories.ILoan;
using LibraryManagementSystem.Application.IRepositories.IMember;
using LibraryManagementSystem.Application.IRepositories.IPublisher;
using LibraryManagementSystem.Application.IRepositories.IRefreshToken;
using LibraryManagementSystem.Application.IRepositories.ISystemuser;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IPublisherRepository, PublisherRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<ISystemUserRepository, SystemUserRepository>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();

            services.AddScoped<IPasswordHasher<SystemUser>, PasswordHasher<SystemUser>>();

            return services;
        }
    }
}
