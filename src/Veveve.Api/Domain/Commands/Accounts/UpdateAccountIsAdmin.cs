using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;
using System.Linq;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class UpdateAccountIsAdmin
{
    public record Command(int Id, bool IsAdmin) : IRequest<AccountEntity>;

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
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if(existingAccount == null)
                throw new NotFoundException(ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST);    
            
            var existingAdminClaim = existingAccount.Claims.FirstOrDefault(x => x.ClaimType == ClaimTypeEnum.Admin);

            if(request.IsAdmin && existingAdminClaim == null)
                existingAccount.Claims.Add(new AccountClaimEntity(ClaimTypeEnum.Admin));
            else if(!request.IsAdmin && existingAdminClaim != null)
                existingAccount.Claims.Remove(existingAdminClaim);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existingAccount;
        }
    }
}