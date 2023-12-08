using System;
using System.Collections.Generic;

namespace ChemDec.Api.Endpoints.Shipments;

public sealed record CreateShipmentRequest
{
    public string Code { get; private set; }
    public string Title { get; private set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Type { get; set; }
    public string Initiator { get; set; }
    public double RinsingOffshorePercent { get; set; }
    public DateTime? PlannedExecutionFrom { get; set; }
    public DateTime? PlannedExecutionTo { get; set; }
    public double WaterAmount { get; set; }
    public double WaterAmountPerHour { get; set; }
    public bool VolumePersentageOffspec { get; set; }
    public string Well { get; set; }
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
    public List<int> ShipmentParts { get; set; }
    //public bool? EvalCapacityOk { get; set; }
    //public string EvalCapacityOkUpdatedBy { get; set; }
    //public bool? EvalContaminationRisk { get; set; }
    //public string EvalContaminationRiskUpdatedBy { get; set; }
    //public bool? EvalAmountOk { get; set; }
    //public string EvalAmountOkUpdatedBy { get; set; }
    //public bool? EvalBiocidesOk { get; set; }
    //public string EvalBiocidesOkUpdatedBy { get; set; }
    //public string EvalEnvImpact { get; set; }
    //public string EvalComments { get; set; }
}