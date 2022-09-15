

using MediatR;
using Veveve.Domain.Database;

public interface IJobRunnerService
{
    Task ProcessQueue();
}

public class JobRunnerService : IJobRunnerService
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _dbContext;

    public JobRunnerService(IMediator mediator, AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task ProcessQueue()
    {
        var queue = _dbContext.Queues.Where(x => x.JobStatus == JobStatusEnum.Pending).ToList();
        foreach(var queueItem in queue)
        {
            // TODO Flip based on FeatureName. Perhaps as an Enum? Seems better.
            //await _mediator.Send(queueItem);
        }
    }
}
