using Veveve.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Domain.Database;

namespace Veveve.Domain.Commands.Clients;

public static class DeleteClient
{
    public record Command(int Id) : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;

        public Handler(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var existingClient = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Id == request.Id);
            if(existingClient == null)
                throw new NotFoundException(ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST);    
            
            _dbContext.Remove(existingClient);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}