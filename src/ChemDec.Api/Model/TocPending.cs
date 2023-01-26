namespace ChemDec.Api.Model
{
    public class TocPending : ChemicalBase
    {
        public TocPending()
        {
            Name = ChemicalTypes.TocPending;
            Color = ChemicalColors.TocPending;
            Stack = ChemicalStack.Toc;
        }
    }
}
