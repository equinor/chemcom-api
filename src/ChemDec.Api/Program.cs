using ChemDec.Api.Datamodel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ChemDec.Api.Infrastructure.Security;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Controllers.Handlers;
using ChemDec.Api.Infrastructure.Middleware;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.Net.Http;
using ChemDec.Api;
using ChemDec.Api.Infrastructure.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Infrastructure.Persistance;
using Application.Common;
using Application.Common.Repositories;
using Infrastructure.Persistance.Repositories;
using Application.Comments.Services;
using Infrastructure.BlobService;
using Application.Common.Services;
using Infrastructure.EmailService;
using Quartz;
using ChemDec.Api.BackgroundJobs;
using ChemDec.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var keyVaultUrl = configuration["KeyVaultEndpoint"];
var clientId = configuration["azure:ClientId"];
var clientSecret = configuration["azure:ClientSecret"];
var tenantId = configuration["azure:TenantId"];

if (!builder.Environment.IsDevelopment())
{
    var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    var chainedTokenCredential = new ChainedTokenCredential(new WorkloadIdentityCredential(), clientSecretCredential);

    var secretClient = new SecretClient(new Uri(keyVaultUrl), chainedTokenCredential);
    builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
}

var corsDomainsFromConfig = Utils.GetCommaSeparatedConfigValue(configuration, "AllowCorsDomains");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ChemContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("chemcomdb"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(20),
                errorNumbersToAdd: null);
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("chemcomdb"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(20),
                errorNumbersToAdd: null);
        });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(optionsA => { }, optionsB =>
    {
        configuration.Bind("azure", optionsB);
        var defaultBackChannel = new HttpClient();
        defaultBackChannel.DefaultRequestHeaders.Add("Origin", "chemcom");
        optionsB.Backchannel = defaultBackChannel;
    }).EnableTokenAcquisitionToCallDownstreamApi(e => { })
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ChemX", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ChemAuthenticationRequirement());
    });

    options.AddPolicy("ChemY", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ChemAuthenticationRequirement { MustBeTreatmentPlant = true });
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
    builder =>
    {
        var origins = new string[]
        {
            "https://frontend-chemcom-dev.radix.equinor.com",
            "https://frontend-chemcom-prod.radix.equinor.com",
            "http://localhost:5174",
            "https://chemcom.equinor.com"
        };
        builder.WithOrigins(origins)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, ChemAuthenticationHandler>();
builder.Services.AddScoped<UserResolver>();
builder.Services.AddScoped<ChemicalHandler>();
builder.Services.AddScoped<ShipmentHandler>();
builder.Services.AddScoped<InstallationHandler>();
builder.Services.AddScoped<AdminHandler>();
builder.Services.AddScoped<UserService>();
builder.Services.AddTransient<ICommentsService, CommentsService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<MailSender>();
builder.Services.AddTransient<LoggerHelper>();
builder.Services.AddMemoryCache();


builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();
builder.Services.AddScoped<IShipmentsRepository, ShipmentsRepository>();
builder.Services.AddScoped<IInstallationsRepository, InstallationsRepository>();
builder.Services.AddScoped<IShipmentPartsRepository, ShipmentPartsRepository>();
builder.Services.AddScoped<IChemicalsRepository, ChemicalsRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IAttachmentsRepository, AttachmentsRepository>();
builder.Services.AddScoped<IUnitOfWork>(serivceProvider => serivceProvider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<IFileUploadService, AzureFileUploadService>();
builder.Services.AddScoped<IEmailNotificationsRepository, EmailNotificationsRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEnvironmentContext, EnvironmentContext>();
builder.Services.AddScoped<IUserProvider, UserProvider>();

//builder.Services.AddQuartz(options =>
//{
//    JobKey jobKey = JobKey.Create(nameof(EmailNotificationJob));
//    options.AddJob<EmailNotificationJob>(jobKey)
//           .AddTrigger(trigger =>
//           {
//               trigger.ForJob(jobKey);
//               if (builder.Environment.IsDevelopment())
//               {
//                   int intervalInSeconds = 1000;
//                   trigger.WithSimpleSchedule(schedule =>
//                   {
//                       schedule.WithIntervalInSeconds(intervalInSeconds)
//                               .RepeatForever();
//                   });
//               }
//               else
//               {
//                   int intervalInMinutes = 5;
//                   trigger.ForJob(jobKey)
//                      .WithSimpleSchedule(schedule =>
//                      {
//                          schedule.WithIntervalInMinutes(intervalInMinutes)
//                                  .RepeatForever();
//                      });
//               }
//           });
//});
//builder.Services.AddQuartzHostedService(options =>
//{
//    options.WaitForJobsToComplete = true;
//});
CommandAndQueryHandlersSetup.AddCommandOrQueryHandlers(builder.Services, typeof(ICommandHandler<>));
CommandAndQueryHandlersSetup.AddCommandOrQueryHandlers(builder.Services, typeof(ICommandHandler<,>));
CommandAndQueryHandlersSetup.AddCommandOrQueryHandlers(builder.Services, typeof(IQueryHandler<,>));

// The following line enables Application Insights telemetry collection.
var appinsightConnStr = configuration["ApplicationInsights:ConnectionString"];
var optionsAppInsight = new ApplicationInsightsServiceOptions { ConnectionString = configuration["ApplicationInsights:ConnectionString"] };
builder.Services.AddApplicationInsightsTelemetry(options: optionsAppInsight);

SwaggerSetup.ConfigureServices(builder.Configuration, builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ChemContext>();
    //dbContext.CheckMigrations();
}
app.UseCors(MyAllowSpecificOrigins);
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
SwaggerSetup.Configure(configuration, app);
app.UseMiddleware(typeof(ErrorHandlingMiddleware));

app.MapControllers();

app.Run();
