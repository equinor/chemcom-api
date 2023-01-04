using System;
using System.ComponentModel.DataAnnotations;

namespace ChemDec.Api.Datamodel
{
    public class BaseTable
    {
        public Guid Id { get; set; }
        public DateTime? Created { get; set; }

        [MaxLength(64)]
        public string CreatedBy { get; set; }
    }
    public class UpdatableBaseTable : BaseTable
    {
        public DateTime? Updated { get; set; }

        [MaxLength(64)]
        public string UpdatedBy { get; set; }
    }
}

