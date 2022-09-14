using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Veveve.Domain.PipelineBehaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : MediatR.IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {

        TResponse response;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            response = await next();
        }
        finally
        {
            stopwatch.Stop();
            var requestName = request.GetType().FullName!.Split(".").Last();
            _logger.LogInformation("{Request} finished in {Elapsed} ms", requestName, stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}