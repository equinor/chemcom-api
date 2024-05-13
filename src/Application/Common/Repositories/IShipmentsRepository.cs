using Domain.ShipmentChemicals;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Repositories;

public interface IShipmentsRepository
{
    Task InsertAsync(Shipment shipment, CancellationToken cancellationToken = default);
    Task<Shipment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(Shipment shipment);
    Task AddShipmentChemicalAsync(ShipmentChemical shipmentChemical, CancellationToken cancellationToken = default);
    Task<ShipmentChemical> GetShipmentChemicalAsync(Guid shipmentId, Guid chemicalId, CancellationToken cancellationToken = default);
    Task<List<ShipmentChemical>> GetShipmentChemicalsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    void UpdateShipmentChemical(ShipmentChemical shipmentChemical);
    void DeleteShipmentChemical(ShipmentChemical shipmentChemical);
    Task<ShipmentChemical> GetShipmentChemicalByIdAsync(Guid id, CancellationToken cancellationToken);
}
