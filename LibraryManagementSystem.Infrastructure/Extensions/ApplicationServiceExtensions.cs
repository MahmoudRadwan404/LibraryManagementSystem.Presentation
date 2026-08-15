using LibraryManagementSystem.Application.IServices.IAuth;
using LibraryManagementSystem.Application.IServices.IAuthor;
using LibraryManagementSystem.Application.IServices.Ibook;
using LibraryManagementSystem.Application.IServices.ICategory;
using LibraryManagementSystem.Application.IServices.ILoan;
using LibraryManagementSystem.Application.IServices.IMember;
using LibraryManagementSystem.Application.IServices.IPublisher;
using LibraryManagementSystem.Application.IServices.IStatistics;
using LibraryManagementSystem.Application.IServices.ISystemUser;
using LibraryManagementSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IPublisherService, PublisherService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<ISystemUserService, SystemUserService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStatisticsService, StatisticsService>();

            return services;
        }
    }
}
