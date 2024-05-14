using Domain.Common;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ShipmentParts;

public class ShipmentPart : IAuditable
{
    public ShipmentPart(double water, DateTime shippedDate)
    {
        Water = water;
        Shipped = shippedDate;
    }
    public Guid Id { get; private set; }
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; }
    public DateTime Shipped { get; set; }
    public double Water { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }

    public void SetNewId()
    {
        Id = Guid.NewGuid();
    }

    private ShipmentPart()
    {
    }
}
