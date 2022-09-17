using MediatR;
using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;
using Veveve.Domain.Database.Entities.Builders;
using Veveve.Domain.Models.Jobs;

namespace Veveve.Domain.Commands.Keywords;

public static class CreateKeywords
{
    public record Command(int ClientId, string GoogleAdsCustomerId, bool Negative, IEnumerable<CreateKeywordsItem> Keywords) : IRequest<JobEntity>;

    public class Handler : IRequestHandler<Command, JobEntity>
    {
        private readonly AppDbContext _dbContext;

        public Handler(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<JobEntity> Handle(Command request, CancellationToken cancellationToken)
        {
            var jobBody = new CreateNegativeKeywordsJobModel
            {
                GoogleAdsAccountId = request.GoogleAdsCustomerId,
                Keywords = request.Keywords
            };

            var newJob = new JobBuilder()
                .WithClientId(request.ClientId)
                .WithFeatureName(JobFeatureNameEnum.CreateNegativeKeywords) // TODO: change this to use request.Negative once we have a feature for creating positive keywords
                .WithBody(jobBody)
                .Build();

            await _dbContext.JobQueue.AddAsync(newJob);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return newJob;
        }
    }
}