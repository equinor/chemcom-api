namespace ChemDec.Api.Model
{

    public class Role
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Roletype { get; set; }
        public InstallationReference Installation { get; set; }
    }
}
