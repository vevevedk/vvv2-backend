using System;

namespace Veveve.Domain.Database.Entities
{
    public class JobEntity : BaseEntity
    {
        public JobEntity()
        {
            // Intentionally Empty
        }

        public JobStatusEnum JobStatus { get; set; }
        public string Body { get; set; } = null!;
        public JobFeatureNameEnum FeatureName { get; set; }
        public int ClientId { get; set; }

        public ClientEntity Client { get; set; } = null!;
        // public RecurringJob Job { get; set; }
        public ICollection<JobErrorEntity> errors { get; set; } = new List<JobErrorEntity>();
    }
}

public enum JobFeatureNameEnum
{
    CreateNegativeKeywords
}

public enum JobStatusEnum
{
    Pending,
    Running,
    Done
}