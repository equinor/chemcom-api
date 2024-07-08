using Application.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Evaluate;

public sealed record SaveEvaluationCommand : ICommand
{
    public Guid ShipmentId { get; set; }
    public bool? EvalAmountOk { get; set; }
    public bool? EvalBiocidesOk { get; set; }
    public bool? EvalCapacityOk { get; set; }
    public bool? EvalContaminationRisk { get; set; }
    public string EvalEnvImpact { get; set; }
    public string EvalComments { get; set; }
    public User User { get; set; }
}
