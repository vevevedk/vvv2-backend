namespace Veveve.Api.Infrastructure.Database.Entities.Builders;

public class UserBuilder
{
    private UserEntity _object;

    public UserBuilder(UserEntity existingUser)
    {
        _object = existingUser;
    }

    public UserBuilder(string fullName, string email, bool withUserClaim = true)
    {
        _object = new UserEntity();
        WithFullName(fullName);
        WithEmail(email);
        WithClaim(new UserClaimEntity(ClaimTypeEnum.User));
    }

    public UserBuilder WithFullName(string fullName)
    {
        _object.FullName = fullName;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _object.Email = email;
        return this;
    }

    public UserBuilder WithPassword(string passwordHash, byte[] salt)
    {
        _object.Password = passwordHash;
        _object.Salt = salt;
        return this;
    }

    public UserBuilder WithResetPasswordToken(Guid token)
    {
        _object.ResetPasswordToken = token;
        return this;
    }

    public UserBuilder WithClient(ClientEntity client)
    {
        _object.Client = client;
        _object.ClientId = client.Id;
        return this;
    }

    public UserBuilder WithTestClient()
    {
        _object.Client = new ClientBuilder("TestClient");
        _object.ClientId = _object.Client.Id;
        return this;
    }

    public UserBuilder WithClaim(UserClaimEntity claim)
    {
        claim.User = _object;
        _object.Claims.Add(claim);
        return this;
    }

    public UserBuilder RemoveClaim(UserClaimEntity claim)
    {
        _object.Claims.Remove(claim);
        return this;
    }

    public static implicit operator UserEntity(UserBuilder builder) => builder.Build();

    public UserEntity Build()
    {
        return _object;
    }
}