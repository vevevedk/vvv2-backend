using Veveve.Api.Infrastructure.Database.Entities;

namespace Veveve.Api.Domain.Commands.Users;

public class LoginUserResult
{
    public LoginUserResult(UserEntity user, string jwtToken)
    {
        User = user;
        JwtToken = jwtToken;
    }

    public UserEntity User { get; set; }
    public string JwtToken { get; set; }
}