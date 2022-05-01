using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Infrastructure.Database.Entities.Builders;
using Npgsql;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class UpdateAccount
{
    public record Command(int Id, int GoogleAdsAccountId, string GoogleAdsAccountName) : IRequest<AccountEntity>;

    public class Handler : IRequestHandler<Command, AccountEntity>
    {
        private readonly AppDbContext _dbContext;

        public Handler(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AccountEntity> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingAccount = await _dbContext.Accounts
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if (existingAccount == null)
                throw new NotFoundException(ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST);

            var builder = new AccountBuilder(existingAccount)
                .WithGoogleAdsAccount(request.GoogleAdsAccountId, request.GoogleAdsAccountName);
            
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is PostgresException ex &&
                    ex.SqlState == "23505" && 
                    ex.ConstraintName?.Contains(nameof(AccountEntity.GoogleAdsAccountId)) == true)
                    throw new ConflictException(ErrorCodesEnum.ACCOUNT_GOOGLEADSID_ALREADY_EXIST);

                throw;
            }
            
            return existingAccount;
        }
    }
}