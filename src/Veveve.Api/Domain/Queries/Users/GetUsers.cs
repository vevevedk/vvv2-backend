using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Api.Domain.Queries.Users;

public static class GetUsers
{
    public record Query() : IRequest<IEnumerable<UserEntity>>;

    public class Handler : IRequestHandler<Query, IEnumerable<UserEntity>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UserEntity>> Handle(Query request, CancellationToken cancellationToken)
        {
            var Users = await _dbContext.Users
                .AsNoTracking()
                .ToArrayAsync();
            return Users;
        }
    }
}