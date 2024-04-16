using Domain.Attachments;
using Domain.Chemicals;
using Domain.Comments;
using Domain.Installations;
using Domain.ShipmentChemicals;
using Domain.ShipmentParts;
using Domain.Shipments;
using Domain.LogEntries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.InstallationPlants;
using Domain.UserTokenCaches;
using Domain.Notifications;
using Domain.FieldChanges;
using Application.Common.Repositories;

namespace Infrastructure.Persistance;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Installation> Installations { get; set; }
    public DbSet<Chemical> Chemicals { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentPart> ShipmentParts { get; set; }
    public DbSet<ShipmentChemical> ShipmentChemicals { get; set; }
    //public DbSet<LogEntry> LogEntries { get; set; }
    //public DbSet<FieldChange> FieldChanges { get; set; }
    public DbSet<InstallationPlant> InstallationPlants { get; set; }
    //public DbSet<UserTokenCache> UserTokenCache { get; set; }
    //public DbSet<Notification> Notifications { get; set; }

    public async Task CommitChangesAsync()
    {        
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

