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

namespace ChemDec.Api.Controllers.Handlers
{
    public class ChemicalHandler
    {
        private readonly Db.ChemContext db;
        private readonly IMapper mapper;
        private readonly UserResolver userResolver;

        public ChemicalHandler(Db.ChemContext db, IMapper mapper, UserResolver userResolver)
        {
            this.db = db;
            this.mapper = mapper;
            this.userResolver = userResolver;
        }

        public IQueryable<Chemical> GetChemicals()
        {
            return db.Chemicals.ProjectTo<Chemical>(mapper.ConfigurationProvider);
        }

        public async Task<(Chemical, IEnumerable<string>)> SaveOrUpdate(Chemical chemical)
        {
            var validationErrors = new List<string>();
           
            var user = userResolver.GetCurrentUserId();

           /* Code no longer mandatory
            * if (string.IsNullOrEmpty(chemical.Code))
            {
                validationErrors.Add("Chemical code must be set");
            }*/

            if (string.IsNullOrEmpty(chemical.Name))
            {
                validationErrors.Add("Chemical name must be set");
            }
            if(chemical.Name.Contains(';'))
            {
                validationErrors.Add("Chemical name cannot contain semicolons.");
            }
            if (chemical.Description.Contains(';'))
            {
                validationErrors.Add("Chemical description cannot contain semicolons.");
            }

            /*
            if (string.IsNullOrEmpty(chemical.Description))
            {
                validationErrors.Add("Chemical description must be set");
            }
            */

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
                    var shipmentsToUpdate = await db.ShipmentChemicals.Include(i=>i.Shipment).Where(w => w.ChemicalId == dbObject.Id).ToListAsync();
                    foreach (var shipment in shipmentsToUpdate)
                    {
                        ShipmentHandler.CalculateChemicals(shipment.Shipment.RinsingOffshorePercent, shipment, dbObject);
                    }
                }

            }
            else
            {
                // User check. Only chem administrator can add non-tentative chemicals

                var newDbObject = mapper.Map<Db.Chemical>(chemical);
                if (newDbObject.Id == Guid.Empty)
                    newDbObject.Id = Guid.NewGuid();
                newDbObject = HandleRelations(chemical, newDbObject);
                db.Chemicals.Add(newDbObject);
                chemical.Id = newDbObject.Id;
            }
            await db.SaveChangesAsync();

            return (await db.Chemicals.ProjectTo<Chemical>(mapper.ConfigurationProvider).FirstOrDefaultAsync(ps => ps.Id == chemical.Id), null);
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

        private Db.Chemical HandleRelations(Chemical dto, Db.Chemical dbObject)
        {
            // Nothing to do here yet
            return dbObject;
        }


    }
}
