using System;

namespace Veveve.Domain.Database.Entities
{
    public class JobErrorEntity : BaseEntity
    {
        public JobErrorEntity()
        {
            // Intentionally Empty
        }

        // Meta
        public string JobId { get; set; } = null!;

        public string RowDetails { get; set; } = null!;
        public string ErrorCode { get; set; } = null!;
        public string ExceptionDetails { get; set; } = null!;
    }
}
