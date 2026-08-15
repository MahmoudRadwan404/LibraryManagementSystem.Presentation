using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Domain.Data.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Isbn { get; set; }
        public int? PublishYear { get; set; }
        public string? Edition { get; set; }
        public string? Language { get; set; }
        public int? PageCount { get; set; }

        public Guid? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }

        public int Quantity { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!; // optimistic concurrency token

        public string? Metadata { get; set; } // jsonb
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Author>? Authors { get; set; } = new List<Author>();
        public ICollection<Category>? Categories { get; set; } = new List<Category>();
        public ICollection<Loan> ?Loans { get; set; } = new List<Loan>();
    }
}
