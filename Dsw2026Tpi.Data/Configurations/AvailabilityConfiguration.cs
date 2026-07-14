using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Data.Configuration
{
    public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
    {
        public void Configure(EntityTypeBuilder<Availability> builder)
        {
            builder.ToTable("AvailabilityRules");
            builder.HasIndex(a => new 
            { 
                a.DoctorId,
                a.Year, 
                a.Month,
                a.DayOfWeek,
                a.StartTime,
                a.EndTime 
            }).IsUnique();
            builder.Property(a => a.Month).IsRequired();
            builder.Property(a => a.Year).IsRequired();
            builder.Property(a => a.DayOfWeek).IsRequired();
            builder.Property(a => a.StartTime).IsRequired();
            builder.Property(a => a.EndTime).IsRequired();
        }
    }
}
