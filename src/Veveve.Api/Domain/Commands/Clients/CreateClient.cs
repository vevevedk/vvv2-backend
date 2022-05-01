using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities.Builders;

namespace Veveve.Api.Domain.Commands.Clients;

public static class CreateClient
{
    public record Command(string Name) : IRequest<ClientEntity>;

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
            var newClient = new ClientBuilder(request.Name);
            await _dbContext.Clients.AddAsync(newClient);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return newClient;
        }
    }
}