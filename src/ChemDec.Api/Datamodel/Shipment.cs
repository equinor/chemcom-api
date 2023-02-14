using ChemDec.Api.Datamodel.Interfaces;
using System;
using System.Collections.Generic;

namespace ChemDec.Api.Datamodel
{
    public class Shipment : IAudit
    {
        public Guid Id { get; set; }
        public string Code { get; set; } /*us248: fjern denne*/
        public string Title { get; set; }
        public Guid SenderId { get; set; }        
        public Installation Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public Installation Receiver { get; set; }
        public string Type { get; set; }
        public string Status { get; set; } //Planned, changed etc. Kanskje vi trenger en "draft" også
        public double RinsingOffshorePercent { get; set; } /*us292: denne skal inn igjen*/
        public DateTime PlannedExecutionFrom { get; set; }
        public DateTime PlannedExecutionTo { get; set; }
        public double WaterAmount { get; set; } //m3 //total amount of water
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
        public Attachment WaterAnalysis { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<ShipmentChemical> Chemicals { get; set; }
        public ICollection<ShipmentPart> ShipmentParts { get; set; }
        public ICollection<LogEntry> LogEntries { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }


        public bool HasBeenOpened { get; set; }

        //Evaluation
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

    }

    public class ShipmentPart : IAudit
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public DateTime Shipped { get; set; }
        public double Water { get; set; }

        /* This does not apply yet but might in the future
        public double Toc { get; set; }
        public double Nitrogen { get; set; }
        public double Biocides { get; set; }
        */

        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }


    }



}
