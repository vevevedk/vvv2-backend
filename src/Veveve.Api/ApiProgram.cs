using Microsoft.EntityFrameworkCore;
using MediatR;
using Veveve.Domain.Commands.Users;
using Veveve.Domain.Database;
using Veveve.Api.Authorization;
using System.Text.Json.Serialization;
using Veveve.Api;
using Veveve.Api.Swagger;
using Serilog;
using Serilog.Events;
using Veveve.Domain.Models.Options;
using Veveve.Api.Middleware;
using Veveve.Api.ErrorHandling;
using Veveve.Domain.Configuration;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();



var authorizationOptions = builder.Configuration.GetSection(AuthorizationOptions.SectionName).Get<AuthorizationOptions>();
builder.Services.AddAuth(authorizationOptions);
builder.Services.AddCustomSwagger();
builder.Services.AddDomain(builder.Configuration);
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
public partial class ApiProgram { }