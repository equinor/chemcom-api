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
using static ChemDec.Api.Controllers.Handlers.ShipmentHandler;

namespace ChemDec.Api.Controllers.Handlers
{
    public class InstallationHandler
    {
        private readonly Db.ChemContext db;
        private readonly IMapper mapper;
        private readonly UserResolver userResolver;
        private readonly UserService userService;

        public InstallationHandler(Db.ChemContext db, IMapper mapper, UserResolver userResolver, UserService userService)
        {
            this.db = db;
            this.mapper = mapper;
            this.userResolver = userResolver;
            this.userService = userService;
        }

        public IQueryable<Installation> GetInstallations()
        {
            return db.Installations.ProjectTo<Installation>(mapper.ConfigurationProvider);
        }

        public async Task<(bool, IEnumerable<string>)> SaveReservoirData(Guid plantId, double? toc, double? nitrogen, double? water)
        {
            var user = await userService.GetCurrentUser();
            if (user.Roles.Any(a => a.Installation?.Id == plantId) == false)
                return (false, new List<string> { "User don't have access to change this shipment" });

            var installation = await db.Installations.FirstOrDefaultAsync(w => w.Id == plantId);
            if (installation == null)
                return (false, new List<string> { "Plant does not exist" });

            if (toc != null) installation.Toc = toc.Value;
            if (nitrogen != null) installation.Nitrogen = nitrogen.Value;
            if (water != null) installation.Water = water.Value;
            await db.SaveChangesAsync();
            return (true, null);

        }
        public async Task<(ReservoirData, IEnumerable<string>)> GetReservoirData(Guid plantId)
        {
            var user = await userService.GetCurrentUser();
            if (user.Roles.Any(a => a.Installation?.Id == plantId) == false)
                return (new ReservoirData(), new List<string> { "User don't have access to change this shipment" });

            var installation = await db.Installations.FirstOrDefaultAsync(w => w.Id == plantId);
            if (installation == null)
                return (new ReservoirData(), new List<string> { "Plant does not exist" });

            var res = db.ShipmentChemicals.AsQueryable();
            var resWater = db.ShipmentParts.AsQueryable();
            var pending = res.Where(w => w.Shipment.Status != Statuses.Approved && w.Shipment.Status != Statuses.Declined && w.Shipment.PlannedExecutionFrom.Date >= DateTime.Now.Date && w.Shipment.ReceiverId == plantId);
            var pendingTotalWater = resWater.Where(w => w.Shipment.Status != Statuses.Approved && w.Shipment.Status != Statuses.Declined && w.Shipment.PlannedExecutionFrom.Date >= DateTime.Now.Date && w.Shipment.ReceiverId == plantId);
            var approved = res.Where(w => w.Shipment.Status == Statuses.Approved && w.Shipment.PlannedExecutionFrom.Date >= DateTime.Now.Date && w.Shipment.ReceiverId == plantId);
            var approvedTotalWater = resWater.Where(w => w.Shipment.Status == Statuses.Approved && w.Shipment.PlannedExecutionFrom.Date >= DateTime.Now.Date && w.Shipment.ReceiverId == plantId);

            var tocApproved = await approved.SumAsync(s => s.CalculatedToc);
            var nitrogenApproved = await approved.SumAsync(s => s.CalculatedNitrogen);
            var waterApproved = await approvedTotalWater.SumAsync(s => s.Water);

            var tocPending = await pending.SumAsync(s => s.CalculatedToc);
            var nitrogenPending = await pending.SumAsync(s => s.CalculatedNitrogen);
            var waterPending = await pendingTotalWater.SumAsync(s => s.Water);

            var reservoirData = new ReservoirData
            {
                Toc = installation.Toc,
                Nitrogen = installation.Nitrogen,
                Water = installation.Water,
                TocApproved = tocApproved,
                NitrogenApproved = nitrogenApproved,
                WaterApproved = waterApproved,
                TocPending = tocPending,
                NitrogenPending = nitrogenPending,
                WaterPending = waterPending
            };
            return (reservoirData, null);
        }

