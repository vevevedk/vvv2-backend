using Microsoft.EntityFrameworkCore;
using MediatR;
using Veveve.Api.Domain.Commands.Users;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Domain.Services;
using SendGrid;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Infrastructure;
using System.Text.Json.Serialization;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Infrastructure.Middleware;
using Veveve.Api;
using Veveve.Api.Infrastructure.Swagger;
using Serilog;
using Serilog.Events;
using Veveve.Api.Domain.PipelineBehaviours;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.Gax.Lib;
using Google.Ads.GoogleAds.Config;
using Google.Ads.Gax.Config;

var builder = WebApplication.CreateBuilder(args);
var appsettings = builder.Configuration.Get<Appsettings>();

// init serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Default", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new DateTimeUtcConverter());
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Appsettings>(builder.Configuration);
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection(nameof(Appsettings.SendGrid)));
builder.Services.Configure<AuthorizationSettings>(builder.Configuration.GetSection(nameof(Appsettings.Authorization)));
builder.Services.Configure<GoogleAdsApi>(builder.Configuration.GetSection(nameof(Appsettings.GoogleAdsApi)));
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ISendGridClientFacade, SendGridClientFacade>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ISendGridClient, SendGridClient>(serviceProvider => new SendGridClient(appsettings.SendGrid.ApiKey));

// ====== Google Ads Client Setup ======
builder.Services.AddScoped<IGoogleAdsClientFacade, GoogleAdsClientFacade>();
GoogleAdsConfig config = new GoogleAdsConfig()
{
    DeveloperToken = appsettings.GoogleAdsApi.DeveloperToken,
    OAuth2Mode = OAuth2Flow.APPLICATION, //might have to be service account?
    OAuth2ClientId = appsettings.GoogleAdsApi.OAuth2ClientId,
    OAuth2ClientSecret = appsettings.GoogleAdsApi.OAuth2ClientSecret,
    OAuth2RefreshToken = appsettings.GoogleAdsApi.OAuth2RefreshToken

};
builder.Services.AddScoped<AdsClient<GoogleAdsConfig>, GoogleAdsClient>(serviceProvider => new GoogleAdsClient(config));
// ====== Google Ads Client Setup ======

builder.Services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();
builder.Services.AddMediatR(typeof(Veveve.Api.Infrastructure.Database.AppDbContext).Assembly); // use any type from Veveve.Api
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DbConnection)));
});

builder.Services.AddAuth(appsettings.Authorization);
builder.Services.AddCustomSwagger();
var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.EnvironmentName != "Testing") // dont run this for integration tests
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>()!;
        var mediator = scope.ServiceProvider.GetService<IMediator>()!;
        await dbContext.Database.MigrateAsync();
        await mediator.Send(new EnsureDefaultAdminUsers.Command());
    }
}

// ====== Pipeline ======
app.UseCustomSwagger();
app.UseHsts();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// this is necessary for the test project to access Program
public partial class Program { }