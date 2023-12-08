using Domain.ShipmentChemicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configurations;

public class ShipmentChemicalConfiguration : IEntityTypeConfiguration<ShipmentChemical>
{
    public void Configure(EntityTypeBuilder<ShipmentChemical> builder)
    {
        builder.HasIndex(e => e.ChemicalId, "IX_ShipmentChemicals_ChemicalId");

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasOne(bc => bc.Chemical)
                .WithMany(b => b.ShipmentChemicals)
                .HasForeignKey(bc => bc.ChemicalId);

        builder.HasOne(bc => bc.Shipment)
             .WithMany(b => b.Chemicals)
             .HasForeignKey(bc => bc.ShipmentId)
             .OnDelete(DeleteBehavior.Restrict);
    }
}
