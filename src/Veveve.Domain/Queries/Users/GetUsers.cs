using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Domain.Queries.Users;

public static class GetUsers
{
    public record Query(int ClientId) : IRequest<IEnumerable<UserEntity>>;

    public class Handler : IRequestHandler<Query, IEnumerable<UserEntity>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UserEntity>> Handle(Query request, CancellationToken cancellationToken)
        {
            var users = await _dbContext.Users
                .Include(x => x.Claims)
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId)
                .ToArrayAsync();
            return users;
        }
    }
}