        public async Task<(ReservoirData, IEnumerable<string>)> GetReservoirDataInKg(Guid plantId)
        {
            var user = await userService.GetCurrentUser();
            if (user.Roles.Any(a => a.Installation?.Id == plantId) == false)
                return (new ReservoirData(), new List<string> { "User don't have access to change this shipment" });

            var installation = await db.Installations.FirstOrDefaultAsync(w => w.Id == plantId);
            if (installation == null)
                return (new ReservoirData(), new List<string> { "Plant does not exist" });

            // Query shipment chemicals and parts
            var res = db.ShipmentChemicals.Include(c => c.Chemical).AsQueryable(); // Include Chemical for density
            var resWater = db.ShipmentParts.AsQueryable();

            // Filter shipments
            var pending = res.Where(w => w.Shipment.Status != Statuses.Approved && w.Shipment.Status != Statuses.Declined && w.Shipment.PlannedExecutionFrom.Date >= DateTime.Now.Date && w.Shipment.ReceiverId == plantId);
            var pendingTotalWater = resWater.Where(w => w.Shipment.Status != Statuses.Approved && w.Shipment.Status != Statuses.Declined && w.Shipment.PlannedExecutionFrom.Date >= DateTime.Now.Date && w.Shipment.ReceiverId == plantId);
            var approved = res.Where(w => w.Shipment.Status == Statuses.Approved && w.Shipment.PlannedExecutionFrom.Date >= DateTime.Now.Date && w.Shipment.ReceiverId == plantId);
            var approvedTotalWater = resWater.Where(w => w.Shipment.Status == Statuses.Approved && w.Shipment.PlannedExecutionFrom.Date >= DateTime.Now.Date && w.Shipment.ReceiverId == plantId);

            var tocApproved = await approved.SumAsync(s => s.CalculatedToc);
            var nitrogenApproved = await approved.SumAsync(s => s.CalculatedNitrogen);
            var waterApproved = await approvedTotalWater.SumAsync(s => s.Water);
            var waterApprovedInKg = ConvertToKg(waterApproved, "m3", 1, 1);

            var tocPending = await pending.SumAsync(s => s.CalculatedToc);
            var nitrogenPending = await pending.SumAsync(s => s.CalculatedNitrogen);
            var waterPending = await pendingTotalWater.SumAsync(s => s.Water);
            var waterPendingInKg = ConvertToKg(waterPending, "m3", 1, 1);

            // Convert installation values to kg
            var tocInstallation = ConvertToKg(installation.Toc, "ppm",1, installation.Water); 
            var nitrogenInstallation = ConvertToKg(installation.Nitrogen, "ppm",1, installation.Water);
            var waterInstallation = ConvertToKg(installation.Water, "m3", 1, 1);

            var reservoirData = new ReservoirData
            {
                Toc = tocInstallation,
                Nitrogen = nitrogenInstallation,
                Water = waterInstallation,
                TocApproved = tocApproved,
                NitrogenApproved = nitrogenApproved,
                WaterApproved = waterApproved,
                TocPending = tocPending,
                NitrogenPending = nitrogenPending,
                WaterPending = waterPending
            };

            return (reservoirData, null);
        }

        private double ConvertToKg(double value, string unit, double density, double referenceWaterVolume)
        {
            switch (unit.ToLower())
            {
                case "kg":
                    return value;
                case "tonn":
                    return value * 1_000; // 1 tonn = 1,000 kg
                case "m3":
                    return value * density * 1000;
                case "l":
                    return value * density; // 1 l = density / 1,000 kg
                case "ppm":
                    return value * referenceWaterVolume * 1_000 * 1e-6; // ppm to kg: value * waterVolume(m³) * 1e-6
                default:
                    return value;
            }
        }



        public async Task<(Installation, IEnumerable<string>)> SaveOrUpdate(Installation installation)
        {
            var validationErrors = new List<string>();

            var user = userResolver.GetCurrentUserId();

            if (string.IsNullOrEmpty(installation.Name))
            {
                validationErrors.Add("Installation name must be set");
            }

            if (installation.InstallationType != "plant" && installation.InstallationType != "platform")
            {
                validationErrors.Add("Installation type must be plant or platform");
            }

            //etc etc

            Db.Installation dbObject = null;
            if (installation.Id != Guid.Empty)
            {
                dbObject = await db.Installations
                    .FirstOrDefaultAsync(ps => ps.Id == installation.Id);
            }



            if (validationErrors.Any()) return (null, validationErrors);


            if (dbObject != null)
            {
                //TODO: User check to see if user can update shipments

                mapper.Map(installation, dbObject);
                dbObject = HandleRelations(installation, dbObject);
            }
            else
            {
                //TODO: User check. 

                var newDbObject = mapper.Map<Db.Installation>(installation);
                if (newDbObject.Id == Guid.Empty)
                    newDbObject.Id = Guid.NewGuid();
                newDbObject = HandleRelations(installation, newDbObject);
                db.Installations.Add(newDbObject);
                installation.Id = newDbObject.Id;
            }
            await db.SaveChangesAsync();

            return (await db.Installations.ProjectTo<Installation>(mapper.ConfigurationProvider).FirstOrDefaultAsync(ps => ps.Id == installation.Id), null);
        }


        private Db.Installation HandleRelations(Installation dto, Db.Installation dbObject)
        {

            return dbObject;
        }


    }
}
