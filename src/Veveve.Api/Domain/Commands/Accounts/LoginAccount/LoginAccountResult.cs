using Veveve.Api.Infrastructure.Database.Entities;

namespace Veveve.Api.Domain.Commands.Accounts;

public class LoginAccountResult
{
    public LoginAccountResult(AccountEntity account, string jwtToken)
    {
        Account = account;
        JwtToken = jwtToken;
    }

    public AccountEntity Account { get; set; }
    public string JwtToken { get; set; }
}