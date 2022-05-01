using System;
using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Domain.Services;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class UpdateAccountPassword
{
    public record Command(Guid ResetPasswordToken, string Password) : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordService _passwordService;

        public Handler(AppDbContext dbContext, IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.ResetPasswordToken == request.ResetPasswordToken);
            if (account == null)
                throw new BusinessRuleException(ErrorCodesEnum.ACCOUNT_RESETPASSWORD_TOKEN_INVALID);

            var hashSalt = _passwordService.EncryptPassword(request.Password);
            account.Password = hashSalt.Hash;
            account.Salt = hashSalt.Salt;
            account.ResetPasswordToken = null;

            await _dbContext.SaveChangesAsync();
        }
    }
}