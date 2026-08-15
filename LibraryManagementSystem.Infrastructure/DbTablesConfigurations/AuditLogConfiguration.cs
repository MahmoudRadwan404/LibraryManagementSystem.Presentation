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
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
            builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
            builder.Property(a => a.EntityId).IsRequired().HasMaxLength(100);
            builder.Property(a => a.OldValue).HasColumnType("nvarchar(max)");
            builder.Property(a => a.NewValue).HasColumnType("nvarchar(max)");
            builder.Property(a => a.Timestamp).IsRequired();

            builder.HasOne(a => a.PerformedByUser)
                   .WithMany(u => u.AuditLogs)
                   .HasForeignKey(a => a.PerformedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.Timestamp);
        }
    }
}
