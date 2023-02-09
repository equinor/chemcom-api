using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemDec.Api.Controllers.Handlers;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Db = ChemDec.Api.Datamodel;

namespace ChemDec.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShipmentController : ControllerBase
    {
        private readonly ShipmentHandler handler;
        private readonly UserService userService;
        private readonly Db.ChemContext db;

        public ShipmentController(ShipmentHandler handler, UserService userService, Db.ChemContext db)
        {
            this.handler = handler;
            this.userService = userService;
            this.db = db;
        }

        [HttpGet]
        [Route("")]
        public async Task<ShipmentResponse> Shipments(int? skip, int? take, Guid? fromInstallationId, Guid? toInstallationId, DateTime? from, DateTime? to)
        {
            //TODO user check
            if (from != null) from = new DateTime(from.Value.Year, from.Value.Month, from.Value.Day, 0, 0, 0);
            if (to != null) to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59);

            skip = skip ?? 0;
            take = take ?? int.MaxValue;
            var res = handler.GetShipmentsInfo();

            if (fromInstallationId != null)
            {
                res = res.Where(w => w.Sender.Id == fromInstallationId);
            }

            if (toInstallationId != null)
            {
                res = res.Where(w => w.Receiver.Id == toInstallationId).Where(w=>w.Status != ShipmentHandler.Statuses.Draft);
            }

            if (from != null)
            {
                res = res.Where(w => w.PlannedExecutionFrom >= from);
            }

            if (to != null)
            {
                res = res.Where(w => w.PlannedExecutionFrom <= to);
            }
            
            return new ShipmentResponse
            {
                Total = res.Count(),
                Skipped = skip.Value,
                Shipments = await res.OrderBy(o => o.Updated).Skip(skip.Value).Take(take.Value).ToListAsync()
            };

        }

        
        [HttpGet]
        [Route("chemicalhistory")]
        public async Task<ActionResult<List<ShipmentChemicalTableItem>>> GetChemicalHistoryForPlant(DateTime? from, DateTime? to, Guid? toInstallationId, string timeZone) //, Filter filter)
        {
            //get data for entire days
            if (from != null) from = new DateTime(from.Value.Year, from.Value.Month, from.Value.Day, 0, 0, 0);
            if (to != null) to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59);
            
            if (toInstallationId == null)
            {
                return BadRequest(new { error = new List<string> { "toInstallationId can not be null " } });
            }
            
            var chemicalShipmentsToPlant = await handler.GetChemicalHistoryForPlant(toInstallationId, timeZone, from, to);

            return chemicalShipmentsToPlant;
        }

        [HttpGet]
        [Route("chemicalhistoryexport")]
        public async Task<IActionResult> ExportChemicalHistoryForPlant(DateTime? from, DateTime? to, Guid? toInstallationId, string timeZone) //, Filter filter)
        {
            //get data for entire days
            if (from != null) from = new DateTime(from.Value.Year, from.Value.Month, from.Value.Day, 0, 0, 0);
            if (to != null) to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59);

            if (toInstallationId == null)
            {
                return BadRequest(new { error = new List<string> { "toInstallationId can not be null " } });
            }

            var chemicalShipmentsToPlant = await handler.GetChemicalHistoryForPlant(toInstallationId, timeZone, from, to);

            Stream stream = new MemoryStream();

            using (var file = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
            {
                var csv = new CsvGenerator().ToString(chemicalShipmentsToPlant, createHeader: true);
                await file.WriteAsync(csv);
            }

            stream.Position = 0;
            var fromDate = from.HasValue ? from.Value.ToString("yyyy.MM.dd") : ""; // $"{ from.Value.Year}.{from.Value.Month}.{from.Value.Day}" : "";
            var toDate = to.HasValue ? $"{to.Value.Year}.{to.Value.Month}.{to.Value.Day}" : "";
            //return File(stream, "text/csv", $"History {chemicalShipmentsToPlant.FirstOrDefault().FromInstallation}.csv");
            return File(stream, "text/csv", $"Chemical History {fromDate}-{toDate}.csv");
        }
             

        [HttpGet]
        [Route("graph")]
        public async Task<ActionResult<GraphData>> SummaryForGraph(Guid? fromInstallationId, Guid? toInstallationId, DateTime? from, DateTime? to, string timeZone, bool excludeDraft = true, string groupBy = "day", Guid? exceptShipment = null)
        {
            if (from != null) from = new DateTime(from.Value.Year, from.Value.Month, from.Value.Day, 0, 0, 0);
            if (to != null) to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59);
            if (groupBy != "day" && groupBy != "hour" && groupBy != "month" && groupBy != "year" && groupBy != "total")
            {
                return BadRequest(new { error = new List<string> { "groupBy must be hour, day, month or year " } });
            }
            var res = await handler.GetSummary(fromInstallationId, toInstallationId, from, to, timeZone, excludeDraft, groupBy, exceptShipment);

            return res;
        }

        [HttpGet]
        [Route("{roleCode}/{shipmentId}")]
        public async Task<ActionResult<Shipment>> Shipment(string roleCode,Guid shipmentId)
        {
            
            var res = await handler.GetShipments().FirstOrDefaultAsync(w => w.Id == shipmentId);

            if (res == null)
            {
                return BadRequest(new { error = $"Shipment with id {shipmentId} was not found"});
            }
            var user = await userService.GetCurrentUser();
            var role = user.Roles.FirstOrDefault(w => w.Code == roleCode);
            if (role == null)
            {
                return BadRequest(new { error = $"User do not have access to do this operations. This should not happen" });
            }

            if (res.Sender.Code != role.Installation?.Code && res.Receiver.Code != role.Installation?.Code)
            {
                return BadRequest(new { error = $"Shipment with id {shipmentId} does not belong to this location" });
            }
            return res;
        }
       
        [HttpPost]
        [Route("{initiator}/{operation}")]
        public async Task<ActionResult<Shipment>> Save(string initiator, string operation, [FromBody] Shipment shipment)
        {
            ShipmentHandler.Operation operationEnum;
            if (Enum.TryParse(operation, true, out operationEnum) == false) {
                return BadRequest(new { error = operation + " is not a valid operation" });
            };

            ShipmentHandler.Initiator initiatorEnum;
            if (Enum.TryParse(initiator, true, out initiatorEnum) == false)
            {
                return BadRequest(new { error = initiator + " is not a valid initiator" });
            };
            Shipment savedShipment;
            IEnumerable<string> validationErrors;
            if (operationEnum == ShipmentHandler.Operation.SaveEvaluation)
            {
                (savedShipment, validationErrors) = await handler.SaveShipmentEvaluation(shipment, initiatorEnum, null, null);
            }
            else
            {
                (savedShipment, validationErrors) = await handler.SaveOrUpdate(shipment, initiatorEnum, operationEnum, ShipmentHandler.DetailedOperation.Saved, null, null);
            }
            if (validationErrors != null)
            {
                return BadRequest(new { error = validationErrors });
            }

            return savedShipment;
        }

        [HttpGet]
        [Route("attachment/{shipmentId}/{attachmentId}/{*filename}")]
        public async Task<ActionResult> GetAttachment(Guid shipmentId, Guid attachmentId, string filename)
        {
           
            (var res, var validationErrors) = await handler.ReturnAttachment(shipmentId, attachmentId);
            if (validationErrors != null)
            {
                return BadRequest(new { error = validationErrors });
            }
            if (res == null)
            {
                return NotFound();
            }
            
            return new FileStreamResult(res.Attachment, res.MimeType) {
                FileDownloadName = $"{res.Path}"
            };
        }

        [HttpPost]
        [Route("{initiator}/attachment")]
        public async Task<ActionResult<Shipment>> AddAttachment(string initiator, [FromForm] NewAttachment request)
        {
            ShipmentHandler.Initiator initiatorEnum;
            if (Enum.TryParse(initiator, true, out initiatorEnum) == false)
            {
                return BadRequest(new { error = initiator + " is not a valid initiator" });
            };

            if (request.Attachment != null && request.Shipment != null)
            {
                var shipment = JsonConvert.DeserializeObject<Shipment>(request.Shipment);
                using (var attachment = request.Attachment.OpenReadStream())
                {
                    (var res, var validationErrors) = await handler.AddAttachment(shipment, initiatorEnum, request.Attachment.FileName, request.Attachment.ContentType, attachment);
                    if (validationErrors != null)
                    {
                        return BadRequest(new { error = validationErrors });
                    }
                    return res;
                }
            }
            return BadRequest(new { error = "Shipment or attachment was not included in the request" });

        }

        [HttpPost]
        [Route("{initiator}/remove-attachment/{attachmentId}")]
        public async Task<ActionResult<Shipment>> RemoveAttachment(string initiator, Guid attachmentId, [FromBody] Shipment shipment)
        {
            ShipmentHandler.Initiator initiatorEnum;
            if (Enum.TryParse(initiator, true, out initiatorEnum) == false)
            {
                return BadRequest(new { error = initiator + " is not a valid initiator" });
            };

            if (shipment != null)
            {
                (var res, var validationErrors) = await handler.RemoveAttachement(shipment, initiatorEnum, attachmentId);
                if (validationErrors != null)
                {
                    return BadRequest(new { error = validationErrors });
                }
                return res;
            }
            return BadRequest(new { error = "Shipment was not included in the request" });

        }



        [HttpPost]
        [Route("{initiator}/comment")]
        public async Task<ActionResult<Shipment>> AddComment(string initiator, NewCommentRequest request)
        {
            ShipmentHandler.Initiator initiatorEnum;
            if (Enum.TryParse(initiator, true, out initiatorEnum) == false)
            {
                return BadRequest(new { error = initiator + " is not a valid initiator" });
            };
            if (string.IsNullOrEmpty(request?.Comment) == false && request?.Shipment != null)
            {
                (var res, var validationErrors) = await handler.AddComment(request.Shipment, initiatorEnum, request.Comment);
                if (validationErrors != null)
                {
                    return BadRequest(new { error = validationErrors });
                }
                return res;
               
            }
            return BadRequest(new { error = "Shipment or comment was not included in the request" });

        }

        [HttpPost]
        [Route("{initiator}/remove-comment/{commentId}")]
        public async Task<ActionResult<Shipment>> RemoveComment(string initiator, Guid commentId, [FromBody] Shipment shipment)
        {
            ShipmentHandler.Initiator initiatorEnum;
            if (Enum.TryParse(initiator, true, out initiatorEnum) == false)
            {
                return BadRequest(new { error = initiator + " is not a valid initiator" });
            };
            if (shipment != null)
            {
                (var res, var validationErrors) = await handler.RemoveComment(shipment,commentId, initiatorEnum);
                if (validationErrors != null)
                {
                    return BadRequest(new { error = validationErrors });
                }
                return res;
            }
            return BadRequest(new { error = "Shipment was not included in the request" });

        }

        [HttpPost]
        [Route("CleanupAfterParts")]
        public async Task<ActionResult<Shipment>> AddPartsWhereMissing()
        {
            //Add ShipmentParts where these are not set. Remove in next round
            var shipmentsWithMissing = await db.Shipments.Include(i=>i.ShipmentParts).Where(w => w.ShipmentParts.Any() == false).ToListAsync();
            foreach (var s in shipmentsWithMissing)
            {
                if (s.PlannedExecutionFrom != DateTime.MinValue && s.PlannedExecutionTo != DateTime.MinValue
                    && s.PlannedExecutionFrom < s.PlannedExecutionTo)
                {
                    var to = new DateTime(s.PlannedExecutionTo.Year, s.PlannedExecutionTo.Month, s.PlannedExecutionTo.Day, 23, 59, 59);
                    var from = new DateTime(s.PlannedExecutionFrom.Year, s.PlannedExecutionFrom.Month, s.PlannedExecutionFrom.Day);

                    var days = to.Subtract(from).Days + 1;

                    for (int i = 0; i < days; i++)
                    {
                        s.ShipmentParts.Add(new Db.ShipmentPart { ShipmentId = s.Id, Shipped = s.PlannedExecutionTo.AddDays(i) });
                    }

                    s.ShipmentParts.First().Water = s.WaterAmount;
                }
            }

            // Recalculate shipments
            var shipmentsToUpdate = await db.ShipmentChemicals.Include(i => i.Chemical).Include(i => i.Shipment).ToListAsync();
            foreach (var shipment in shipmentsToUpdate)
            {
                ShipmentHandler.CalculateChemicals(shipment.Shipment.RinsingOffshorePercent, shipment, shipment.Chemical);
            }
            await db.SaveChangesAsync();
            var updatedShipments = shipmentsWithMissing.Select(s => s.Id).ToList();
            return Ok(new { updated = updatedShipments });

        }

    }
}