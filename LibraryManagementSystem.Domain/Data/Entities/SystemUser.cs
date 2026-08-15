using LibraryManagementSystem.Domain.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Domain.Data.Entities
{
    public class SystemUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public RoleType RoleType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Loan> ?ProcessedLoans { get; set; } = new List<Loan>();
        public ICollection<AuditLog>? AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<RefreshToken> ?RefreshTokens { get; set; } = new List<RefreshToken>();
    }

}
