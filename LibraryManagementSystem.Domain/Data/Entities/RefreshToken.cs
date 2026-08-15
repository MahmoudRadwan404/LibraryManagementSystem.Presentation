using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Domain.Data.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public SystemUser User { get; set; } = null!;

        public string Token { get; set; } = null!;   // store a hash, not the raw value
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
