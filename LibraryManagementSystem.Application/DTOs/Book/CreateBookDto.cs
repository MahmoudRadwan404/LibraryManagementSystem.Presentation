using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.DTOs.Book
{
    public class CreateBookDto
    {
        public string Title { get; set; } = null!;
        public string? Isbn { get; set; }
        public int? PublishYear { get; set; }
        public string? Edition { get; set; }
        public string? Language { get; set; }
        public int? PageCount { get; set; }
        public Guid? PublisherId { get; set; }
        public int Quantity { get; set; }
        public JsonElement? Metadata { get; set; }
        public List<Guid> AuthorIds { get; set; } = new();
        public List<Guid> CategoryIds { get; set; } = new();
    }
}
