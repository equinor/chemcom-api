using Domain.Shipments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands;

public class ShipmentCommandValidator : AbstractValidator<ShipmentCommandsBase>
{
    public ShipmentCommandValidator()
    {

    }
}
