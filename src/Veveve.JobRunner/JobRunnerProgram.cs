

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Veveve.Domain.Configuration;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    // .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDomain(context.Configuration);
        services.AddScoped<IJobRunnerService, JobRunnerService>();
        services.AddHostedService<JobRunnerProgram>();
    })
    .UseSerilog()
    .Build();

await host.RunAsync();

public class JobRunnerProgram : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly int _milliSecondsToWait = 5000;

    // we need to start a new scope for each iteration of the background service
    public JobRunnerProgram(IMediator mediator, IServiceScopeFactory scopeFactory)
    {
        _mediator = mediator;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // initiate a scope. all services within this scope are isolated.
            // this allows us to inject scoped services without risk of them becoming singletons
            using var scope = _scopeFactory.CreateAsyncScope();
            var schedulerService = scope.ServiceProvider.GetRequiredService<IJobRunnerService>();

            await schedulerService.ProcessQueue();

            await Task.Delay(_milliSecondsToWait, stoppingToken);
        }
    }
}
