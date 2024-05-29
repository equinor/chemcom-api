using Domain.Attachments;
using Domain.Comments;
using Domain.Common;
using Domain.Installations;
using Domain.LogEntries;
using Domain.ShipmentChemicals;
using Domain.ShipmentParts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments;

public class Shipment : IAuditable
{
    private readonly List<Attachment> _attachments = new();
    private readonly List<Comment> _comments = new();
    private readonly List<ShipmentChemical> _chemicals = new();
    private readonly List<ShipmentPart> _shipmentParts = new();
    private readonly List<LogEntry> _logEntries = new();

    public Shipment(ShipmentDetails shipmentDetails)
    {
        Code = shipmentDetails.Code;
        Title = shipmentDetails.Title;
        SenderId = shipmentDetails.SenderId;
        ReceiverId = shipmentDetails.ReceiverId;
        Type = shipmentDetails.Type;
        RinsingOffshorePercent = shipmentDetails.RinsingOffshorePercent;
        PlannedExecutionFrom = shipmentDetails.PlannedExecutionFrom;
        PlannedExecutionTo = shipmentDetails.PlannedExecutionTo;
        WaterAmount = shipmentDetails.WaterAmount;
        WaterAmountPerHour = shipmentDetails.WaterAmountPerHour;
        VolumePersentageOffspec = shipmentDetails.VolumePersentageOffspec;
        Well = shipmentDetails.Well;
        ContainsChemicals = shipmentDetails.ContainsChemicals;
        ContainsStableOilEmulsion = shipmentDetails.ContainsStableOilEmulsion;
        ContainsHighParticleAmount = shipmentDetails.ContainsHighParticleAmount;
        ContainsBiocides = shipmentDetails.ContainsBiocides;
        VolumeHasBeenMinimized = shipmentDetails.VolumeHasBeenMinimized;
        VolumeHasBeenMinimizedComment = shipmentDetails.VolumeHasBeenMinimizedComment;
        NormalProcedure = shipmentDetails.NormalProcedure;
        OnlyWayToGetRidOf = shipmentDetails.OnlyWayToGetRidOf;
        OnlyWayToGetRidOfComment = shipmentDetails.OnlyWayToGetRidOfComment;
        AvailableForDailyContact = shipmentDetails.AvailableForDailyContact;
        HeightenedLra = shipmentDetails.HeightenedLra;
        Pb210 = shipmentDetails.Pb210;
        Ra226 = shipmentDetails.Ra226;
        Ra228 = shipmentDetails.Ra228;
        TakePrecaution = shipmentDetails.TakePrecaution;
        Precautions = shipmentDetails.Precautions;
        WaterHasBeenAnalyzed = shipmentDetails.WaterHasBeenAnalyzed;
        HasBeenOpened = shipmentDetails.HasBeenOpened;
        Updated = DateTime.Now;
        UpdatedBy = shipmentDetails.UpdatedBy;
        UpdatedByName = shipmentDetails.UpdatedByName;
        HasBeenOpened = shipmentDetails.HasBeenOpened;
    }


    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Title { get; private set; }
    public Guid SenderId { get; set; }
    public Installation Sender { get; private set; }
    public Guid ReceiverId { get; set; }
    public Installation Receiver { get; set; }
    public string Type { get; set; }
    public string Status { get; set; } //Planned, changed etc. Kanskje vi trenger en "draft" også
    public double RinsingOffshorePercent { get; set; } /*us292: denne skal inn igjen*/
    public DateTime PlannedExecutionFrom { get; set; }
    public DateTime PlannedExecutionTo { get; set; }
    public double WaterAmount { get; private set; } //m3 //total amount of water
    public double WaterAmountPerHour { get; set; } //m3

    public bool VolumePersentageOffspec { get; set; } //true if above 0.5% 
    public string Well { get; set; }

