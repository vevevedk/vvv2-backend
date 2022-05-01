using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class DeleteAccount
{
    public record Command(int Id) : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var existingAccount = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == request.Id);
            if(existingAccount == null)
                throw new NotFoundException(ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST);    
            
            _dbContext.Remove(existingAccount);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}