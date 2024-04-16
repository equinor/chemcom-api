using Domain.Chemicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configurations;

public class ChemicalConfigurations : IEntityTypeConfiguration<Chemical>
{
    public void Configure(EntityTypeBuilder<Chemical> builder)
    {
        builder.HasIndex(e => e.ProposedByInstallationId, "IX_Chemicals_ProposedByInstallationId");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.HasOne(d => d.ProposedByInstallation).WithMany(p => p.Chemicals).HasForeignKey(d => d.ProposedByInstallationId);
    }
}
