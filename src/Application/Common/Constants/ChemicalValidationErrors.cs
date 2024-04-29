using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Constants;

public static class ChemicalValidationErrors
{
    public const string ChemicalNameRequiredText = "Chemical name must be set";
    public const string ChemicalDescriptionRequiredText = "Chemical description must be set";
    public const string ChemicalNameSemicolonNotAllowedText = "Chemical name cannot contain semicolons.";
    public const string ChemicalDescriptionSemicolonNotAllowedText = "Chemical description cannot contain semicolons.";
    public const string ChemicalAlreadyExistsText = "Chemical with the name {0} already exists";
}
