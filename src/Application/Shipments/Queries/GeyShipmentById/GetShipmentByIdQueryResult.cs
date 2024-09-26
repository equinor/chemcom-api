using Application.Shipments.Commands.Create;
using Domain.Installations;
using Domain.ShipmentParts;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Queries.GeyShipmentById;

public sealed record GetShipmentByIdQueryResult
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public Guid ReceiverId { get; set; }
    public string Type { get; set; }
    public double RinsingOffshorePercent { get; set; }
    public DateTime? PlannedExecutionFrom { get; set; }
    public DateTime? PlannedExecutionTo { get; set; }
    public double WaterAmount { get; set; }
    public double WaterAmountPerHour { get; set; }
    public string Well { get; set; }
    public List<double> ShipmentParts { get; set; }
    public bool VolumePersentageOffspec { get; set; }
    public bool ContainsChemicals { get; set; }
    public bool ContainsStableOilEmulsion { get; set; }
    public bool ContainsHighParticleAmount { get; set; }
    public bool ContainsBiocides { get; set; }
    public bool VolumeHasBeenMinimized { get; set; }
    public string VolumeHasBeenMinimizedComment { get; set; }
    public bool? NormalProcedure { get; set; }
    public bool? OnlyWayToGetRidOf { get; set; }
    public string OnlyWayToGetRidOfComment { get; set; }
    public bool? AvailableForDailyContact { get; set; }
    public bool HeightenedLra { get; set; }
    public double? Pb210 { get; set; }
    public double? Ra226 { get; set; }
    public double? Ra228 { get; set; }
    public bool TakePrecaution { get; set; }
    public string Precautions { get; set; }
    public bool WaterHasBeenAnalyzed { get; set; }
    public bool HasBeenOpened { get; set; }
    public bool? EvalCapacityOk { get; set; }
    public string EvalCapacityOkUpdatedBy { get; set; }
    public bool? EvalContaminationRisk { get; set; }
    public string EvalContaminationRiskUpdatedBy { get; set; }
    public bool? EvalAmountOk { get; set; }
    public string EvalAmountOkUpdatedBy { get; set; }
    public bool? EvalBiocidesOk { get; set; }
    public string EvalBiocidesOkUpdatedBy { get; set; }
    public string EvalEnvImpact { get; set; }
    public string EvalComments { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
    public InstallationResult Sender { get; set; }

    public static GetShipmentByIdQueryResult Map(Shipment shipment, Installation installation, List<ShipmentPart> shipmentParts = null)
    {
        GetShipmentByIdQueryResult result = new()
        {
            Id = shipment.Id,
            Code = shipment.Code,
            Title = shipment.Title,
            Status = shipment.Status,
            Sender = new InstallationResult(installation.Id, installation.Name, installation.Code),
            ReceiverId = shipment.ReceiverId,
            Type = shipment.Type,
            RinsingOffshorePercent = shipment.RinsingOffshorePercent,
            PlannedExecutionFrom = shipment.PlannedExecutionFrom,
            PlannedExecutionTo = shipment.PlannedExecutionTo,
            WaterAmount = shipment.WaterAmount,
            WaterAmountPerHour = shipment.WaterAmountPerHour,
            Well = shipment.Well,
            ShipmentParts = shipmentParts.OrderBy(s => s.Shipped).Select(s => s.Water).ToList(),
            VolumePersentageOffspec = shipment.VolumePersentageOffspec,
            ContainsChemicals = shipment.ContainsChemicals,
            ContainsStableOilEmulsion = shipment.ContainsStableOilEmulsion,
            ContainsHighParticleAmount = shipment.ContainsHighParticleAmount,
            ContainsBiocides = shipment.ContainsBiocides,
            VolumeHasBeenMinimized = shipment.VolumeHasBeenMinimized,
            VolumeHasBeenMinimizedComment = shipment.VolumeHasBeenMinimizedComment,
            NormalProcedure = shipment.NormalProcedure,
            OnlyWayToGetRidOf = shipment.OnlyWayToGetRidOf,
            OnlyWayToGetRidOfComment = shipment.OnlyWayToGetRidOfComment,
            AvailableForDailyContact = shipment.AvailableForDailyContact,
            HeightenedLra = shipment.HeightenedLra,
            Pb210 = shipment.Pb210,
            Ra226 = shipment.Ra226,
            Ra228 = shipment.Ra228,
            TakePrecaution = shipment.TakePrecaution,
            Precautions = shipment.Precautions,
            WaterHasBeenAnalyzed = shipment.WaterHasBeenAnalyzed,
            HasBeenOpened = shipment.HasBeenOpened,
            EvalAmountOk = shipment.EvalAmountOk,
            EvalAmountOkUpdatedBy = shipment.EvalAmountOkUpdatedBy,
            EvalBiocidesOk = shipment.EvalBiocidesOk,
            EvalBiocidesOkUpdatedBy = shipment.EvalBiocidesOkUpdatedBy,
            EvalCapacityOk = shipment.EvalCapacityOk,
            EvalCapacityOkUpdatedBy = shipment.EvalCapacityOkUpdatedBy,
            EvalContaminationRisk = shipment.EvalContaminationRisk,
            EvalContaminationRiskUpdatedBy = shipment.EvalContaminationRiskUpdatedBy,
            EvalEnvImpact = shipment.EvalEnvImpact,
            EvalComments = shipment.EvalComments,
            Updated = shipment.Updated,
            UpdatedBy = shipment.UpdatedBy,
            UpdatedByName = shipment.UpdatedByName
        };

        return result;
    }
}

public sealed record InstallationResult(Guid Id, string Name, string Code);
