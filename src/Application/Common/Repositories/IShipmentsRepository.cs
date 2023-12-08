using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Repositories;

public interface IShipmentsRepository
{
    Task InsertAsync(Shipment shipment);
    Task<Shipment> GetByIdAsync(Guid id);
}
