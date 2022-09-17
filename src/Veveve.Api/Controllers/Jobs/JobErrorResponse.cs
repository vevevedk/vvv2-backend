
using Veveve.Domain.Database.Entities;

namespace Veveve.Api.Controllers.Jobs;

public class JobErrorResponse
{
    public JobErrorResponse(JobErrorEntity jobErrorEntity)
    {
        RowDetails = jobErrorEntity.RowDetails;
        ErrorCode = jobErrorEntity.ErrorCode;
        ExceptionDetails = jobErrorEntity.ExceptionDetails;
    }

    public string RowDetails { get; set; } = null!;
    public string ErrorCode { get; set; } = null!;
    public string ExceptionDetails { get; set; } = null!;
}