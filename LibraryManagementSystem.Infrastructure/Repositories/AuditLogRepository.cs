using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.IAuditLog;
using LibraryManagementSystem.Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Repositories
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ApplicationDbContext context) : base(context) { }

        public async Task LogAsync(Guid performedByUserId, string action, string entityType,
                                    string entityId, object? oldValue = null, object? newValue = null)
        {
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                PerformedByUserId = performedByUserId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValue = oldValue is null ? null : JsonSerializer.Serialize(oldValue),
                NewValue = newValue is null ? null : JsonSerializer.Serialize(newValue),
                Timestamp = DateTime.UtcNow
            };
            await _context.Set<AuditLog>().AddAsync(log);
        }
    }
}
