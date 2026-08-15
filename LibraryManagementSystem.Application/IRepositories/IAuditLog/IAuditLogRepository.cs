using LibraryManagementSystem.Application.IRepositories.IGeneric;
using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IRepositories.IAuditLog
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        // convenience method so services don't build the entity by hand every time
        Task LogAsync(Guid performedByUserId, string action, string entityType, string entityId,
                      object? oldValue = null, object? newValue = null);
    }
}
