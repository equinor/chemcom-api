using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Comments;

public class Comment
{
    //public Comment(string comment, Guid? shipmentId, string updatedBy, string updatedByName)
    //{
    //    CommentText = comment;
    //    ShipmentId = shipmentId;
    //    UpdatedBy = updatedBy;
    //    UpdatedByName = updatedByName;
    //}

    public Guid Id { get; private set; }
    public string CommentText { get; private set; }
    public Guid? ShipmentId { get; private set; }
    public DateTime Updated { get; private set; }
    public string UpdatedBy { get; private set; }
    public string UpdatedByName { get; private set; }
    public Shipment Shipment { get; set; }

    public void Update(Guid shipmentId,string updatedBy, string updatedByName)
    {       
        ShipmentId = shipmentId;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;
    }

    private Comment()
    { }
}
