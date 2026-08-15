using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Domain.Data.Entities
{
    public class Author
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; } // jsonb
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Book> ?Books { get; set; } = new List<Book>();
    }
}