    //Checks
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
    public ICollection<Attachment> Attachments { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<ShipmentChemical> Chemicals { get; set; }
    //public ICollection<ShipmentPart> ShipmentParts { get; private set; }
    public ICollection<LogEntry> LogEntries { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
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

    //public void AddNewShipmentParts(List<int> shipmentParts, DateTime plannedExecutionFrom, int days)
    //{
    //    for (int i = 0; i < days; i++)
    //    {
    //        DateTime shippedDate = plannedExecutionFrom.AddDays(i);
    //        ShipmentPart shipmentPart = new(shipmentParts[i], shippedDate)
    //        {
    //            Updated = DateTime.Now,
    //            UpdatedByName = UpdatedByName,
    //            UpdatedBy = UpdatedBy
    //        };
    //        shipmentPart.SetNewId();
    //        //ShipmentParts.Add(shipmentPart);
    //    }
    //}

    public void SetUpdatedInfo(string updatedBy, string updatedByName)
    {
        Updated = DateTime.Now;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;
    }

    public List<ShipmentPart> AddNewShipmentParts(List<double> shipmentParts, DateTime plannedExecutionFrom, int days)
    {
        var list = new List<ShipmentPart>();
        for (int i = 0; i < days; i++)
        {
            DateTime shippedDate = plannedExecutionFrom.AddDays(i);
            ShipmentPart shipmentPart = new(shipmentParts[i], shippedDate)
            {
                ShipmentId = Id,
                Updated = DateTime.Now,
                UpdatedByName = UpdatedByName,
                UpdatedBy = UpdatedBy
            };
            shipmentPart.SetNewId();
            list.Add(shipmentPart);
        }

        return list;
    }

    public void SetStatus(string status)
    {
        Status = status;
    }

    public void SetNewId()
    {
        Id = Guid.NewGuid();
    }

    public void Update(ShipmentDetails shipmentDetails)
    {
        Code = shipmentDetails.Code;
        Title = shipmentDetails.Title;
        SenderId = shipmentDetails.SenderId;
        ReceiverId = shipmentDetails.ReceiverId;
        Type = shipmentDetails.Type;
        RinsingOffshorePercent = shipmentDetails.RinsingOffshorePercent;
        PlannedExecutionFrom = shipmentDetails.PlannedExecutionFrom;
        PlannedExecutionTo = shipmentDetails.PlannedExecutionTo;
        WaterAmount = shipmentDetails.WaterAmount;
        WaterAmountPerHour = shipmentDetails.WaterAmountPerHour;
        VolumePersentageOffspec = shipmentDetails.VolumePersentageOffspec;
        Well = shipmentDetails.Well;
        ContainsChemicals = shipmentDetails.ContainsChemicals;
        ContainsStableOilEmulsion = shipmentDetails.ContainsStableOilEmulsion;
        ContainsHighParticleAmount = shipmentDetails.ContainsHighParticleAmount;
        ContainsBiocides = shipmentDetails.ContainsBiocides;
        VolumeHasBeenMinimized = shipmentDetails.VolumeHasBeenMinimized;
        VolumeHasBeenMinimizedComment = shipmentDetails.VolumeHasBeenMinimizedComment;
        NormalProcedure = shipmentDetails.NormalProcedure;
        OnlyWayToGetRidOf = shipmentDetails.OnlyWayToGetRidOf;
        OnlyWayToGetRidOfComment = shipmentDetails.OnlyWayToGetRidOfComment;
        AvailableForDailyContact = shipmentDetails.AvailableForDailyContact;
        HeightenedLra = shipmentDetails.HeightenedLra;
        Pb210 = shipmentDetails.Pb210;
        Ra226 = shipmentDetails.Ra226;
        Ra228 = shipmentDetails.Ra228;
        TakePrecaution = shipmentDetails.TakePrecaution;
        Precautions = shipmentDetails.Precautions;
        WaterHasBeenAnalyzed = shipmentDetails.WaterHasBeenAnalyzed;
        Updated = DateTime.Now;
        UpdatedBy = shipmentDetails.UpdatedBy;
        UpdatedByName = shipmentDetails.UpdatedByName;
        HasBeenOpened = shipmentDetails.HasBeenOpened;
    }

    private Shipment()
    {
    }
}
