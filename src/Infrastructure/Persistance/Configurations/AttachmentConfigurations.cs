using Domain.Attachments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configurations;

public class AttachmentConfigurations : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasIndex(e => e.ShipmentId, "IX_Attachments_ShipmentId");
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasOne(d => d.Shipment).WithMany(p => p.Attachments)
            .HasForeignKey(d => d.ShipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
