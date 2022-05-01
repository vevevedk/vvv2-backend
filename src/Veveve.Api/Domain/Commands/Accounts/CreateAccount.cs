using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Microsoft.Data.SqlClient;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Domain.Commands.Emails;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class CreateAccount
{
    public record Command(string FullName, string Email, bool IsAdmin) : IRequest<AccountEntity>;

    public class Handler : IRequestHandler<Command, AccountEntity>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(
            AppDbContext dbContext,
            IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<AccountEntity> Handle(Command request, CancellationToken cancellationToken)
        {

            var newAccount = new AccountEntity(request.FullName, request.Email);
            newAccount.ResetPasswordToken = Guid.NewGuid();
            newAccount.Claims.Add(new AccountClaimEntity(ClaimTypeEnum.User));
            if(request.IsAdmin)
                newAccount.Claims.Add(new AccountClaimEntity(ClaimTypeEnum.Admin));

            await _dbContext.Accounts.AddAsync(newAccount);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is SqlException sqlEx &&
                    sqlEx.Number == 2601)
                    throw new ConflictException(ErrorCodesEnum.ACCOUNT_EMAIL_ALREADY_EXIST);

                throw;
            }

            await _mediator.Send(new SendResetPasswordMail.Command(request.Email, request.FullName, newAccount.ResetPasswordToken.Value));
            return newAccount;
        }
    }
}