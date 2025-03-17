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
            var validationErrors = ValidateChemical(chemical);
            if (validationErrors.Any()) return (null, validationErrors);

            var user = await _userService.GetCurrentUser();
            var dbObject = await GetExistingChemical(chemical);

            if (dbObject != null)
            {
                await UpdateChemical(chemical, dbObject);
            }
            else
            {
                await AddNewChemical(chemical, user);
            }

            var savedChemical = await db.Chemicals
                .ProjectTo<Model.Chemical>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ps => ps.Id == chemical.Id);

            return (savedChemical, null);
        }

        private List<string> ValidateChemical(Model.Chemical chemical)
        {
            var validationErrors = new List<string>();

            chemical.Name = chemical.Name.Trim();
            chemical.Description = chemical.Description.Trim();

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

            var existingChemical = db.Chemicals
                .FirstOrDefaultAsync(ps => ps.Name.Trim().ToLower().Equals(chemical.Name.Trim().ToLower(), StringComparison.InvariantCultureIgnoreCase) && ps.Id != chemical.Id).Result;

            if (existingChemical != null)
            {
                validationErrors.Add("Chemical with name " + chemical.Name + " already exists!");
            }

            return validationErrors;
        }

        private async Task<Db.Chemical> GetExistingChemical(Model.Chemical chemical)
        {
            if (chemical.Id != Guid.Empty)
            {
                return await db.Chemicals
                    .FirstOrDefaultAsync(ps => ps.Id == chemical.Id);
            }
            return null;
        }

        private async Task UpdateChemical(Model.Chemical chemical, Db.Chemical dbObject)
        {
            var tentative = dbObject.Tentative;
            var tocWeight = dbObject.TocWeight;
            var nWeight = dbObject.NitrogenWeight;
            var density = dbObject.Density;

            mapper.Map(chemical, dbObject);
            dbObject.Tentative = tentative; // can only be approved through Approve-method
            dbObject = HandleRelations(chemical, dbObject);

            if (dbObject.TocWeight != tocWeight || dbObject.NitrogenWeight != nWeight || dbObject.Density != density)
            {
                await RecalculateShipments(dbObject);
            }

            await db.SaveChangesAsync();
        }

        private async Task RecalculateShipments(Db.Chemical dbObject)
        {
            var shipmentsToUpdate = await db.ShipmentChemicals
                .Include(i => i.Shipment)
                .Where(w => w.ChemicalId == dbObject.Id)
                .ToListAsync();

            foreach (var shipment in shipmentsToUpdate)
            {
                ShipmentHandler.CalculateChemicals(shipment.Shipment.RinsingOffshorePercent, shipment, dbObject);
            }
        }

        private async Task AddNewChemical(Model.Chemical chemical, User user)
        {
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
