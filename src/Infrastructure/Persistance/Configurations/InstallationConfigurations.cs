using Domain.Installations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configurations;

public class InstallationConfigurations : IEntityTypeConfiguration<Installation>
{
    public void Configure(EntityTypeBuilder<Installation> builder)
    {
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.HasIndex(e => e.ShipsToId, "IX_Installations_ShipsToId");

        builder.HasMany(bc => bc.GetsShipmentsFrom)
                .WithOne(b => b.ShipsTo)
                .HasForeignKey(f => f.ShipsToId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
