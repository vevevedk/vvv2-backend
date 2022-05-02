namespace Veveve.Api.Infrastructure.Database.Entities.Builders;

public class AccountBuilder
{
    private AccountEntity _object;

    public AccountBuilder()
    {
        _object = new AccountEntity();
    }

    public AccountBuilder(AccountEntity existingAccount)
    {
        _object = existingAccount;
    }

    public AccountBuilder WithGoogleAdsAccount(string googleAdsAccountId, string googleAdsAccountName)
    {
        _object.GoogleAdsAccountId = googleAdsAccountId;
        _object.GoogleAdsAccountName = googleAdsAccountName;
        return this;
    }

    public AccountBuilder WithClient(ClientEntity client)
    {
        _object.ClientId = client.Id;
        _object.Client = client;
        return this;
    }
    
    public static implicit operator AccountEntity(AccountBuilder builder) => builder.Build();

    public AccountEntity Build()
    {
        if(_object.GoogleAdsAccountId == null || _object.GoogleAdsAccountName == null)
            throw new InvalidOperationException("GoogleAdsAccountId and GoogleAdsAccountName must be set.");
        return _object;
    }
}