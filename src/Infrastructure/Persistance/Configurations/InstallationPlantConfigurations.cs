using Domain.InstallationPlants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configurations;

public class InstallationPlantConfigurations : IEntityTypeConfiguration<InstallationPlant>
{
    public void Configure(EntityTypeBuilder<InstallationPlant> builder)
    {
        builder.HasKey(r => new { r.PlantId, r.InstallationId });
        builder.HasOne(pt => pt.Installation)
        .WithMany(p => p.ShipsToPlant)
        .HasForeignKey(pt => pt.InstallationId).OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(pt => pt.Plant)
          .WithMany(p => p.GetsShipmentsFromInstallation)
          .HasForeignKey(pt => pt.PlantId).OnDelete(DeleteBehavior.Restrict);
    }
}
