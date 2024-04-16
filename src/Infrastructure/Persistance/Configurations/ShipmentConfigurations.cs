using Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configurations;

public class ShipmentConfigurations : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(e => e.ReceiverId, "IX_Shipments_ReceiverId");
        builder.HasIndex(e => e.SenderId, "IX_Shipments_SenderId");

        builder.HasOne(bc => bc.Sender)
               .WithMany(b => b.SentShipments)
               .HasForeignKey(f => f.SenderId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bc => bc.Receiver)
           .WithMany(b => b.ReceivedShipments)
           .HasForeignKey(f => f.ReceiverId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(bc => bc.Attachments)
              .WithOne(b => b.Shipment)
              .HasForeignKey(f => f.ShipmentId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
