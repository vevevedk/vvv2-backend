using System;

namespace Veveve.Domain.Database.Entities
{
    public class QueueEntity : BaseEntity
    {
        public QueueEntity()
        {
            // Intentionally Empty
        }

        public JobStatusEnum JobStatus { get; set; }
        public string Body { get; set; } = null!;
        public string FeatureName { get; set; } = null!; // Maybe an Enum?
        public ClientEntity Client { get; set; } = null!;
        // public RecurringJob Job { get; set; }
        public ICollection<JobErrorEntity> errors { get; set; } = new List<JobErrorEntity>();
    }
}


public enum JobStatusEnum
{
    Pending,
    Running,
    Done
}