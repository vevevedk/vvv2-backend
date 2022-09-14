using Veveve.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;

namespace Veveve.Domain.Commands.Clients;

public static class AssumeClient
{
    /// <summary>
    /// Returns user and the targeted client if the user has access to the client.
    /// Otherwise throws an exception.
    /// </summary>
    public record Command(int UserId, int TargetClientId) : IRequest<(UserEntity, ClientEntity)>;

    public class Handler : IRequestHandler<Command, (UserEntity, ClientEntity)>
    {
        private readonly AppDbContext _dbContext;

        public Handler(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(UserEntity, ClientEntity)> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user == null)
                throw new Exception("user is null");

            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(x => x.Id == request.TargetClientId);
            if (client == null)
                throw new NotFoundException(ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST);

            // return named tuple with User and targeted Client
            return (user, client);
        }
    }
}