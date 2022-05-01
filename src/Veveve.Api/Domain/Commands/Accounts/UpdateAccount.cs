using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Microsoft.Data.SqlClient;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class UpdateAccount
{
    public record Command(int Id, string FullName, string Email) : IRequest<AccountEntity>;

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

            var existingAccount = await _dbContext.Accounts
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if(existingAccount == null)
                throw new NotFoundException(ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST);    
            
            existingAccount.FullName = request.FullName;
            existingAccount.Email = request.Email;

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

            return existingAccount;
        }
    }
}