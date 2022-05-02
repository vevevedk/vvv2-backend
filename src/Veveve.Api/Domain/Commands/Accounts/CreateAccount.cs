using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities.Builders;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class CreateAccount
{
    public record Command(int ClientId, string GoogleAdsAccountId, string GoogleAdsAccountName) : IRequest<AccountEntity>;

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
            var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Id == request.ClientId);
            if (client == null)
                throw new NotFoundException(ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST);

            var newAccount = new AccountBuilder()
                .WithGoogleAdsAccount(request.GoogleAdsAccountId, request.GoogleAdsAccountName)
                .WithClient(client)
                .Build();
            await _dbContext.Accounts.AddAsync(newAccount);

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
            return newAccount;
        }
    }
}