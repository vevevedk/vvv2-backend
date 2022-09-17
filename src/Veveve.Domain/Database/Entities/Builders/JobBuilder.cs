using System.Text.Json;

namespace Veveve.Domain.Database.Entities.Builders;

public class JobBuilder
{
    private JobEntity _object;

    public JobBuilder()
    {
        _object = new JobEntity();
        WithJobStatus(JobStatusEnum.Pending);
    }

    public JobBuilder(JobEntity existingJob)
    {
        _object = existingJob;
    }

    public JobBuilder WithJobStatus(JobStatusEnum jobStatus)
    {
        _object.JobStatus = jobStatus;
        return this;
    }

    public JobBuilder WithBody(dynamic body)
    {
        _object.Body = JsonSerializer.Serialize(body);
        return this;
    }

    public JobBuilder WithFeatureName(JobFeatureNameEnum featureName)
    {
        _object.FeatureName = featureName;
        return this;
    }

    public JobBuilder WithClientId(int clientId)
    {
        _object.ClientId = clientId;
        return this;
    }

    


    public static implicit operator JobEntity(JobBuilder builder) => builder.Build();

    public JobEntity Build()
    {
        return _object;
    }
}