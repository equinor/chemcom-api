﻿using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands;
using Application.Shipments.Commands.Create;
using Domain.Attachments;
using Domain.ShipmentChemicals;
using Domain.ShipmentParts;
using Domain.Shipments;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Update;

public sealed record UpdateShipmentCommand : ICommand
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public Guid SenderId { get; set; }
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

    public User User { get; set; }

    public static ShipmentDetails Map(UpdateShipmentCommand command)
    {
        return new ShipmentDetails()
        {
            SenderId = command.SenderId,
            Code = command.Code,
            Title = command.Title,
            Type = command.Type,
            PlannedExecutionFrom = command.PlannedExecutionFrom.Value,
            PlannedExecutionTo = command.PlannedExecutionTo.Value,
            WaterAmount = command.WaterAmount,
            WaterAmountPerHour = command.WaterAmountPerHour,
            Well = command.Well,
            UpdatedBy = command.User.Email,
            UpdatedByName = command.User.Name,
            VolumePersentageOffspec = command.VolumePersentageOffspec,
            ContainsChemicals = command.ContainsChemicals,
            ContainsStableOilEmulsion = command.ContainsStableOilEmulsion,
            ContainsHighParticleAmount = command.ContainsHighParticleAmount,
            ContainsBiocides = command.ContainsBiocides,
            VolumeHasBeenMinimized = command.VolumeHasBeenMinimized,
            VolumeHasBeenMinimizedComment = command.VolumeHasBeenMinimizedComment,
            NormalProcedure = command.NormalProcedure,
            OnlyWayToGetRidOf = command.OnlyWayToGetRidOf,
            OnlyWayToGetRidOfComment = command.OnlyWayToGetRidOfComment,
            AvailableForDailyContact = command.AvailableForDailyContact,
            HeightenedLra = command.HeightenedLra,
            Pb210 = command.Pb210,
            Ra226 = command.Ra226,
            Ra228 = command.Ra228,
            TakePrecaution = command.TakePrecaution,
            Precautions = command.Precautions,
            WaterHasBeenAnalyzed = command.WaterHasBeenAnalyzed,
            HasBeenOpened = command.HasBeenOpened,
            RinsingOffshorePercent = command.RinsingOffshorePercent
        };
    }
}
