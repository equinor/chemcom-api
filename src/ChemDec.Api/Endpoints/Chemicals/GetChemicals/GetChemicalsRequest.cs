using Microsoft.AspNetCore.Mvc;
using System;

namespace ChemDec.Api.Endpoints.Chemicals.GetChemicals;

public sealed record GetChemicalsRequest
{    
    public bool ExcludeActive { get; set; }   
    public bool ExcludeDisabled { get; set; }  
    public bool ExcludeProposed { get; set; }    
    public bool ExcludeNotProposed { get; set; }    
    public Guid? ForInstallation { get; set; }
}
