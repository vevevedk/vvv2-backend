using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Infrastructure.Database.Entities.Builders;

namespace Veveve.Api.Domain.Commands.Clients;

public static class UpdateClient
{
    public record Command(int Id, string Name) : IRequest<ClientEntity>;

    public class Handler : IRequestHandler<Command, ClientEntity>
    {
        private readonly AppDbContext _dbContext;

        public Handler(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClientEntity> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingClient = await _dbContext.Clients
                .Include(x => x.Accounts)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if (existingClient == null)
                throw new NotFoundException(ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST);

            var builder = new ClientBuilder(existingClient)
                .WithName(request.Name);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existingClient;
        }
    }
}