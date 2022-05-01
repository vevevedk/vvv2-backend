namespace Veveve.Api.Infrastructure.Database.Entities.Builders;

public class ClientBuilder
{
    private ClientEntity _object;

    public ClientBuilder(string name)
    {
        _object = new ClientEntity();
        WithName(name);
    }

    public ClientBuilder(ClientEntity existingClient)
    {
        _object = existingClient;
    }

    public ClientBuilder WithName(string name)
    {
        _object.Name = name;
        return this;
    }

    public ClientBuilder WithAccount(AccountEntity account)
    {
        account.Client = _object;
        _object.Accounts.Add(account);
        return this;
    }

    public static implicit operator ClientEntity(ClientBuilder builder) => builder.Build();

    public ClientEntity Build()
    {
        return _object;
    }
}