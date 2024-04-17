using Domain.ShipmentParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Repositories;

public interface IShipmentPartsRepository
{
    Task InsertAsync(ShipmentPart shipmentPart);
    void Delete(ICollection<ShipmentPart> shipmentParts);
    Task<List<ShipmentPart>> GetByShipmentId(Guid shipmentId);
}
