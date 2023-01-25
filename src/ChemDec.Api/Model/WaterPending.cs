namespace ChemDec.Api.Model
{
    public class WaterPending : ChemicalBase
    {
        public WaterPending()
        {
            Name = ChemicalTypes.WaterPending;
            Color = ChemicalColors.WaterPending;
            Stack = ChemicalStack.Water;
        }
    }
}
