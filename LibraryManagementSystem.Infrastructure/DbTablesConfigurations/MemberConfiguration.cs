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
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Email).HasMaxLength(256);
            builder.HasIndex(m => m.Email).IsUnique();

            builder.Property(m => m.MembershipStatus)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}
