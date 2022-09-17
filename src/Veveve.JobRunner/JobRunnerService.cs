

using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Domain.Database;
using Veveve.Domain.Models.Jobs;

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
        var queue = await _dbContext.JobQueue.Where(x => x.JobStatus == JobStatusEnum.Pending).ToListAsync();
        foreach (var queueItem in queue)
        {
            switch (queueItem.FeatureName)
            {
                case JobFeatureNameEnum.CreateNegativeKeywords:
                    var body = JsonSerializer.Deserialize<CreateNegativeKeywordsJobModel>(queueItem.Body);
                    // await _mediator.Send(new CreateNegativeKeywordsCommand(queueItem.ClientId, body.GoogleAdsAccountId, body.Keywords));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
