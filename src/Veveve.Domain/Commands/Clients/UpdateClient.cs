using Veveve.Domain.Database.Entities;
using Veveve.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities.Builders;
using Npgsql;

namespace Veveve.Domain.Commands.Clients;

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

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is PostgresException ex &&
                    ex.SqlState == "23505" && 
                    ex.ConstraintName?.Contains(nameof(ClientEntity.Name)) == true)
                    throw new ConflictException(ErrorCodesEnum.CLIENT_NAME_ALREADY_EXISTS);

                throw;
            }
            return existingClient;
        }
    }
}