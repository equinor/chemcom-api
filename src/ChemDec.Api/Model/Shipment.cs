using Db = ChemDec.Api.Datamodel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ChemDec.Api.Model
{
    public class Shipment
    {
        public Guid Id { get; set; }
        public string Code { get; set; } /*us248: fjern denne*/
        public string Title { get; set; }
        public Reference Sender { get; set; }
        public Guid SenderId { get; set; }
        public PlantReference Receiver { get; set; }
        public string Type { get; set; }
        public string Status { get; set; } //Planned, changed etc
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

        public IEnumerable<ShipmentChemical> Chemicals { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<LogEntry> LogEntries { get; set; }
        public IEnumerable<ShipmentPart> ShipmentParts { get; set; }
        public List<IFormFile>  FileAttachments { get; set; }

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
    }

    public class Comment
    {
        public Guid Id { get; set; }
        public string CommentText { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
    }

    public class NewCommentRequest
    {
        public Shipment Shipment { get; set; }
        public string Comment { get; set; }
    }

    public class ShipmentInfo
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public Reference Sender { get; set; }
        public Reference Receiver { get; set; }
        public string Type { get; set; }
        public string Status { get; set; } //Planned, changed etc
        public DateTime PlannedExecutionFrom { get; set; }
        public DateTime PlannedExecutionTo { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public bool HasBeenOpened { get; set; }
        public string Well { get; set; }
        public string LastComment { get; set; }
        public string EvalComments { get; set; }
    }

    public class LogEntry
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public Reference Installation { get; set; }
        public IEnumerable<FieldChange> FieldChanges { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }

    }

    public class FieldChange
    {
        public Guid Id { get; set; }
        public string FromField { get; set; }
        public string FromValue { get; set; }
        public string ToField { get; set; }
        public string ToValue { get; set; }

    }
    public class ShipmentResponse
    {
        public int Total { get; set; }
        public int Skipped { get; set; }
        public int Count => Shipments.Count;
        public List<ShipmentInfo> Shipments { get; set; }
    }

    public class ShipmentPart
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

    public class ShipmentProfile : Profile
    {
        public ShipmentProfile()
        {

            CreateMap<Shipment, Db.Shipment>()
                .ForMember(dest => dest.Sender, m => m.Ignore())
                .ForMember(dest => dest.Receiver, m => m.Ignore())
                .ForMember(dest => dest.Attachments, m => m.Ignore())
                .ForMember(dest => dest.Chemicals, m => m.Ignore())
                .ForMember(dest => dest.Comments, m => m.Ignore())
                .ForMember(dest => dest.ShipmentParts, m => m.Ignore())

                ;
            CreateMap<Db.Shipment, Shipment>()
                .ForMember(dest => dest.Attachments, m => m.MapFrom(d => d.Attachments.OrderByDescending(o => o.Updated)))
                .ForMember(dest => dest.Chemicals, m => m.MapFrom(d => d.Chemicals.OrderByDescending(o => o.Updated)))
                .ForMember(dest => dest.Comments, m => m.MapFrom(d => d.Comments.OrderByDescending(o => o.Updated)))
                .ForMember(dest => dest.ShipmentParts, m => m.MapFrom(d => d.ShipmentParts.OrderBy(o => o.Shipped)))
                ;

            CreateMap<Db.ShipmentPart, ShipmentPart>();
            CreateMap<ShipmentPart, Db.ShipmentPart>();

            CreateMap<Db.Comment, Comment>();
            CreateMap<Comment, Db.Comment>();
            CreateMap<Db.LogEntry, LogEntry>();
            CreateMap<Db.FieldChange, FieldChange>();

            CreateMap<Db.Shipment, ShipmentInfo>()
                .ForMember(d => d.LastComment,
                s => s.MapFrom(m => m.Comments.OrderByDescending(o => o.Updated).Select(ss => ss.CommentText).FirstOrDefault()));
            CreateMap<Db.Installation, Reference>();

        }
    }



}
