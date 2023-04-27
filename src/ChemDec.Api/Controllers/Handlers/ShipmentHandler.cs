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
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;
using Azure.Storage.Blobs;
using Azure.Storage;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ChemDec.Api.Controllers.Handlers
{
    public class ShipmentHandler
    {
        private readonly Db.ChemContext db;
        private readonly IMapper mapper;
        private readonly UserResolver userResolver;
        private readonly UserService userService;
        private readonly IConfiguration config;
        private readonly MailSender mailSender;
        private readonly LoggerHelper loggerHelper;
        private TelemetryClient telemetry = new TelemetryClient();

        public ShipmentHandler(Db.ChemContext db, IMapper mapper, UserResolver userResolver, UserService userService, IConfiguration config, MailSender mailSender, LoggerHelper loggerHelper)
        {
            this.db = db;
            this.mapper = mapper;
            this.userResolver = userResolver;
            this.userService = userService;
            this.config = config;
            this.mailSender = mailSender;
            this.loggerHelper = loggerHelper;
        }

        public IQueryable<Shipment> GetShipments()
        {
            return db.Shipments.ProjectTo<Shipment>(mapper.ConfigurationProvider);
        }

        public IQueryable<ShipmentInfo> GetShipmentsInfo()
        {
            return db.Shipments.ProjectTo<ShipmentInfo>(mapper.ConfigurationProvider);
        }
        class GraphGroup
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }

        }

        private List<GraphFlat> addEmptyItems(DateTime from, DateTime to, List<GraphFlat> input, Func<DateTime, DateTime, int> steps, Func<DateTime, int, GraphFlat> addEmptyObject)
        {
            var range = Enumerable.Range(0, Math.Abs(steps(from, to)));
            List<GraphFlat> otherGraph = new List<GraphFlat>();
            foreach (var item in range)
            {
                var date = addEmptyObject(from, item);
                var dateString = date.Date;//.Year + (date.Month != 0 ? "/" + date.Month.ToString() : string.Empty) + (date.Day != 0 ? "/" + date.Day.ToString() : string.Empty);
                var n = input.FirstOrDefault(w => w.Date == dateString);
                if (n == null)
                {
                    otherGraph.Add(date);
                }
                else otherGraph.Add(n);
            }
            return otherGraph;
        }

        public async Task<GraphData> GetSummaryForGraph(Guid? fromInstallationId, Guid? toInstallationId, DateTime? from, DateTime? to, string timeZone, bool excludeDraft = true, string groupBy = "day", Guid? exceptShipment = null)
        {
            int timeDiff = GetTimeDiff(timeZone);

            if (from != null) from = new DateTime(from.Value.Year, from.Value.Month, from.Value.Day, 0, 0, 0);
            if (to != null) to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59);



            var res = db.ShipmentChemicals.AsQueryable();
            var resWater = db.ShipmentParts.AsQueryable();

            if (fromInstallationId != null)
            {
                res = res.Where(w => w.Shipment.SenderId == fromInstallationId);
                resWater = resWater.Where(w => w.Shipment.SenderId == fromInstallationId);
            }

            if (exceptShipment != null)
            {
                res = res.Where(w => w.ShipmentId != exceptShipment);
                resWater = resWater.Where(w => w.ShipmentId != exceptShipment);
            }

            if (toInstallationId != null)
            {
                res = res.Where(w => w.Shipment.Receiver.Id == toInstallationId);
                resWater = resWater.Where(w => w.Shipment.Receiver.Id == toInstallationId);
            }

            if (from != null)
            {
                res = res.Where(w => w.Shipment.PlannedExecutionFrom.AddHours(timeDiff) >= from);
                resWater = resWater.Where(w => w.Shipped.AddHours(timeDiff) >= from);
            }

            if (to != null)
            {
                res = res.Where(w => w.Shipment.PlannedExecutionFrom.AddHours(timeDiff) <= to);
                resWater = resWater.Where(w => w.Shipped.AddHours(timeDiff) <= to);
            }
            GraphData data = null;
            List<GraphFlat> chemicals = null;
            List<GraphFlat> pendingChemicals = null;
            List<GraphFlat> water = null;
            List<GraphFlat> pendingWater = null;

            if (excludeDraft)
            {
                res = res.Where(w => w.Shipment.Status != Statuses.Draft);
                resWater = resWater.Where(w => w.Shipment.Status != Statuses.Draft);
            }
            //TODO: How to show declined in graph? Hiding them for now
            var approved = res.Where(w => w.Shipment.Status == Statuses.Approved);
            var pending = res.Where(w => w.Shipment.Status != Statuses.Approved && w.Shipment.Status != Statuses.Declined);

            var approvedTotalWater = resWater.Where(w => w.Shipment.Status == Statuses.Approved);
            var pendingTotalWater = resWater.Where(w => w.Shipment.Status != Statuses.Approved && w.Shipment.Status != Statuses.Declined);

            // The following sections are very verbose. The reason for this it's that it is very easy
            // to break EF GroupBy at SQL-level. All attempts at refactoring this has resulted in in-memory grouping 
            // (which will be very expensive). Take care when adding to the sections below and check the result SQL

            if (groupBy == "hour")
            {

                chemicals = await approved.GroupBy(g => new { g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Year, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Month, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Day, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Hour })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        Hour = g.Key.Hour,
                        Toc = g.Sum(x => x.CalculatedToc),
                        Nitrogen = g.Sum(x => x.CalculatedNitrogen),
                        Biocides = g.Sum(x => x.CalculatedBiocides),

                    }).ToListAsync();


                pendingChemicals = await pending.GroupBy(g => new { g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Year, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Month, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Day, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Hour })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        Hour = g.Key.Hour,
                        TocPending = g.Sum(x => x.CalculatedToc),
                        NitrogenPending = g.Sum(x => x.CalculatedNitrogen),
                        BiocidesPending = g.Sum(x => x.CalculatedBiocides),

                    }).ToListAsync();

                water = await approvedTotalWater.GroupBy(g => new { g.Shipped.AddHours(timeDiff).Year, g.Shipped.AddHours(timeDiff).Month, g.Shipped.AddHours(timeDiff).Day, g.Shipped.AddHours(timeDiff).Hour })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        Hour = g.Key.Hour,
                        Water = g.Sum(x => x.Water),
                    }).ToListAsync();

                pendingWater = await pendingTotalWater.GroupBy(g => new { g.Shipped.AddHours(timeDiff).Year, g.Shipped.AddHours(timeDiff).Month, g.Shipped.AddHours(timeDiff).Day, g.Shipped.AddHours(timeDiff).Hour })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        Hour = g.Key.Hour,
                        PendingWater = g.Sum(x => x.Water),
                    }).ToListAsync();

                if (from != null && to != null) //Add "empty" hours
                {
                    chemicals = addEmptyItems(from.Value, to.Value, chemicals, (f, t) => Convert.ToInt32(Math.Ceiling((t - f).TotalHours)), (d, s) => { var newDate = d.AddHours(s); return new GraphFlat { Year = newDate.Year, Month = newDate.Month, Day = newDate.Day, Hour = newDate.Hour }; });
                }

            }
            if (groupBy == "day")
            {

                chemicals = await approved.GroupBy(g => new { g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Year, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Month, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Day })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        Toc = g.Sum(x => x.CalculatedToc),
                        Nitrogen = g.Sum(x => x.CalculatedNitrogen),
                        Biocides = g.Sum(x => x.CalculatedBiocides),
                    }).ToListAsync();


                pendingChemicals = await pending.GroupBy(g => new { g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Year, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Month, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Day })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        TocPending = g.Sum(x => x.CalculatedToc),
                        NitrogenPending = g.Sum(x => x.CalculatedNitrogen),
                        BiocidesPending = g.Sum(x => x.CalculatedBiocides),
                    }).ToListAsync();

                water = await approvedTotalWater.GroupBy(g => new { g.Shipped.AddHours(timeDiff).Year, g.Shipped.AddHours(timeDiff).Month, g.Shipped.AddHours(timeDiff).Day })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        Water = g.Sum(x => x.Water),
                    }).ToListAsync();

                pendingWater = await pendingTotalWater.GroupBy(g => new { g.Shipped.AddHours(timeDiff).Year, g.Shipped.AddHours(timeDiff).Month, g.Shipped.AddHours(timeDiff).Day })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        PendingWater = g.Sum(x => x.Water),
                    }).ToListAsync();

                if (from != null && to != null) //Add "empty" days
                {
                    chemicals = addEmptyItems(from.Value, to.Value, chemicals, (f, t) => (t - f).Days + 1, (d, s) => { var newDate = d.AddDays(s); return new GraphFlat { Year = newDate.Year, Month = newDate.Month, Day = newDate.Day }; });
                }

            }

            if (groupBy == "month")
            {
                chemicals = await approved.GroupBy(g => new { g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Year, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Month })
                .Select(g => new GraphFlat
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Toc = g.Sum(x => x.CalculatedToc),
                    Nitrogen = g.Sum(x => x.CalculatedNitrogen),
                    Biocides = g.Sum(x => x.CalculatedBiocides),
                }).ToListAsync();


                pendingChemicals = await pending.GroupBy(g => new { g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Year, g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Month })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TocPending = g.Sum(x => x.CalculatedToc),
                        NitrogenPending = g.Sum(x => x.CalculatedNitrogen),
                        BiocidesPending = g.Sum(x => x.CalculatedBiocides),
                    }).ToListAsync();

                water = await approvedTotalWater.GroupBy(g => new { g.Shipped.AddHours(timeDiff).Year, g.Shipped.AddHours(timeDiff).Month })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Water = g.Sum(x => x.Water),
                    }).ToListAsync();

                pendingWater = await pendingTotalWater.GroupBy(g => new { g.Shipped.AddHours(timeDiff).Year, g.Shipped.AddHours(timeDiff).Month })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        PendingWater = g.Sum(x => x.Water),
                    }).ToListAsync();

                if (from != null && to != null) //Add "empty" months
                {
                    chemicals = addEmptyItems(from.Value, to.Value, chemicals, (f, t) => ((t.Year - f.Year) * 12) + (f.Month - t.Month) + 1, (d, s) => { var newDate = d.AddMonths(s); return new GraphFlat { Year = newDate.Year, Month = newDate.Month }; });
                }


            }
            if (groupBy == "year")
            {
                chemicals = await approved.GroupBy(g => new { g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Year })
                 .Select(g => new GraphFlat
                 {
                     Year = g.Key.Year,
                     Toc = g.Sum(x => x.CalculatedToc),
                     Nitrogen = g.Sum(x => x.CalculatedNitrogen),
                     Biocides = g.Sum(x => x.CalculatedBiocides),
                 }).ToListAsync();


                pendingChemicals = await pending.GroupBy(g => new { g.Shipment.PlannedExecutionFrom.AddHours(timeDiff).Year })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        TocPending = g.Sum(x => x.CalculatedToc),
                        NitrogenPending = g.Sum(x => x.CalculatedNitrogen),
                        BiocidesPending = g.Sum(x => x.CalculatedBiocides),
                    }).ToListAsync();

                water = await approvedTotalWater.GroupBy(g => new { g.Shipped.AddHours(timeDiff).Year })
                    .Select(g => new GraphFlat
                    {
                        Year = g.Key.Year,
                        Water = g.Sum(x => x.Water),
                    }).ToListAsync();

                pendingWater = await pendingTotalWater.GroupBy(g => new { g.Shipped.AddHours(timeDiff).Year })
                       .Select(g => new GraphFlat
                       {
                           Year = g.Key.Year,
                           PendingWater = g.Sum(x => x.Water),
                       }).ToListAsync();

                if (from != null && to != null) //Add "empty" years
                {
                    chemicals = addEmptyItems(from.Value, to.Value, chemicals, (f, t) => t.Year - f.Year + 1, (d, s) => { var newDate = d.AddYears(s); return new GraphFlat { Year = newDate.Year }; });
                }

            }

            if (groupBy == "total")
            {

                chemicals = new List<GraphFlat>
                {
                    new GraphFlat
                        {
                          Toc = await approved.SumAsync(s => s.CalculatedToc),
                          Nitrogen = await approved.SumAsync(s => s.CalculatedNitrogen),
                          Biocides = await approved.SumAsync(s => s.CalculatedBiocides)
                          }
                };

                pendingChemicals = new List<GraphFlat>
                {
                    new GraphFlat
                        {
                          TocPending = await pending.SumAsync(s => s.CalculatedToc),
                          NitrogenPending = await pending.SumAsync(s => s.CalculatedNitrogen),
                          BiocidesPending = await pending.SumAsync(s => s.CalculatedBiocides)
                          }
                };

                water = new List<GraphFlat>
                {
                    new GraphFlat
                        {
                           Water = await approvedTotalWater.SumAsync(s => s.Water),
                       }
                };

                pendingWater = new List<GraphFlat>
                {
                    new GraphFlat
                        {
                           PendingWater = await pendingTotalWater.SumAsync(s => s.Water),
                       }
                };

            }

            foreach (var item in water)
            {
                //Merge water into final list
                var chemItem = chemicals.FirstOrDefault(w => w.Date == item.Date);
                if (chemItem != null) chemItem.Water = item.Water;
            }
            foreach (var item in pendingWater)
            {
                //Merge pending water into final list
                var chemItem = chemicals.FirstOrDefault(w => w.Date == item.Date);
                if (chemItem != null) chemItem.PendingWater = item.PendingWater;
            }
            foreach (var item in pendingChemicals)
            {
                //Merge pending chemical into final list
                var chemItem = chemicals.FirstOrDefault(w => w.Date == item.Date);
                if (chemItem != null)
                {
                    chemItem.NitrogenPending = item.NitrogenPending;
                    chemItem.TocPending = item.TocPending;
                    chemItem.BiocidesPending = item.BiocidesPending;
                }
            }

            Func<GraphFlat, Func<GraphFlat, double>, DataItem> getData = (g, getter) => new DataItem { Y = getter(g), HasBiocides = g.Biocides > 0.0 || g.BiocidesPending > 0.0, Metric = "kg" };
            Func<GraphFlat, Func<GraphFlat, double>, string, DataItem> getDataWithMetric = (g, getter, metric) => new DataItem { Y = getter(g), HasBiocides = g.Biocides > 0.0 || g.BiocidesPending > 0.0, Metric = metric };
            data = new GraphData
            {
                ShipmentDates = chemicals.Select(s => new GraphLabel { Label = s.Date }).ToList(),
                Chemicals = new List<GraphItem>
                {
                    new GraphItem{Name="TOC (pending)", Color="#dadada", Stack="TOC", Data=chemicals.Select(s=>getData(s, i=>i.TocPending)).ToList(), MaxPointWidth=30},
                    new GraphItem{Name="TOC", Stack="TOC",Color="#e88", Data=chemicals.Select(s=> getData(s, i=>i.Toc)).ToList(), MaxPointWidth=30},
                    new GraphItem{Name="Nitrogen (pending)",Color="#9d9d9c", Stack="Nitrogen", Data=chemicals.Select(s=>getData(s, i=>i.NitrogenPending)).ToList(), MaxPointWidth=30},
                    new GraphItem{Name="Nitrogen", Stack="Nitrogen",Color="#7c7", Data=chemicals.Select(s=>getData(s, i=>i.Nitrogen)).ToList(), MaxPointWidth=30},
                  //  new GraphItem{Name="Biocides", Stack="Biocides", Data=chemicals.Select(s=>s.Biocides).ToList()},
                    new GraphItem{Name="Water (pending)", YAxis=1, Stack="Water",Color="#cae1f7", Data=chemicals.Select(s=>getDataWithMetric(s, i=>i.PendingWater,"m³")).ToList(), MaxPointWidth=30},
                    new GraphItem{Name="Water", Stack="Water", YAxis=1, Color="#7cb5ec", Data=chemicals.Select(s=>getDataWithMetric(s, i=>i.Water,"m³") ).ToList(), MaxPointWidth=30 }
                },
            };
            for (int i = 0; i < data.ShipmentDates.Count; i++)
            {
                data.ShipmentDates[i].HasBiocides = data.Chemicals[0].Data[i].HasBiocides;
            }
            var approvedList = approved.ToList();
            var pendingList = pending.ToList();
            return data;
        }

        private static int GetTimeDiff(string timeZone)
        {
            int timeDiff = 0;
            TimeZoneInfo timeZoneInfo = null;
            if (!string.IsNullOrWhiteSpace(timeZone))
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var utcTime = DateTime.UtcNow;
                var timeInTimeZone = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
                timeDiff = timeInTimeZone.Subtract(utcTime).Hours;
            }

            return timeDiff;
        }

        public async Task<List<ShipmentChemicalTableItem>> GetChemicalHistoryForPlant(
            Guid? toInstallationId, string timeZone, DateTime? from, DateTime? to)
        {
            //if (filter == null) filter = new Filter();
            int timeDiff = GetTimeDiff(timeZone);

            //collect and filter data:
            var resShipmentChemicals = db.ShipmentChemicals.AsQueryable();
            var resWater = db.ShipmentParts.AsQueryable();
            var resInstallations = db.InstallationPlants.AsQueryable();

            if (toInstallationId != null)
            {
                resShipmentChemicals = resShipmentChemicals.Where(w => w.Shipment.Receiver.Id == toInstallationId);
                resWater = resWater.Where(w => w.Shipment.Receiver.Id == toInstallationId);
            }

            //if (filter.SenderInstallations != null && filter.SenderInstallations.Any()) // fromInstallationId != null)
            //{
            //    //resShipmentChemicals = resShipmentChemicals.Where(w => w.Shipment.SenderId == fromInstallationId);
            //    //resWater = resWater.Where(w => w.Shipment.SenderId == fromInstallationId);
            //}
            if (from != null)
            {
                resShipmentChemicals = resShipmentChemicals.Where(w => w.Shipment.PlannedExecutionFrom.AddHours(timeDiff) >= from);
                resWater = resWater.Where(w => w.Shipped.AddHours(timeDiff) >= from);
            }

            if (to != null)
            {
                resShipmentChemicals = resShipmentChemicals.Where(w => w.Shipment.PlannedExecutionFrom.AddHours(timeDiff) <= to);
                resWater = resWater.Where(w => w.Shipped.AddHours(timeDiff) <= to);
            }

            // exclude Draft for incoming to plant            
            resShipmentChemicals = resShipmentChemicals.Where(w => w.Shipment.Status != Statuses.Draft);
            resWater = resWater.Where(w => w.Shipment.Status != Statuses.Draft);

            var groupedBySenderAndChemical = resShipmentChemicals
                .Where(w => w.Shipment.Status != Statuses.Declined)
                .Include(sc => sc.Chemical)
                .Include(sc => sc.Shipment)
                .ThenInclude(sh => sh.Sender)
                .GroupBy(g => new { g.Shipment.SenderId, g.ChemicalId });

            //extract list of chemicalHistory data
            var tableItems = groupedBySenderAndChemical
                .Select(scGroup =>
               new ShipmentChemicalTableItem
               {
                   ChemicalName = scGroup.FirstOrDefault().Chemical.Name,
                   Description = scGroup.FirstOrDefault().Chemical.Description,
                   Density = scGroup.FirstOrDefault().Chemical.Density,
                   HazardClass = scGroup.FirstOrDefault().Chemical.HazardClass,
                   MeasureUnitDefault = scGroup.FirstOrDefault().Chemical.MeasureUnitDefault,
                   FollowOilPhaseDefault = scGroup.FirstOrDefault().Chemical.FollowOilPhaseDefault,
                   FollowWaterPhaseDefault = scGroup.FirstOrDefault().Chemical.FollowWaterPhaseDefault,
                   FromInstallation = scGroup.FirstOrDefault().Shipment.Sender.Name,
                   Weight = scGroup.Sum(x => x.CalculatedWeight),
                   TocWeight = scGroup.Sum(x => x.CalculatedToc),
                   NitrogenWeight = scGroup.Sum(x => x.CalculatedNitrogen),
                   BiocideWeight = scGroup.Sum(x => x.CalculatedBiocides)
               });
            //var test3 = tableItems2.ToList();


            var shipmentChemicalTableItems = await tableItems.ToListAsync();
            return shipmentChemicalTableItems;
        }


        public enum Operation { Change, Submit, Approve, Decline, SaveEvaluation }
        public enum DetailedOperation
        {
            Saved, NewAttachment, DeleteAttachment, AddComment,
            RemoveComment, SavedEvaluation
        }

        public enum Initiator { Onshore, Offshore }
        public class Statuses
        {
            public const string Draft = "Draft";
            public const string Submitted = "Submitted";
            public const string Changed = "Changed";
            public const string Approved = "Approved";
            public const string Executed = "Executed";
            public const string Declined = "Declined";

        }

        static string OperationStatus(Operation operation, string prevStatus, Db.ChemContext db)
        {
            db.ChangeTracker.DetectChanges();
            var shipmentOrChemicalsChange = db.ChangeTracker.Entries().Where(x => (x.Entity is Db.Shipment || x.Entity is Db.ShipmentChemical) && (x.State == EntityState.Added || x.State == EntityState.Modified)).Any();


            switch (operation)
            {
                case Operation.Change: return (prevStatus == Statuses.Draft || prevStatus == null) ? Statuses.Draft : shipmentOrChemicalsChange ? Statuses.Changed : prevStatus;
                case Operation.Submit: return (prevStatus == Statuses.Draft || prevStatus == null) ? Statuses.Submitted : Statuses.Changed;
                case Operation.Approve: return Statuses.Approved;
                case Operation.Decline: return Statuses.Declined;
                default: return Statuses.Draft;
            }
        }


        private string CheckIfUserCanChangeShipment(string status, Operation operation, Guid shipsFrom, User user)
        {
            var role = user.Roles.FirstOrDefault(u => u.Id == shipsFrom.ToString());
            var receiver = role.Installation.ShipsTo.FirstOrDefault();

            if ((operation == Operation.Change && (status == null || status == Statuses.Draft)) || operation == Operation.Submit)
            {
                if (user.Roles.Any(a => a.Installation?.Id == shipsFrom) == false)
                    return "User don't have access to register shipments from this installation";
            }

            if (operation == Operation.Submit)
            {
                if (status != Statuses.Draft && status != null) return "Can't re-submit shipment with status " + status;
            }

            if (operation == Operation.Change)
            {
                if (receiver != null && user.Roles.Any(a => a.Installation?.Id == shipsFrom || a.Installation?.Id == receiver.Id) == false)
                    return "User don't have access to change this shipment";
            }
            if (operation == Operation.Approve)
            {

                if (receiver != null && !user.Roles.Any(a => a.Id == receiver.Id.ToString()))
                    return "User don't have access to approve this shipment";

            }
            if (operation == Operation.SaveEvaluation)
            {
                if (receiver != null && user.Roles.Any(a => a.Installation?.Id == receiver.Id) == false)
                    return "User don't have access to approve this shipment";
            }

            return null;
        }

        private string CheckIfShipmentCanBeChanged(string status, Operation operation, Shipment shipment)
        {
            if (status == null && operation != Operation.Change && operation != Operation.Submit)
            {
                return "Shipment must be submitted before " + operation.ToString();
            }

            if (operation == Operation.Submit)
            {
                if (status != null && status != Statuses.Draft) return "Can't re-submit shipment with status " + status;
                if (shipment.WaterAmount != shipment.ShipmentParts.Sum(s => s.Water)) return "Sum of days does not match amount of water";
            }

            if (operation == Operation.Change)
            {
                if (status != null && status != Statuses.Draft && shipment.WaterAmount != shipment.ShipmentParts.Sum(s => s.Water)) return "Sum of days does not match amount of water";
                // Add correct check
                //if (status != OperationStatus[Operation.Draft]) return "Can't change shipment with status " + status;
            }
            if (operation == Operation.Approve)
            {
                // Add correct check
                //if (status != OperationStatus[Operation.Draft]) return "Can't change shipment with status " + status;
            }

            return null;
        }

        private Db.Shipment GetShipment(Guid id)
        {
            if (id != Guid.Empty)
            {
                return db.Shipments.Include(i => i.Chemicals).Include(i => i.Comments).Include(i => i.Attachments).Include(i => i.ShipmentParts)
                    .FirstOrDefault(ps => ps.Id == id);
            }
            return null;
        }

        private List<string> CheckIfUserCanSaveEvaluation(Db.Shipment savedShipment, Shipment shipment, User user)
        {
            var permissionCheck = CheckIfUserCanChangeShipment(savedShipment?.Status, Operation.Approve, shipment.SenderId, user);
            if (permissionCheck != null)
            {
                var validationErrors = new List<string>
                {
                    permissionCheck
                };
                return validationErrors;
            }
            return null;
        }

        private void UpdateEvaluationValues(Db.Shipment savedShipment, Shipment sourceShipment, User user)
        {
            //Check which settings changed and set updatedBy if they did:
            if (savedShipment.EvalAmountOk != sourceShipment.EvalAmountOk)
            {
                savedShipment.EvalAmountOk = sourceShipment.EvalAmountOk;
                savedShipment.EvalAmountOkUpdatedBy = user.Name;
            }
            if (savedShipment.EvalBiocidesOk != sourceShipment.EvalBiocidesOk)
            {
                savedShipment.EvalBiocidesOk = sourceShipment.EvalBiocidesOk;
                savedShipment.EvalBiocidesOkUpdatedBy = user.Name;
            }
            if (savedShipment.EvalCapacityOk != sourceShipment.EvalCapacityOk)
            {
                savedShipment.EvalCapacityOk = sourceShipment.EvalCapacityOk;
                savedShipment.EvalCapacityOkUpdatedBy = user.Name;
            }
            if (savedShipment.EvalContaminationRisk != sourceShipment.EvalContaminationRisk)
            {
                savedShipment.EvalContaminationRisk = sourceShipment.EvalContaminationRisk;
                savedShipment.EvalContaminationRiskUpdatedBy = user.Name;
            }
            //settings without updated by:
            savedShipment.EvalEnvImpact = sourceShipment.EvalEnvImpact;
            savedShipment.EvalComments = sourceShipment.EvalComments;
        }

        public async Task<(Shipment, IEnumerable<string>)> SaveShipmentEvaluation(Shipment shipment, Initiator initiator, string comment, string attachment)
        {
            var user = await userService.GetCurrentUser();
            var savedShipment = this.GetShipment(shipment.Id);
            var operation = Operation.SaveEvaluation; //checks same as approval
            var details = DetailedOperation.SavedEvaluation;

            var validationErrors = CheckIfUserCanSaveEvaluation(savedShipment, shipment, user);
            if (validationErrors != null && validationErrors.Any()) return (null, validationErrors);

            if (savedShipment != null)
            {
                if (shipment.Updated < savedShipment.Updated)
                {
                    return (null, new List<string> { "This form has been updated while you have been working on it. Refresh the page to load the most recent version" });
                }
                UpdateEvaluationValues(savedShipment, shipment, user);

                var sender = await db.Installations.Where(w => w.Id == shipment.SenderId).ProjectTo<PlantReference>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
                var role = user.Roles.FirstOrDefault(u => u.Id == shipment.SenderId.ToString());
                var receiver = role.Installation.ShipsTo.FirstOrDefault();
                var plant = await db.Installations.Where(w => w.Id == receiver.Id).ProjectTo<PlantReference>(mapper.ConfigurationProvider).FirstOrDefaultAsync();

                savedShipment.Updated = DateTime.Now;
                await db.SaveChangesAsync();

                loggerHelper.LogEvent(telemetry, user, sender, plant, operation, details, "ShipmentEvaluationSaved", shipment);
                await SendShipmentChangedMail(shipment, initiator, operation, details, comment, attachment, user, null, savedShipment.Status, sender, plant);
            }
            return (await db.Shipments.ProjectTo<Shipment>(mapper.ConfigurationProvider).FirstOrDefaultAsync(ps => ps.Id == shipment.Id), null);
        }


        private void ValidateIsSenderIdSet(Shipment shipment, List<string> validationErrors)
        {
            if (shipment.SenderId == Guid.Empty)
            {
                validationErrors.Add("sender.id must be set");
            }
        }

        private void ValidateIshipmentDaysSet(Shipment shipment, List<string> validationErrors)
        {
            if (shipment.ShipmentParts == null)
            {
                validationErrors.Add("Shipment days must be set");
            }
        }

        private void ValidateReceiverIsSet(Guid? receiverId, List<string> validationErrors)
        {
            if (receiverId == null)
            {
                validationErrors.Add("Installation is not set up to ship to anyone");
            }
        }

        private void ValidateAccessToSaveFromThisInstallation(Initiator initiator, User user, Shipment shipment, List<string> validationErrors)
        {
            if (initiator == Initiator.Offshore && user != null && user.Roles.Any(s => s.Installation != null && s.Installation.Id == shipment.SenderId) == false)
            {
                validationErrors.Add("You do not have access to save from this installation");
            }
        }

        private void ValidateAccessToSaveFromThisPlant(Initiator initiator, User user, Shipment shipment, List<string> validationErrors)
        {
            var sender = db.Installations.Where(w => w.Id == shipment.SenderId).ProjectTo<PlantReference>(mapper.ConfigurationProvider).FirstOrDefault();
            var role = user.Roles.FirstOrDefault(u => u.Id == shipment.SenderId.ToString());
            if (role != null)
            {
                var receiver = role.Installation.ShipsTo.FirstOrDefault();

                if (initiator == Initiator.Onshore && user.Roles.Any(s => s.Installation != null && s.Installation.Id == receiver.Id) == false)
                {
                    validationErrors.Add("You do not have access to save from this plant");
                }
            }
            else
            {
                validationErrors.Add("You do not have access to save from this plant");
            }
        }

        private void ValidateChemicalsListIsSetInCorrectFormat(Shipment shipment, List<string> validationErrors)
        {
            if (shipment.Chemicals == null) validationErrors.Add("chemical list must be set");
            else foreach (var chem in shipment.Chemicals)
                {
                    if (chem.MeasureUnit != "kg" && chem.MeasureUnit != "tonn" && chem.MeasureUnit != "l" && chem.MeasureUnit != "m3")
                    {
                        validationErrors.Add("chemical.measureUnit must be one of 'kg','tonn','l' or 'm3' in chemicals[]");
                    }
                    if (string.IsNullOrEmpty(chem.Chemical?.Name) && string.IsNullOrEmpty(chem.Chemical?.Description) && chem.Chemical?.Id == Guid.Empty)
                    {
                        validationErrors.Add("chemical.id must be set in chemicals[]");
                    }
                }
        }

        private void ValidatePlannedExecutionDatesIsSet(Shipment shipment, List<string> validationErrors)
        {
            if (shipment.PlannedExecutionFrom == DateTime.MinValue || shipment.PlannedExecutionTo == DateTime.MinValue)
            {
                validationErrors.Add("Planned execution dates must be set");
            }
        }

        public async Task<(Shipment, IEnumerable<string>)> SaveOrUpdate(Shipment shipment, Initiator initiator, Operation operation, DetailedOperation details, string comment, string attachment, List<IFormFile> attachments = null, Guid? deleteAttachmentId = null)
        {
            var validationErrors = new List<string>();

            var user = await userService.GetCurrentUser();
            if (user == null)
            {
                validationErrors.Add("You do not have access to save from this plant");
                return (shipment, validationErrors);
            }

            ValidateIsSenderIdSet(shipment, validationErrors);
            ValidateIshipmentDaysSet(shipment, validationErrors);
            if (validationErrors.Any()) return (null, validationErrors);

            ValidateAccessToSaveFromThisInstallation(initiator, user, shipment, validationErrors);
            ValidateAccessToSaveFromThisPlant(initiator, user, shipment, validationErrors);

            if (shipment.ContainsChemicals == false)
            {
                shipment.Chemicals = new List<ShipmentChemical>();
            }

            ValidateChemicalsListIsSetInCorrectFormat(shipment, validationErrors);
            ValidatePlannedExecutionDatesIsSet(shipment, validationErrors);

            if (validationErrors.Any()) return (null, validationErrors);

            if (shipment.PlannedExecutionFrom < shipment.PlannedExecutionTo)
            {
                var installation = await db.Installations.Where(w => w.Id == shipment.SenderId).FirstOrDefaultAsync();
                var localTimezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(installation.TimeZone);
                var localFrom = TimeZoneInfo.ConvertTimeFromUtc(shipment.PlannedExecutionFrom, localTimezoneInfo);
                var localTo = TimeZoneInfo.ConvertTimeFromUtc(shipment.PlannedExecutionTo, localTimezoneInfo);

                var to = new DateTime(localTo.Year, localTo.Month, localTo.Day, 23, 59, 59);
                var from = new DateTime(localFrom.Year, localFrom.Month, localFrom.Day);

                var days = to.Subtract(from).Days + 1;
                var parts = shipment.ShipmentParts.ToList();
                if (parts.Count() != days)
                {
                    validationErrors.Add("Days does not match the execution dates. This should normally not happen");
                }
                else
                {
                    for (int i = 0; i < days; i++)
                    {
                        parts[i].Shipped = shipment.PlannedExecutionFrom.AddDays(i);
                    }

                    shipment.ShipmentParts = parts;
                }
            }


            Db.Shipment dbObject = null;
            if (shipment.Id != Guid.Empty)
            {
                dbObject = await db.Shipments.Include(i => i.Chemicals).Include(i => i.Comments).Include(i => i.Attachments).Include(i => i.ShipmentParts)
                    .FirstOrDefaultAsync(ps => ps.Id == shipment.Id);
            }

            // Syntax/missing fields
            if (validationErrors.Any()) return (null, validationErrors);

            var statusCheck = CheckIfShipmentCanBeChanged(dbObject?.Status, operation, shipment);
            if (statusCheck != null)
            {
                validationErrors.Add(statusCheck);
            }

            var permissionCheck = CheckIfUserCanChangeShipment(dbObject?.Status, operation, shipment.SenderId, user);
            if (permissionCheck != null)
            {
                validationErrors.Add(permissionCheck);
            }

            // Permission checks
            if (validationErrors.Any()) return (null, validationErrors);

            IEnumerable<Db.Chemical> newChemicals = null;
            string status = null;
            if (dbObject != null)
            {
                if (shipment.Updated < dbObject.Updated)
                {
                    return (null, new List<string> { "This form has been updated while you have been working on it. Refresh the page to load the most recent version" });
                }
                mapper.Map(shipment, dbObject);
                (dbObject, newChemicals) = await HandleRelations(shipment, dbObject);

                if (details == DetailedOperation.DeleteAttachment)
                {
                    var attach = await db.Attachments.FirstOrDefaultAsync(a => a.Id == deleteAttachmentId);
                    if (attach != null) { db.Attachments.Remove(attach); }
                }

                if (details == DetailedOperation.NewAttachment)
                {
                    //Add attachments
                    if (attachments != null)
                    {
                        var blobContainerClient = GetBlobContainerClient(dbObject.Id);
                        await blobContainerClient.CreateIfNotExistsAsync();
                        foreach (var item in attachments)
                        {
                            using (var file = item.OpenReadStream())
                            {
                                var blob = blobContainerClient.GetBlobClient(item.FileName);
                                await blob.UploadAsync(file);
                                var newAttachment = new Db.Attachment
                                {
                                    Id = Guid.NewGuid(),
                                    ShipmentId = dbObject.Id,
                                    Path = item.FileName,
                                    MimeType = item.ContentType,
                                    Extension = item.FileName.Substring(item.FileName.LastIndexOf(".") >= 0 ? item.FileName.LastIndexOf(".") : 0)
                                };
                                db.Attachments.Add(newAttachment);
                            }
                        }
                    }
                }

                dbObject.Status = OperationStatus(operation, dbObject.Status, db);
                status = dbObject.Status;
            }
            else
            {

                var newDbObject = mapper.Map<Db.Shipment>(shipment);
                if (newDbObject.Id == Guid.Empty)
                    newDbObject.Id = Guid.NewGuid();
                (newDbObject, newChemicals) = await HandleRelations(shipment, newDbObject);

                //Add attachments
                if (attachments != null)
                {
                    var blobContainerClient = GetBlobContainerClient(newDbObject.Id);
                    await blobContainerClient.CreateIfNotExistsAsync();
                    newDbObject.Attachments = new List<Db.Attachment>();
                    foreach (var item in attachments)
                    {
                        using (var file = item.OpenReadStream())
                        {
                            var blob = blobContainerClient.GetBlobClient(item.FileName);
                            await blob.UploadAsync(file);
                            var newAttachment = new Db.Attachment
                            {
                                Id = Guid.NewGuid(),
                                ShipmentId = newDbObject.Id,
                                Path = item.FileName,
                                MimeType = item.ContentType,
                                Extension = item.FileName.Substring(item.FileName.LastIndexOf(".") >= 0 ? item.FileName.LastIndexOf(".") : 0)
                            };
                            newDbObject.Attachments.Add(newAttachment);
                        }
                    }
                }


                db.Shipments.Add(newDbObject);
                shipment.Id = newDbObject.Id;
                newDbObject.Status = OperationStatus(operation, null, db);
                status = newDbObject.Status;
                dbObject = newDbObject;
            }

            if (validationErrors.Any()) return (null, validationErrors);


            var sender = await db.Installations.Where(w => w.Id == shipment.SenderId).ProjectTo<PlantReference>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            var role = user.Roles.FirstOrDefault(u => u.Id == shipment.SenderId.ToString());
            var receiver = role.Installation.ShipsTo.FirstOrDefault();
            var plant = await db.Installations.Where(w => w.Id == receiver.Id).ProjectTo<PlantReference>(mapper.ConfigurationProvider).FirstOrDefaultAsync();

            dbObject.Updated = DateTime.Now;
            dbObject.ReceiverId = receiver.Id;

            //update legal terms settings:
            if (dbObject.AvailableForDailyContact == true)
            {
                dbObject.NormalProcedure = null;
                dbObject.OnlyWayToGetRidOf = null;
            }

            await db.SaveChangesAsync();

            loggerHelper.LogEvent(telemetry, user, sender, plant, operation, details, "ShipmentSaved", shipment);

            //TODO: Remove this code
            //await SendShipmentChangedMail(shipment, initiator, operation, details, comment, attachment, user, newChemicals, status, sender, plant);

            return (await db.Shipments.ProjectTo<Shipment>(mapper.ConfigurationProvider).FirstOrDefaultAsync(ps => ps.Id == shipment.Id), null);
        }
        private async Task SendShipmentChangedMail(Shipment shipment, Initiator initiator, Operation operation, DetailedOperation details, string comment, string attachment, User user, IEnumerable<Db.Chemical> newChemicals, string status, PlantReference sender, PlantReference plant)
        {
            if (newChemicals != null && newChemicals.Any() && string.IsNullOrEmpty(config["chemicalEmail"]) == false)
            {
                var to = config["chemicalEmail"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (to.Any())
                {
                    (var subject, var html, var plainText) = buildEmailContentForChemicalResponsible(initiator, sender, plant, user, newChemicals);
                    await mailSender.SendMail(to, subject, html, plainText);
                    loggerHelper.LogEvent(telemetry, user, sender, plant, operation, details, "EmailNotificationNewChemical", new { To = to, Subject = subject });
                }

            }

            if (status != Statuses.Draft)
            {
                (var subject, var html, var plainText) = buildEmailContent(initiator, sender, plant, user, operation, details, comment, shipment.EvalComments, attachment, status, shipment.Id);

                if (initiator == Initiator.Onshore)
                {
                    var recipients = sender?.ContactList;
                    if (recipients != null && recipients.Any())
                    {
                        await mailSender.SendMail(recipients, subject, html, plainText);
                        loggerHelper.LogEvent(telemetry, user, plant, sender, operation, details, "EmailNotificationFromOnshore", new { To = recipients, Subject = subject });

                    }
                }
                else if (initiator == Initiator.Offshore)
                {
                    var recipients = plant?.ContactList;
                    if (recipients != null && recipients.Any())
                    {
                        await mailSender.SendMail(recipients, subject, html, plainText);
                        loggerHelper.LogEvent(telemetry, user, sender, plant, operation, details, "EmailNotificationFromOffshore", new { To = recipients, Subject = subject });
                    }
                }
            }
        }

        private (string, string, string) buildEmailContentForChemicalResponsible(Initiator initiator, PlantReference installation, PlantReference destination, User user, IEnumerable<Db.Chemical> chemicals)
        {

            var emailTemplate = new StringBuilder().Append("{{subject}}")
                                                    .Append("<br/>")
                                                    .Append("{{title}}")
                                                    .Append("<br/>")
                                                    .Append("{{change}}")
                                                    .Append("<br/>")
                                                    .Append("{{changedBy}}")
                                                    .Append("<br/>")
                                                    .Append("{{link}}")
                                                    .Append("<br/>")
                                                    .Append("{{linkTitle}}")
                                                    .Append("<br/>")
                                                    .Append("{{portalLink}}");


            var subject = string.Empty;

            var title = string.Empty;

            var change = string.Empty;
            var changeText = string.Empty;

            var changedBy = "Added by " + user.Name + " on " + installation.Name + " (<a href=\"mailto:" + user.Email + "\">" + user.Email + "</>)";

            var link = string.Empty;
            var linkTitle = string.Empty;
            var portalLink = string.Empty;

            switch (config["env"])
            {
                case "Local": portalLink = "https://localhost:44356"; break;
                case "Dev": portalLink = "https://chemdec-dev.azurewebsites.net"; break;
                case "Test": portalLink = "https://chemdec-test.azurewebsites.net"; break;
                case "QA": portalLink = "https://chemcom-qa.azurewebsites.net"; break;
                case "Prod": portalLink = "https://chemcom.equinor.com"; break;
            }

            subject = "Chemical(s) was added as added to the chemical register";

            change = "Chemical(s) was added to the chemical register while registering a delivery from from " +
                installation.Name + " to " + destination.Name + ":<br/><br/><i>" + string.Join("<br/>", chemicals.Select(s => s.Name)) + "</i>";



            switch (config["env"])
            {
                case "Local": subject = "[Local] " + subject; break;
                case "Dev": subject = "[Dev] " + subject; break;
                case "Test": subject = "[Test] " + subject; break;
                case "QA": subject = "[QA] " + subject; break;
            }

            var linkSubject = "<a href=\"" + portalLink + "\">" + subject + "</a>";

            if (string.IsNullOrEmpty(changeText)) changeText = change;


            var html = emailTemplate
                .Replace("{{subject}}", subject)
                .Replace("{{title}}", linkSubject)
                .Replace("{{change}}", change)
                .Replace("{{changedBy}}", changedBy)
                .Replace("{{link}}", link)
                .Replace("{{linkTitle}}", linkTitle)
                .Replace("{{portalLink}}", portalLink);

            var plainText = $"{subject}\n\n{change}\n{changedBy}\n{link}\n\nRegards\nChemCom";

            return (subject, html.ToString(), plainText);



        }
        private (string, string, string) buildEmailContent(Initiator initiator, PlantReference installation, PlantReference destination, User user, Operation operation, DetailedOperation details, string comment, string approversComments, string attachment, string status, Guid shipmentId)
        {
            var emailTemplate = new StringBuilder().Append("{{subject}}")
                                                    .Append("<br/>")
                                                    .Append("{{title}}")
                                                    .Append("<br/>")
                                                    .Append("{{change}}")
                                                    .Append("<br/>")
                                                    .Append("{{changedBy}}")
                                                    .Append("<br/>")
                                                    .Append("{{link}}")
                                                    .Append("<br/>")
                                                    .Append("{{linkTitle}}")
                                                    .Append("<br/>")
                                                    .Append("{{portalLink}}");

            var subject = string.Empty;
            var change = string.Empty;
            var changedBy = status + " by " + user.Name + " on " + installation.Name + " (<a href=\"mailto:" + user.Email + "\">" + user.Email + "</>)";

            var link = string.Empty;
            var linkTitle = string.Empty;
            var portalLink = getEnvironmentSpecificPortalLink();

            if (initiator == Initiator.Offshore)
            {
                if (operation == Operation.Submit)
                {
                    subject = "Chemical form was submitted to " + destination.Name;
                }

            }
            else if (initiator == Initiator.Onshore)
            {
                if (operation == Operation.Approve)
                {
                    subject = "Chemical form was approved";
                }
                if (operation == Operation.Change)
                {
                    subject = "Chemical form was changed";
                }
                if (operation == Operation.Decline)
                {
                    subject = "Chemical form was declined";
                }
                if (operation == Operation.SaveEvaluation)
                {
                    subject = "Chemical form evaluation was changed";
                }
            }

            if (operation == Operation.Change)
            {
                subject = "Chemical form was changed";

                if (details == DetailedOperation.Saved)
                {
                    change = string.Empty;
                }
                if (details == DetailedOperation.AddComment)
                {
                    change = "Comment was added: <i>" + comment + "</i>";
                }
                if (details == DetailedOperation.NewAttachment)
                {
                    change = "Attachment was added: <i>" + attachment + "</i>";
                }
                if (details == DetailedOperation.DeleteAttachment)
                {
                    change = "Attachment was removed: <i>" + attachment + "</i>";
                }
            }

            subject = getEnvironmentSpecificSubject(subject);

            var linkSubject = "<a href=\"" + portalLink + "/" + (initiator == Initiator.Offshore ? destination.Code : installation.Code) + "/viewform/" + shipmentId + "\">" + subject + "</a>";

            var approversCommentsSection = !string.IsNullOrWhiteSpace(approversComments) ? "<h3>Approver comments<br /></h3>" + approversComments : "";

            var html = emailTemplate
                .Replace("{{subject}}", subject)
                .Replace("{{title}}", linkSubject)
                .Replace("{{change}}", change)
                .Replace("{{changedBy}}", changedBy)
                .Replace("{{link}}", link)
                .Replace("{{linkTitle}}", linkTitle)
                .Replace("{{portalLink}}", portalLink)
                .Replace("{{approversComments}}", approversCommentsSection);

            var plainText = $"{subject}\n\n{change}\n{changedBy}\n{link}\nApprovers comments\n{approversComments}\n\nRegards\nChemCom";

            return (subject, html.ToString(), plainText);
        }

        private string getEnvironmentSpecificSubject(string subject)
        {
            switch (config["env"])
            {
                case "Local": subject = "[Local] " + subject; break;
                case "Dev": subject = "[Dev] " + subject; break;
                case "Test": subject = "[Test] " + subject; break;
                case "QA": subject = "[QA] " + subject; break;
            }

            return subject;
        }

        private string getEnvironmentSpecificPortalLink()
        {
            switch (config["env"])
            {
                case "Local": return "https://localhost:44356";
                case "Dev": return "https://chemdec-dev.azurewebsites.net";
                case "Test": return "https://chemdec-test.azurewebsites.net";
                case "Prod": return "https://chemcom.equinor.com";
                default: return string.Empty;
            }
        }

        private IEnumerable<LogEntry> FindGeneralDifferences(Shipment dto, Db.Shipment dbObject)
        {
            // Fill in when logging is approved
            return null;
        }
        public static void CalculateChemicals(double rinsedBeforeShipment, Db.ShipmentChemical toBeUpdated, Db.Chemical chemical)
        {
            if (toBeUpdated.MeasureUnit == "kg")
            {
                //=HVIS(C45="kg";D45*N45/100;HVIS(C45="tonn";D45*1000*N45/100;HVIS(C45="L";D45*O45*N45/100;HVIS(C45="m3";D45*1000*O45*N45/100;""))))
                toBeUpdated.CalculatedWeight = toBeUpdated.Amount;
            }
            if (toBeUpdated.MeasureUnit == "tonn")
            {
                toBeUpdated.CalculatedWeight = toBeUpdated.Amount * 1000;
            }

            if (toBeUpdated.MeasureUnit == "l")
            {
                toBeUpdated.CalculatedWeight = toBeUpdated.Amount * chemical.Density;
            }

            if (toBeUpdated.MeasureUnit == "m3")
            {
                toBeUpdated.CalculatedWeight = toBeUpdated.Amount * chemical.Density * 1000;
            }

            toBeUpdated.CalculatedWeightUnrinsed = toBeUpdated.CalculatedWeight;

            toBeUpdated.CalculatedNitrogenUnrinsed = (toBeUpdated.CalculatedWeight * chemical.NitrogenWeight) / 100;
            toBeUpdated.CalculatedTocUnrinsed = (toBeUpdated.CalculatedWeight * chemical.TocWeight) / 100;
            toBeUpdated.CalculatedBiocidesUnrinsed = (toBeUpdated.CalculatedWeight * chemical.BiocideWeight) / 100;

            // Remove rinsed
            toBeUpdated.CalculatedWeight = toBeUpdated.CalculatedWeight * ((100 - rinsedBeforeShipment) / 100);

            toBeUpdated.CalculatedNitrogen = (toBeUpdated.CalculatedWeight * chemical.NitrogenWeight) / 100;
            toBeUpdated.CalculatedToc = (toBeUpdated.CalculatedWeight * chemical.TocWeight) / 100;
            toBeUpdated.CalculatedBiocides = (toBeUpdated.CalculatedWeight * chemical.BiocideWeight) / 100;

        }
        private async Task<(Db.Shipment, IEnumerable<Db.Chemical>)> HandleRelations(Shipment dto, Db.Shipment dbObject)
        {
            var newChemicals = new List<Db.Chemical>();
            // Comments
            if (dto.Comments == null) dto.Comments = new List<Comment>();
            if (dbObject.Comments == null) dbObject.Comments = new List<Db.Comment>();
            var commentsToBeRemoved = dbObject.Comments.Where(w => dto.Comments.Select(s => s.Id).Any(a => a == w.Id) == false).ToList();
            foreach (var item in commentsToBeRemoved)
            {
                dbObject.Comments.Remove(item);
                db.Comments.Remove(item);
            }

            var commentsToBeAdded = dto.Comments.Where(w => dbObject.Comments.Select(s => s.Id).Any(a => a == w.Id) == false).ToList();
            foreach (var item in commentsToBeAdded)
            {
                var newComment = mapper.Map<Db.Comment>(item);
                dbObject.Comments.Add(newComment);
            }

            //Shipment parts
            if (dbObject.ShipmentParts == null) dbObject.ShipmentParts = new List<Db.ShipmentPart>();
            var removeParts = dbObject.ShipmentParts.ToList();

            foreach (var part in removeParts)
            {
                dbObject.ShipmentParts.Remove(part);
                db.ShipmentParts.Remove(part);
            }

            foreach (var part in dto.ShipmentParts)
            {
                var newPart = mapper.Map<Db.ShipmentPart>(part);
                dbObject.ShipmentParts.Add(newPart);
            }

            foreach (var item in dto.Chemicals)
            {
                if (dbObject.Chemicals == null)
                {
                    dbObject.Chemicals = new List<Db.ShipmentChemical>();
                }

                var shipmentChemical = await db.ShipmentChemicals.FirstOrDefaultAsync(c => c.ChemicalId == item.Id && c.ShipmentId == dto.Id);

                if(shipmentChemical == null)
                {
                    var newShipmentChemical = new Db.ShipmentChemical
                    {
                        Id = Guid.NewGuid(),
                        ChemicalId = item.Id,
                        MeasureUnit = item.MeasureUnit,
                        Amount = item.Amount,
                        ShipmentId = dbObject.Id
                    };
                    db.ShipmentChemicals.Add(newShipmentChemical);
                }
                else
                {
                    shipmentChemical.MeasureUnit = item.MeasureUnit;
                    shipmentChemical.Amount = item.Amount;
                    db.ShipmentChemicals.Update(shipmentChemical);
                }
            }
            

            dbObject.SenderId = dto.SenderId;

            return (dbObject, newChemicals);
        }

        private BlobContainerClient GetBlobContainerClient(Guid shipmentId)
        {
            var storageSharedKeyCredentials = new StorageSharedKeyCredential(config["AzureBlobAccount"], config["AzureBlobKey"]);
            Uri uri = new Uri($"https://{config["AzureBlobAccount"]}.blob.core.windows.net");
            var blobServiceClient = new BlobServiceClient(uri, storageSharedKeyCredentials);
            return blobServiceClient.GetBlobContainerClient(shipmentId.ToString());
        }


        public async Task<(AttachmentResponse, IEnumerable<string>)> ReturnAttachment(Guid shipmentId, Guid attachmentId)
        {
            var attachmentAndShipment = await db.Attachments.Where(w => w.Id == attachmentId && w.ShipmentId == shipmentId).Select(s => new { s.Path, s.MimeType, s.Shipment.ReceiverId, s.Shipment.SenderId }).FirstOrDefaultAsync();
            if (attachmentAndShipment == null)
            {
                return (null, null);
            }

            var user = await userService.GetCurrentUser();
            if (user.Roles.Any(a => a.Installation?.Id == attachmentAndShipment.SenderId || a.Installation?.Id == attachmentAndShipment.ReceiverId) == false)
            {
                return (null, new List<string> { "User does not av access to attachment " + attachmentAndShipment.Path + " on shipment " + shipmentId });
            }

            var blobContainerClient = GetBlobContainerClient(shipmentId);
            var newBlob = blobContainerClient.GetBlobClient(attachmentAndShipment.Path);
            if (await newBlob.ExistsAsync())
            {
                return (new AttachmentResponse { Attachment = await newBlob.OpenReadAsync(), MimeType = attachmentAndShipment.MimeType, Path = attachmentAndShipment.Path }, null);
            }
            else return (null, null);
        }

        public async Task<(Shipment, IEnumerable<string>)> AddAttachment(Shipment shipment, Initiator initiator, string fileName, string mimeType, IFormFile attachement)
        {
            if (shipment.Id == Guid.Empty || shipment.Status == null)
            {
                shipment.Id = Guid.NewGuid();
            }

            return await SaveOrUpdate(shipment, initiator, Operation.Change, DetailedOperation.NewAttachment, null, null, new List<IFormFile> { attachement });
        }

        public async Task<(Shipment, IEnumerable<string>)> RemoveAttachement(Shipment shipment, Initiator initiator, Guid attachmentId)
        {
            if (shipment.Id == Guid.Empty || shipment.Status == null)
            {
                shipment.Id = Guid.NewGuid();
            }

            var id = shipment.Id;
            if (shipment.Attachments == null) shipment.Attachments = new List<Attachment>();

            var attachment = shipment.Attachments.FirstOrDefault(w => w.Id == attachmentId);
            if (attachment == null) return (null, new List<string> { "No attachment with id " + attachmentId + " was found" });
            var blobContainerClient = GetBlobContainerClient(shipment.Id);
            var newBlob = blobContainerClient.GetBlobClient(attachment.Path);
            await newBlob.DeleteIfExistsAsync();
            return await SaveOrUpdate(shipment, initiator, Operation.Change, DetailedOperation.DeleteAttachment, null, attachment.Path, null, attachmentId);
        }


        public async Task<(Shipment, IEnumerable<string>)> AddComment(Shipment shipment, Initiator initiator, string comment)
        {

            if (shipment.Id == Guid.Empty || shipment.Status == null)
            {
                shipment.Id = Guid.NewGuid();
            }

            var id = shipment.Id;


            if (shipment.Comments == null) shipment.Comments = new List<Comment>();
            var comments = shipment.Comments.ToList();
            comments.Add(new Comment { Id = Guid.NewGuid(), CommentText = comment });
            shipment.Comments = comments;

            return await SaveOrUpdate(shipment, initiator, Operation.Change, DetailedOperation.AddComment, comment, null);

        }

        public async Task<(Shipment, IEnumerable<string>)> RemoveComment(Shipment shipment, Guid commentId, Initiator initiator)
        {

            if (shipment.Id == Guid.Empty || shipment.Status == null)
            {
                shipment.Id = Guid.NewGuid();
            }

            var id = shipment.Id;

            if (shipment.Comments == null) shipment.Comments = new List<Comment>();
            var comment = shipment.Comments.FirstOrDefault(w => w.Id == commentId)?.CommentText;
            shipment.Comments = shipment.Comments.Where(w => w.Id != commentId).ToList();

            if (comment != null)
                return await SaveOrUpdate(shipment, initiator, Operation.Change, DetailedOperation.RemoveComment, comment, null);
            else return (shipment, null);
        }


    }
}
