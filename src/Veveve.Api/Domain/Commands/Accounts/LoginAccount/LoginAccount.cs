using Veveve.Api.Domain.Services;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Infrastructure.Authorization;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class LoginAccount
{
    public record Command(string Email, string Password) : IRequest<LoginAccountResult>;

    public class Handler : IRequestHandler<Command, LoginAccountResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenHelper _jwtTokenHelper;

        public Handler(
            AppDbContext dbContext,
            IPasswordService passwordService,
            IJwtTokenHelper jwtTokenHelper)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task<LoginAccountResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Email == request.Email);
            if (account == null)
                throw new BusinessRuleException(ErrorCodesEnum.ACCOUNT_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            if (string.IsNullOrEmpty(account.Password))
                throw new BusinessRuleException(ErrorCodesEnum.ACCOUNT_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            if (_passwordService.VerifyPassword(request.Password, account.Salt!, account.Password) == false)
                throw new BusinessRuleException(ErrorCodesEnum.ACCOUNT_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            var token = _jwtTokenHelper.GenerateJwtToken(account);
            return new LoginAccountResult(account, token);
        }
    }
}