using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Constants;

public static class ShipmentValidationErrors
{
    public const string InvalidInitiatorText = "Invalid initiator";
    public const string SenderRequiredText = "Sender is required";
    public const string PlannedExecutionFromDateRequiredText = "Planned execution from date is required";
    public const string PlannedExecutionToDateRequiredText = "Planned execution to date is required";
    public const string UserAccessForInstallationText = "User do not have access to save from this installation";
    public const string ChemicalNameRequiredText = "Chemical name must be set";
    public const string ChemicalNameSemicolonNotAllowedText = "Chemical name cannot contain semicolons.";
    public const string ChemicalDescriptionSemicolonNotAllowedText = "Chemical description cannot contain semicolons.";
    public const string ShipmentPartsDaysDoesNotMatchText = "Days does not match the execution dates. This should normally not happen";
    public const string ShipmentNotFoundText = "Shipment not found";
    public const string ShipmentCanNotBeResubmittedText = "Can't re-submit shipment";
    public const string FileUploadFailedText = "File upload failed";
    public const string AttachmentNotFound = "Attachment not found";
    public const string ShipmentChemicalNotFoundText = "Shipment chemical not found";
    public const string ChemicalAlreadyAddedText = "Chemical already added to shipment";
    public const string InvalidMeasureUnitText = "Invalid measure unit";
}
