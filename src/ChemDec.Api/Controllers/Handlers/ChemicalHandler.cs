using Db = ChemDec.Api.Datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights.Channel;
using System.Reflection;
using Microsoft.ApplicationInsights;
using ChemDec.Api.Datamodel;

namespace ChemDec.Api.Controllers.Handlers
{
    public class ChemicalHandler
    {
        private readonly Db.ChemContext db;
        private readonly IMapper mapper;
        private readonly UserResolver userResolver;
        private readonly UserService _userService;
        private readonly MailSender _mailSender;
        private readonly IConfiguration _config;

        public ChemicalHandler(Db.ChemContext db, IMapper mapper, UserResolver userResolver, UserService userService, MailSender mailSender, IConfiguration config)
        {
            this.db = db;
            this.mapper = mapper;
            this.userResolver = userResolver;
            _userService = userService;
            _mailSender = mailSender;
            _config = config;
        }

        public IQueryable<Model.Chemical> GetChemicals()
        {
            return db.Chemicals.ProjectTo<Model.Chemical>(mapper.ConfigurationProvider);
        }

        public async Task<(Model.Chemical, IEnumerable<string>)> SaveOrUpdate(Model.Chemical chemical)
        {
            var validationErrors = new List<string>();
            var user = await _userService.GetCurrentUser();

            if (string.IsNullOrEmpty(chemical.Name))
            {
                validationErrors.Add("Chemical name must be set");
            }
            if (chemical.Name.Contains(';'))
            {
                validationErrors.Add("Chemical name cannot contain semicolons.");
            }
            if (chemical.Description.Contains(';'))
            {
                validationErrors.Add("Chemical description cannot contain semicolons.");
            }

            if (string.IsNullOrEmpty(chemical.Description))
            {
                validationErrors.Add("Chemical description must be set");
            }


            Db.Chemical dbObject = null;
            if (chemical.Id != Guid.Empty)
            {
                dbObject = await db.Chemicals
                    .FirstOrDefaultAsync(ps => ps.Id == chemical.Id);
            }
            else
            {
                dbObject = await db.Chemicals
                    .FirstOrDefaultAsync(ps => ps.Name == chemical.Name);
            }

            if (chemical.Id == Guid.Empty && dbObject != null)
            {
                validationErrors.Add("Chemical with name " + chemical.Name + " already exist");
            }

            if (validationErrors.Any()) return (null, validationErrors);


            if (dbObject != null)
            {
                // User check to see if user can update chemicals

                chemical.Id = dbObject.Id;
                var tentative = dbObject.Tentative;
                var tocWeight = dbObject.TocWeight;
                var nWeight = dbObject.NitrogenWeight;
                var density = dbObject.Density;

                mapper.Map(chemical, dbObject);
                dbObject.Tentative = tentative; // can only be approved through Approve-method
                dbObject = HandleRelations(chemical, dbObject);
                if (dbObject.TocWeight != tocWeight || dbObject.NitrogenWeight != nWeight || dbObject.Density != density)
                {
                    // Recalculate shipments
                    var shipmentsToUpdate = await db.ShipmentChemicals.Include(i => i.Shipment).Where(w => w.ChemicalId == dbObject.Id).ToListAsync();
                    foreach (var shipment in shipmentsToUpdate)
                    {
                        ShipmentHandler.CalculateChemicals(shipment.Shipment.RinsingOffshorePercent, shipment, dbObject);
                    }
                }

                await db.SaveChangesAsync();
            }
            else
            {
                // User check. Only chem administrator can add non-tentative chemicals
                var newDbObject = mapper.Map<Db.Chemical>(chemical);

                if (newDbObject.Id == Guid.Empty)
                    newDbObject.Id = Guid.NewGuid();
                newDbObject = HandleRelations(chemical, newDbObject);
                newDbObject.ProposedBy = user.Email;
                newDbObject.ProposedByEmail = user.Email;
                newDbObject.ProposedByName = user.Name;
                newDbObject.Proposed = DateTime.Now;
                db.Chemicals.Add(newDbObject);
                chemical.Id = newDbObject.Id;

                await db.SaveChangesAsync();
                await SendNewChemicalEmail(user, newDbObject);
            }

            return (await db.Chemicals.ProjectTo<Model.Chemical>(mapper.ConfigurationProvider).FirstOrDefaultAsync(ps => ps.Id == chemical.Id), null);
        }

        public async Task<(bool, IEnumerable<string>)> Approve(Guid chemicalId)
        {
            var validationErrors = new List<string>();

            var user = userResolver.GetCurrentUserId();

            var dbObject = await db.Chemicals
                    .FirstOrDefaultAsync(ps => ps.Id == chemicalId);

            if (dbObject != null)
            {
                dbObject.Tentative = false;
            }
            else
            {
                validationErrors.Add("Chemical does not exist");
            }

            if (validationErrors.Any()) return (false, validationErrors);

            await db.SaveChangesAsync();

            return (true, null);
        }

        private Db.Chemical HandleRelations(Model.Chemical dto, Db.Chemical dbObject)
        {
            // Nothing to do here yet
            return dbObject;
        }


        public async Task SendNewChemicalEmail(User user, Db.Chemical chemical)
        {
            if (chemical == null) return;

            var to = _config["chemicalEmail"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (to.Any())
            {
                (var subject, var html) = buildEmailContentForChemicalResponsible(user, chemical);
                await _mailSender.SendMail(to, subject, html);
            }
        }

        private (string, string) buildEmailContentForChemicalResponsible(User user, Db.Chemical chemical)
        {
            var emailTemplate = new StringBuilder().Append("{{change}}")
                                                    .Append("<br/>")
                                                    .Append("{{changedBy}}");

            var changedBy = "Added by " + user.Name + ". Contact on " + " (<a href=\"mailto:" + user.Email + "\">" + user.Email + "</>)";
            string subject = "New chemical added to the chemical register";
            string portalLink = "https://chemcom.equinor.com";

            if (_config["env"] == "dev")
            {
                subject = $"[Dev] {subject}";
                portalLink = "https://frontend-chemcom-dev.radix.equinor.com/";
            }

           
            var change = $"New chemical added {chemical.Name}";
            var link = "<a href=\"" + portalLink + "\">" + change + "</a>"; ;

            var html = emailTemplate
                .Replace("{{change}}", link)
                .Replace("{{changedBy}}", changedBy);

            return (subject, html.ToString());
        }
    }
}
