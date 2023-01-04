using System;
using System.Collections.Generic;
using ChemDec.Api.Model.Filtering;

namespace ChemDec.Api.Model
{
    public class Filter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public IEnumerable<InstallationInfo> SenderInstallations { get; set; }
        public IEnumerable<ChemicalInfo> Chemicals {get;set;}


        public Filter()
        {
            //default from and to dates
            From = new DateTime(2020, 7, 9);
            To = new DateTime(2020, 7, 10);

            //use defaults for filter for now:
            var senderInstallations = new List<InstallationInfo>();
            senderInstallations.Add(new InstallationInfo { 
                Id = new Guid("B10FC741-EBE3-45C1-BC70-8ECA5BA5CED6"), Code = "OsebergC", Name = "Oseberg C" });
            senderInstallations.Add(new InstallationInfo { 
                Id = new Guid("B10FC741-EBE3-45C1-BC70-8ECA5BA5CF03"), Code = "Brage", Name = "Brage" });
            SenderInstallations = SenderInstallations;
            //leave chemicals as null, we want all chemicals for now
        }
    }
}
