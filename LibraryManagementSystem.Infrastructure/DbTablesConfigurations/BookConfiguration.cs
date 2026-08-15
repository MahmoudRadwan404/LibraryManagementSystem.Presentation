using LibraryManagementSystem.Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Title).IsRequired().HasMaxLength(300);
            builder.Property(b => b.Isbn).HasMaxLength(20);
            builder.HasIndex(b => b.Isbn).IsUnique();

            builder.Property(b => b.Quantity).IsRequired();
            builder.Property(b => b.Metadata).HasColumnType("nvarchar(max)");

            builder.Property(b => b.RowVersion).IsRowVersion();

            builder.HasOne(b => b.Publisher)
                   .WithMany(p => p.Books)
                   .HasForeignKey(b => b.PublisherId)
                   .OnDelete(DeleteBehavior.SetNull);

            // book-Author many-to-many table  
            builder.HasMany(b => b.Authors)
                   .WithMany(a => a.Books)
                   .UsingEntity(j => j.ToTable("Book_Author"));
            // book-category many-to-many table  

            builder.HasMany(b => b.Categories)
                   .WithMany(c => c.Books)
                   .UsingEntity(j => j.ToTable("Book_Category"));

            builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }
}
