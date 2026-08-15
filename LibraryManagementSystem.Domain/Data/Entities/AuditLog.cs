using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Domain.Data.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }

        public Guid PerformedByUserId { get; set; }
        public SystemUser PerformedByUser { get; set; } = null!;

        public string Action { get; set; } = null!;      // e.g. "PROCESS_LOAN", "SYSTEM_MARK_OVERDUE"
        public string EntityType { get; set; } = null!;
        public string EntityId { get; set; } = null!;
        public string? OldValue { get; set; } // jsonb
        public string? NewValue { get; set; } // jsonb
        public DateTime Timestamp { get; set; }
    }
}
