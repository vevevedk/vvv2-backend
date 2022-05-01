using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Api.Domain.Queries.Accounts;

public static class GetAccounts
{
    public record Query() : IRequest<IEnumerable<AccountEntity>>;

    public class Handler : IRequestHandler<Query, IEnumerable<AccountEntity>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AccountEntity>> Handle(Query request, CancellationToken cancellationToken)
        {
            var accounts = await _dbContext.Accounts
                .AsNoTracking()
                .ToArrayAsync();
            return accounts;
        }
    }
}