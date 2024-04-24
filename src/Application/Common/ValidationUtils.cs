using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public static class ValidationUtils
{
    public static bool IsCorrectMeasureUnit(string measureUnit)
    {
        return measureUnit == MeasureUnit.Kilogram ||
               measureUnit == MeasureUnit.Litre ||
               measureUnit == MeasureUnit.Tonn ||
               measureUnit == MeasureUnit.CubicMetre;
    }
}
