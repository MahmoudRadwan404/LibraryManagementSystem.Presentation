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
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.HasKey(l => l.Id);
            builder.Property(l => l.BorrowedAt).IsRequired();
            builder.Property(l => l.DueDate).IsRequired();
            builder.Property(l => l.Fine).HasColumnType("decimal(10,2)");

            builder.Property(l => l.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.HasOne(l => l.Book)
                   .WithMany(b => b.Loans)
                   .HasForeignKey(l => l.BookId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Member)
                   .WithMany(m => m.Loans)
                   .HasForeignKey(l => l.MemberId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.ProcessedByUser)
                   .WithMany(u => u.ProcessedLoans)
                   .HasForeignKey(l => l.ProcessedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(l => new { l.BookId, l.Status });
        }
    }
}
