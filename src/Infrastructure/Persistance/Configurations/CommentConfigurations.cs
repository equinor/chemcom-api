using Domain.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configurations;

public class CommentConfigurations : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasIndex(e => e.ShipmentId, "IX_Comments_ShipmentId");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.HasOne(d => d.Shipment).WithMany(p => p.Comments).HasForeignKey(d => d.ShipmentId);
    }
}
