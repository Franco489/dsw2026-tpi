using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Data.Configuration
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");
            builder.HasIndex(p => p.Dni).IsUnique();
            builder.Property(p => p.Email).HasMaxLength(150);
            builder.Property(p => p.Dni).HasMaxLength(10);
            builder.Property(p => p.Name).HasMaxLength(150);
            builder.Property(p => p.PhoneNumber).HasMaxLength(20);
        }
    }
}
