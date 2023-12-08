using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands;
using Domain.Attachments;
using Domain.ShipmentChemicals;
using Domain.ShipmentParts;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Create;

public sealed class CreateShipmentCommand : ICommand
{
    public string Code { get; set; }
    public string Title { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Initiator Initiator { get; set; }
    public bool IsInstallationPartOfUserRoles { get; set; }
    public string Type { get; set; }
    public DateTime? PlannedExecutionFrom { get; set; }
    public DateTime? PlannedExecutionTo { get; set; }
    public double WaterAmount { get; set; }
    public double WaterAmountPerHour { get; set; }
    public string Well { get; set; }
    public List<int> ShipmentParts { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }

    public static ShipmentDetails Map(CreateShipmentCommand command)
    {
        return new ShipmentDetails()
        {
            SenderId = command.SenderId,
            ReceiverId = command.ReceiverId,
            Code = command.Code,
            Title = command.Title,
            Type = command.Type,
            PlannedExecutionFrom = command.PlannedExecutionFrom.Value,
            PlannedExecutionTo = command.PlannedExecutionTo.Value,
            WaterAmount = command.WaterAmount,
            WaterAmountPerHour = command.WaterAmountPerHour,
            Well = command.Well,
            UpdatedBy = command.UpdatedBy,
            UpdatedByName = command.UpdatedByName,
        };
    }
}