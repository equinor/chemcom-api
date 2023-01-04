using System;

namespace ChemDec.Api.Model
{
    //lightweight class for filter and search
    public class InstallationInfo
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
