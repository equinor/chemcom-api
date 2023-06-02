using System;
using System.Linq;
using System.Threading.Tasks;
using ChemDec.Api.Controllers.Handlers;
using ChemDec.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChemDec.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class ChemicalController : ControllerBase
    {
        private readonly ChemicalHandler handler;

        public ChemicalController(ChemicalHandler handler)
        {
            this.handler = handler;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<ChemicalResponse>> Chemicals(int? skip, int? take, bool? excludeActive = false, bool? excludeDisabled = true, bool? excludeProposed = true, bool? excludeNotProposed = false, string filter = null, Guid? forInstallation=null)
        {
            skip = skip ?? 0;
            take = take ?? int.MaxValue;
            var res = handler.GetChemicals();

            if (excludeDisabled == true)
            {
                res = res.Where(w => w.Disabled == false);
            }

            if (excludeActive == true)
            {
                res = res.Where(w => w.Disabled == true);
            }

            if (excludeProposed == true)
            {
                if (forInstallation != null)
                {
                    res = res.Where(w => w.Tentative == false || w.ProposedByInstallation.Id == forInstallation);
                }
                else
                {
                    res = res.Where(w => w.Tentative == false);
                }
            }
            if (excludeNotProposed == true)
            {
                res = res.Where(w => w.Tentative == true);
            }

            return new ChemicalResponse
            {
                Total = res.Count(),
                Skipped = skip.Value,
                Chemicals = await res.OrderBy(o => o.Name).Skip(skip.Value).Take(take.Value).ToListAsync()
            };
          
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Chemical>> SaveChemical([FromBody] Chemical chemical)
        {
            (var savedChemical, var validationErrors) = await handler.SaveOrUpdate(chemical);

            if (validationErrors != null)
            {
                return BadRequest(new { error = validationErrors });
            }

            return savedChemical;
        }

        [HttpPost]
        [Route("approve/{chemicalId}")]
        public async Task<ActionResult<dynamic>> ApproveChemical(Guid chemicalId)
        {
            //TODO usercheco
            (var ok, var validationErrors) = await handler.Approve(chemicalId);

            if (validationErrors != null)
            {
                return BadRequest(new { error = validationErrors });
            }

            return new { Res = "Approved" } ;
        }

    }
}