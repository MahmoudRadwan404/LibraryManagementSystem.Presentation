using LibraryManagementSystem.Application.DTOs.Statistics;
using LibraryManagementSystem.Application.IRepositories.IBook;
using LibraryManagementSystem.Application.IRepositories.ILoan;
using LibraryManagementSystem.Application.IRepositories.IMember;
using LibraryManagementSystem.Application.IServices.IStatistics;
using LibraryManagementSystem.Domain.Data.Enums;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IBookRepository _bookRepo;
        private readonly ILoanRepository _loanRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IMemoryCache _cache;

        public StatisticsService(IBookRepository bookRepo, ILoanRepository loanRepo,
                                  IMemberRepository memberRepo, IMemoryCache cache)
        {
            _bookRepo = bookRepo;
            _loanRepo = loanRepo;
            _memberRepo = memberRepo;
            _cache = cache;
        }

        public async Task<StatisticsDto> GetDashboardStatsAsync()
        {
            if (_cache.TryGetValue("dashboard-stats", out StatisticsDto? cached))
                return cached!;

            var books = await _bookRepo.GetAllAsync();
            var activeLoans = await _loanRepo.GetActiveLoansAsync();
            var overdueLoans = await _loanRepo.GetOverdueLoansAsync();
            var members = await _memberRepo.GetAllAsync();

            var stats = new StatisticsDto
            {
                TotalBooks = books.Count(),
                TotalCopies = books.Sum(b => b.Quantity),
                CurrentlyBorrowed = activeLoans.Count(),
                OverdueCount = overdueLoans.Count(),
                ActiveMembersCount = members.Count(m => m.MembershipStatus == MembershipStatus.Active),
                MostBorrowedBooks = activeLoans
                    .GroupBy(l => l.BookId)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new PopularBookDto
                    {
                        BookId = g.Key,
                        Title = g.First().Book?.Title ?? string.Empty,
                        BorrowCount = g.Count()
                    }).ToList()
            };

            _cache.Set("dashboard-stats", stats, TimeSpan.FromMinutes(5));
            return stats;
        }
    }
}
