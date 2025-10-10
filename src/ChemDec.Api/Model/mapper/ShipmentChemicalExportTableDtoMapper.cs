using ChemDec.Api.Model.dto;

namespace ChemDec.Api.Model.mapper
{
    public static class ShipmentChemicalExportTableDtoMapper
    {
        public static ShipmentChemicalExportTableDto ToDto(ShipmentChemicalTableItem item)
        {
            return new ShipmentChemicalExportTableDto
            {
                ChemicalName = item.ChemicalName,
                Weight = item.Weight,
                Description = item.Description,
                ShipmentTitle = item.ShipmentTitle,
                PlannedExecutionFromDate = item.PlannedExecutionFromDate.ToString("yyyy-MM-dd") ,
                PlannedExecutionToDate = item.PlannedExecutionToDate.ToString("yyyy-MM-dd"),
                FromInstallation = item.FromInstallation,
                TocWeight = item.TocWeight,
                NitrogenWeight = item.NitrogenWeight,
                BiocideWeight = item.BiocideWeight,
                Density = item.Density,
                Amount = item.Amount,
                HazardClass = item.HazardClass,
                MeasureUnitDefault = item.MeasureUnitDefault,
                MeasureUnit = item.MeasureUnit,
                FollowOilPhaseDefault = item.FollowOilPhaseDefault,
                FollowWaterPhaseDefault = item.FollowWaterPhaseDefault,
                Water = item.Water
            };
        }
    }
}
