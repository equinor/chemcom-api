using System;

namespace ChemDec.Api.Datamodel.Interfaces
{
    public interface IAudit
    {
        Guid Id { get; set; }
        DateTime Updated { get; set; }
        string UpdatedBy { get; set; }
        string UpdatedByName { get; set; }
    }
}
