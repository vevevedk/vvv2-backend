using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Domain.Queries.Clients;

public static class GetClients
{
    public record Query() : IRequest<IEnumerable<ClientEntity>>;

    public class Handler : IRequestHandler<Query, IEnumerable<ClientEntity>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ClientEntity>> Handle(Query request, CancellationToken cancellationToken)
        {
            var clients = await _dbContext.Clients
                .AsNoTracking()
                .ToArrayAsync();
            return clients;
        }
    }
}