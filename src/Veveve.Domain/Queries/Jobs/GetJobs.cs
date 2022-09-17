using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Domain.Queries.Jobs;

public static class GetJobs
{
    public record Query(int ClientId, JobFeatureNameEnum jobFeatureName) : IRequest<IEnumerable<JobEntity>>;

    public class Handler : IRequestHandler<Query, IEnumerable<JobEntity>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<JobEntity>> Handle(Query request, CancellationToken cancellationToken)
        {
            var clients = await _dbContext.JobQueue
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && x.FeatureName == request.jobFeatureName)
                .ToArrayAsync();
            return clients;
        }
    }
}