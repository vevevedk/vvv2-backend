
using Veveve.Domain.Database.Entities;

namespace Veveve.Api.Controllers.Jobs;

public class JobResponse : BaseResponse
{
    public JobResponse(JobEntity jobEntity)
    {
        Id = jobEntity.Id;
        JobStatus = jobEntity.JobStatus;
        Body = jobEntity.Body;
        FeatureName = jobEntity.FeatureName;
        CreatedDate = jobEntity.CreatedDate;
        LastModifiedDate = jobEntity.LastModifiedDate;
        Errors = jobEntity.errors.Select(x => new JobErrorResponse(x)).ToList();
    }

    public int Id { get; set; }

    public JobStatusEnum JobStatus { get; set; }
    public string Body { get; set; }
    public JobFeatureNameEnum FeatureName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public IEnumerable<JobErrorResponse> Errors { get; set; }
}