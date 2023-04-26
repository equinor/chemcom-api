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
    public class InstallationController : ControllerBase
    {
        private readonly InstallationHandler handler;

        public InstallationController(InstallationHandler handler)
        {
            this.handler = handler;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<InstallationResponse>> Installations(int? skip, int? take, bool? excludePlants = false, bool? excludePlatforms = false, string filter = null)
        {
            skip = skip ?? 0;
            take = take ?? int.MaxValue;
            var res = handler.GetInstallations();

            if (string.IsNullOrWhiteSpace(filter) == false)
            {
                res = res.Where(w => w.Code.StartsWith(filter) || w.Description.Contains(filter) || w.Name.Contains(filter));
            }

            if (excludePlants == true)
            {
                res = res.Where(w => w.InstallationType != "plant");
            }

            if (excludePlatforms == true)
            {
                res = res.Where(w => w.InstallationType != "platform");
            }


            return new InstallationResponse
            {
                Total = res.Count(),
                Skipped = skip.Value,
                Installations = await res.OrderBy(o => o.Code).Skip(skip.Value).Take(take.Value).ToListAsync()
            };

        }

        [HttpPost]
        [Route("reservoir/{plantId}")]
        public async Task<ActionResult> SaveReservoirData(Guid plantId, double? toc, double? nitrogen, double? water)
        {
            (var ok, var validationErrors) = await handler.SaveReservoirData(plantId, toc, nitrogen, water);

            if (validationErrors != null)
            {
                return BadRequest(new { error = validationErrors });
            }

            return Ok(new { Res = ok });
        }
        [HttpGet]
        [Route("reservoir/{plantId}")]
        public async Task<ActionResult> GetReservoirData(Guid plantId)
        {

            (var reservoirData, var validationErrors) = await handler.GetReservoirData(plantId);

            if (validationErrors != null)
            {
                return BadRequest(new { error = validationErrors });
            }

            return Ok(new { Res = reservoirData });
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Installation>> SaveInstallation([FromBody] Installation installation)
        {
            (var savedInstallation, var validationErrors) = await handler.SaveOrUpdate(installation);

            if (validationErrors != null)
            {
                return BadRequest(new { error = validationErrors });
            }

            return savedInstallation;
        }


    }
}