using ChemDec.Api.Datamodel.Interfaces;
using System;

namespace ChemDec.Api.Datamodel
{
    public class Comment: IAudit
    {
        public Guid Id { get; set; }
        public string CommentText { get; set; }
        public Guid? ShipmentId { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
    }
}
