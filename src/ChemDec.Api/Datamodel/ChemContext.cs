using ChemDec.Api.Datamodel.Interfaces;
using ChemDec.Api.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChemDec.Api.Datamodel
{
    public class ChemContext :  DbContext
    {
        public ChemContext(DbContextOptions<ChemContext> options, UserResolver resolver) : base(options)
        {
            this.resolver = resolver;
        }
        public static string ValidationErrors { get; set; }
        //public DbSet<Role> Roles { get; set; } Might not need this...
        public DbSet<Installation> Installations { get; set; }
        public DbSet<Chemical> Chemicals { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentPart> ShipmentParts { get; set; }
        public DbSet<ShipmentChemical> ShipmentChemicals { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<FieldChange> FieldChanges { get; set; }
        public DbSet<InstallationPlant> InstallationPlants { get; set; }

        public DbSet<UserTokenCache> UserTokenCache { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            

            builder.Entity<ShipmentChemical>()
                .HasOne(bc => bc.Chemical)
                .WithMany(b => b.Shipments)
                .HasForeignKey(bc => bc.ChemicalId);

          
            builder.Entity<ShipmentChemical>()
             .HasOne(bc => bc.Shipment)
             .WithMany(b => b.Chemicals)
             .HasForeignKey(bc => bc.ShipmentId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Shipment>()
               .HasOne(bc => bc.Sender)
               .WithMany(b => b.SendtShipments)
               .HasForeignKey(f => f.SenderId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Shipment>()
               .HasOne(bc => bc.Receiver)
               .WithMany(b => b.ReceivedShipments)
               .HasForeignKey(f=>f.ReceiverId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Shipment>()
                  .HasMany(bc => bc.Attachments)
                  .WithOne(b => b.Shipment)
                  .HasForeignKey(f => f.ShipmentId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Attachment>()
                .HasOne<Shipment>()
                .WithMany(b => b.Attachments)
                .HasForeignKey(f => f.ShipmentId);

            builder.Entity<Installation>()
                 .HasMany(bc => bc.GetsShipmentsFrom)
                 .WithOne(b => b.ShipsTo)
                 .HasForeignKey(f => f.ShipsToId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InstallationPlant>().HasKey(r => new { r.PlantId, r.InstallationId });
            builder.Entity<InstallationPlant>()
            .HasOne(pt => pt.Installation)
            .WithMany(p => p.ShipsToPlant)
            .HasForeignKey(pt => pt.InstallationId).OnDelete(DeleteBehavior.Restrict); ;

            builder.Entity<InstallationPlant>()
              .HasOne(pt => pt.Plant)
              .WithMany(p => p.GetsShipmentsFromInstallation)
              .HasForeignKey(pt => pt.PlantId).OnDelete(DeleteBehavior.Restrict); 

      
        

    }


        private static bool initalized = false;
        private readonly UserResolver resolver;

        public void CheckMigrations(bool force = true)
        {
            if (initalized == false || force)
            {
                //Database.Migrate(); //Auto migrate on - turn this off when db is more or less ready
                
                /*var pendingMigrations = Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    ValidationErrors = "Fatal error: The following migrations are missing:\n" + string.Join("\n", pendingMigrations);
                }
                else { ValidationErrors = null; }*/
                
                initalized = true;
            }
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.DetectChanges();
            AddTimestamps();
            ChangeTracker.DetectChanges();
            return base.SaveChangesAsync();
        }



        private void AddTimestamps()
        {
            var auditable = ChangeTracker.Entries().Where(x => x.Entity is IAudit && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var currentUsername = resolver.GetCurrentUserId();
            var currentUserDisplayName = resolver.GetCurrentUserDisplayName();

            foreach (var audit in auditable)
            {
                if (((IAudit)audit.Entity).Id == null) ((IAudit)audit.Entity).Id = Guid.NewGuid();
                ((IAudit)audit.Entity).Updated = DateTime.UtcNow;
                ((IAudit)audit.Entity).UpdatedBy = currentUsername;
                ((IAudit)audit.Entity).UpdatedByName = currentUserDisplayName;

            }

        }
    }
}
