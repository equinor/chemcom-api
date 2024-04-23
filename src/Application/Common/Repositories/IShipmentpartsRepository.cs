using Domain.ShipmentParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Repositories;

public interface IShipmentPartsRepository
{
    Task InsertAsync(ShipmentPart shipmentPart, CancellationToken cancellationToken = default);
    Task InsertManyAsync(List<ShipmentPart> shipmentParts, CancellationToken cancellationToken = default);
    void Delete(ICollection<ShipmentPart> shipmentParts);
    Task<List<ShipmentPart>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
}
