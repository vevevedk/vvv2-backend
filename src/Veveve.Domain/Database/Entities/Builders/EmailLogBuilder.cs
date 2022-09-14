namespace Veveve.Domain.Database.Entities.Builders;

public class EmailLogBuilder
{
    private EmailLogEntity _object;

    public EmailLogBuilder()
    {
        _object = new EmailLogEntity();
    }

    public EmailLogBuilder WithEmail(string email)
    {
        _object.Email = email;
        return this;
    }

    public EmailLogBuilder WithReference(Guid reference)
    {
        _object.Reference = reference;
        return this;
    }

    public EmailLogBuilder WithEmailType(EmailTypeEnum emailType)
    {
        _object.EmailType = emailType;
        return this;
    }

    public static implicit operator EmailLogEntity(EmailLogBuilder builder) => builder.Build();

    public EmailLogEntity Build()
    {
        return _object;
    }
}