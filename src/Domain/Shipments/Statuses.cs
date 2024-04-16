using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments;

public sealed class Statuses
{
    public const string Draft = nameof(Draft);
    public const string Submitted = nameof(Submitted);
    public const string Changed = nameof(Changed);
    public const string Approved = nameof(Approved);
    public const string Executed = nameof(Executed);
    public const string Declined = nameof(Declined);
}
