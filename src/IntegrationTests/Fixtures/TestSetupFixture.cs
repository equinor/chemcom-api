﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Application.Common;
using Application.Common.Repositories;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Persistance.Interceptors;
using Domain.Installations;
using Application.Comments.Services;
using IntegrationTests.Fakes;
using IntegrationTests.Common;
using Testcontainers.MsSql;
using System.Security.Claims;
using Domain.Users;
using System.Runtime.InteropServices;

namespace IntegrationTests.Fixtures;

public class TestSetupFixture : IAsyncLifetime, IDisposable
{
    public IHost Host { get; set; }
    public ICommandDispatcher CommandDispatcher { get; private set; }
    public IQueryDispatcher QueryDispatcher { get; private set; }
    public IConfigurationRoot Configuration { get; private set; }
    public ClaimsPrincipal ClaimsPrincipal { get; private set; }
    public IUserProvider UserProvider { get; private set; }

    private readonly ApplicationDbContext _dbContext;
    private readonly MsSqlContainer _msSqlContainer;
        //= new MsSqlBuilder().Build();

    //TODO: Add Faker
    //TODO: Refactor to use IClassFixture<>
    public TestSetupFixture()
    {
        // Workaround to fix issue with Testcontainers.MsSql on Linux
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _msSqlContainer = new MsSqlBuilder()
                .WithImage(
                    "mcr.microsoft.com/mssql/server:2022-latest"
                )
                .WithPortBinding(1433, true)
                .Build();
        }
        else
        {
            _msSqlContainer = new MsSqlBuilder()
                .WithPortBinding(1433, true)
                .Build();
        }
        _msSqlContainer.StartAsync().Wait();
        ConfigurationBuilder configurationBuilder = new();
        Configuration = configurationBuilder
                            .AddUserSecrets<TestSetupFixture>()
                            .AddEnvironmentVariables()
                            .Build();

        Host = new HostBuilder()
              .UseEnvironment("Development")
              .ConfigureHostConfiguration(configHost => configHost.AddConfiguration(Configuration))
              .ConfigureServices(services =>
              {
                  services.AddSingleton<AuditableEntitiesInterceptor>();
                  services.
                       AddDbContext<ApplicationDbContext>(options =>
                       {
                           options.UseSqlServer(_msSqlContainer.GetConnectionString())
                                  .EnableSensitiveDataLogging();
                       });
                  services.AddScoped<ICommandDispatcher, CommandDispatcher>();
                  services.AddScoped<IQueryDispatcher, QueryDispatcher>();
                  services.AddScoped<IShipmentsRepository, ShipmentsRepository>();
                  services.AddScoped<IInstallationsRepository, InstallationsRepository>();
                  services.AddScoped<IShipmentPartsRepository, ShipmentPartsRepository>();
                  services.AddScoped<IChemicalsRepository, ChemicalsRepository>();
                  services.AddScoped<ICommentsRepository, CommentsRepository>();
                  services.AddScoped<IAttachmentsRepository, AttachmentsRepository>();
                  services.AddScoped<IEmailNotificationsRepository, EmailNotificationsRepository>();
                  services.AddScoped<IUnitOfWork>(serivceProvider => serivceProvider.GetRequiredService<ApplicationDbContext>());
                  services.AddScoped<IFileUploadService, FakeFileUploadService>();
                  services.AddScoped<IEnvironmentContext, FakeEnvironmentContext>();
                  services.AddScoped<IUserProvider, FakeUserProvider>();
                  AddCommandOrQueryHandlers(services, typeof(ICommandHandler<>));
                  AddCommandOrQueryHandlers(services, typeof(ICommandHandler<,>));
                  AddCommandOrQueryHandlers(services, typeof(IQueryHandler<,>));
              })
              .Build();


        CommandDispatcher = Host.Services.GetService(typeof(ICommandDispatcher)) as ICommandDispatcher;
        QueryDispatcher = Host.Services.GetService(typeof(IQueryDispatcher)) as IQueryDispatcher;
        _dbContext = Host.Services.GetService<ApplicationDbContext>();
        UserProvider = Host.Services.GetService<IUserProvider>();
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _dbContext.Database.EnsureCreated();

        Installation sender = new Installation()
        {
            Id = Constants.SenderId,
            Name = "Oseberg C",
            Code = "OsebergC",
            Description = "Oseberg C",
            InstallationType = "platform",
            Updated = DateTime.Now,
            UpdatedBy = "VEHE@equinor.com",
            ShipsToId = new Guid("C4D2D827-48E6-45A8-9FB4-DBD8E7A54A67"),
            Contact = "gm_chemcom@equinor.com",
            TimeZone = "W. Europe Standard Time"
        };

        Installation receiver = new Installation()
        {
            Id = Constants.ReceiverId,
            Name = "Sture",
            Code = "Sture",
            Description = "Sture Terminal",
            InstallationType = "plant",
            Updated = DateTime.Now,
            UpdatedBy = "VEHE@equinor.com",
            UpdatedByName = "Vegard Helland (Capgemini Norge AS,Oslo)",
            Contact = "gm_chemcom@equinor.com",
            TimeZone = "W. Europe Standard Time",
            NitrogenCapacity = 300,
            TocCapacity = 300,
            WaterCapacity = 200000
        };
        _dbContext.Installations.AddRange(sender, receiver);
        _dbContext.SaveChanges();
    }

    private static void AddCommandOrQueryHandlers(IServiceCollection services, Type interfaceType)
    {
        var types = interfaceType.Assembly.GetTypes().Where(t =>
            t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType));
        foreach (var type in types)
        {
            type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                .ToList().ForEach(i => services.AddScoped(i, type));
        }
    }

    //public void MockClaimsPrincipal()
    //{
    //    var claims = new List<Claim>()
    //        {
    //            new Claim(ClaimTypes.Name, "username"),
    //            new Claim(ClaimTypes.NameIdentifier, "userId"),
    //            new Claim("name", "John Doe"),
    //        };
    //    var identity = new ClaimsIdentity(claims, "TestAuthType");
    //    var claimsPrincipal = new ClaimsPrincipal(identity);
    //}

    public void Dispose()
    {

    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }
}
