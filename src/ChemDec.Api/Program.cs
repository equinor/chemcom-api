using ChemDec.Api.Datamodel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using ChemDec.Api.Infrastructure.Security;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Controllers.Handlers;
using Microsoft.Graph;
using ChemDec.Api.Infrastructure.Middleware;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.Net.Http;
using ChemDec.Api;
using ChemDec.Api.GraphApi;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var corsDomainsFromConfig = Utils.GetCommaSeparatedConfigValue(configuration, "AllowCorsDomains");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ChemContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("chemcomdb"));
});

//builder.Services.AddMicrosoftIdentityWebApiAuthentication(configuration, "azure");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(optionsA => { }, optionsB =>
    {
        configuration.Bind("azure", optionsB);
        var defaultBackChannel = new HttpClient();
        defaultBackChannel.DefaultRequestHeaders.Add("Origin", "chemcom");
        optionsB.Backchannel = defaultBackChannel;
    }).EnableTokenAcquisitionToCallDownstreamApi(e => { })
    .AddInMemoryTokenCaches();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(configuration, "azure");


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
        //var domainsAsArray = new string[corsDomainsFromConfig.Count];
        //corsDomainsFromConfig.CopyTo(domainsAsArray);

        //builder.WithOrigins(domainsAsArray);
        builder.AllowAnyOrigin()
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, ChemAuthenticationHandler>();
builder.Services.AddScoped<UserResolver>();
builder.Services.AddScoped<ChemicalHandler>();
builder.Services.AddScoped<ShipmentHandler>();
builder.Services.AddScoped<InstallationHandler>();
builder.Services.AddScoped<AdminHandler>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAuthenticationProvider, MicrosoftGraphAuthenticationProvider>();
builder.Services.AddScoped<EquinorMsGraphHandler>();
builder.Services.AddScoped<MicrosoftGraphAuthenticationProvider>();
builder.Services.AddTransient<AuthorizationHandler>();
builder.Services.AddTransient<ConfigurationHelper>();
builder.Services.AddTransient<MailSender>();
builder.Services.AddTransient<LoggerHelper>();
builder.Services.AddScoped<IGraphServiceProvider, GraphServiceProvider>();
builder.Services.AddMemoryCache();

SwaggerSetup.ConfigureServices(builder.Configuration, builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ChemContext>();
    dbContext.CheckMigrations();
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
