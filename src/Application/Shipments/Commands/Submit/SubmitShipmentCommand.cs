using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Submit;

public sealed record SubmitShipmentCommand : ICommand
{
    public Guid ShipmentId { get; set; }
    public bool TakePrecaution { get; set; }
    public string Precautions { get; set; }
    public bool HeightenedLra { get; set; }
    public double? Pb210 { get; set; }
    public double? Ra226 { get; set; }
    public double? Ra228 { get; set; }
    public bool? AvailableForDailyContact { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
