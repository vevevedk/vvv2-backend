namespace Veveve.Api.Infrastructure.Database.Entities.Builders;

public class UserClaimBuilder
{
    private UserClaimEntity _object;

    public UserClaimBuilder(ClaimTypeEnum claimType)
    {
        _object = new UserClaimEntity(claimType);
    }

    public static implicit operator UserClaimEntity(UserClaimBuilder builder) => builder.Build();
    public UserClaimEntity Build() => _object;
}