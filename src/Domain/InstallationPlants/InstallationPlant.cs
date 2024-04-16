using Domain.Installations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.InstallationPlants;

public class InstallationPlant
{
    public Guid InstallationId { get; set; }
    public Installation Installation { get; set; }
    public Guid PlantId { get; set; }
    public Installation Plant { get; set; }
}
