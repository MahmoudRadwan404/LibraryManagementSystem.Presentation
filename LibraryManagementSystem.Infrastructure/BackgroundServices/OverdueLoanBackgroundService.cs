using LibraryManagementSystem.Application.Actions;
using LibraryManagementSystem.Application.IRepositories.IAuditLog;
using LibraryManagementSystem.Application.IRepositories.ILoan;
using LibraryManagementSystem.Application.IRepositories.ISystemuser;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Domain.Data.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.BackgroundServices
{
    public class OverdueLoanBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OverdueLoanBackgroundService> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

        public OverdueLoanBackgroundService(IServiceScopeFactory scopeFactory, ILogger<OverdueLoanBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MarkOverdueLoansAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to run overdue-loan check");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task MarkOverdueLoansAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var loanRepo = scope.ServiceProvider.GetRequiredService<ILoanRepository>();
            var auditLogRepo = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();
            var userRepo = scope.ServiceProvider.GetRequiredService<ISystemUserRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var overdueLoans = await loanRepo.GetOverdueLoansAsync();
            if (!overdueLoans.Any())
                return;

            // "System actor" = first Administrator account, per your earlier decision
            var systemUsers = await userRepo.GetAllAsync();
            var systemActorId = systemUsers.First(u => u.RoleType == RoleType.Administrator).Id;

            foreach (var loan in overdueLoans)
            {
                loan.Status = LoanStatus.Overdue;
                loan.UpdatedAt = DateTime.UtcNow;
                loanRepo.Update(loan);

                await auditLogRepo.LogAsync(
                    systemActorId,
                    AuditActions.SystemMarkOverdue,
                    nameof(Loan),
                    loan.Id.ToString(),
                    new { Status = "Active" },
                    new { Status = "Overdue" });
            }

            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Marked {Count} loan(s) as Overdue", overdueLoans.Count());
        }
    }
}
