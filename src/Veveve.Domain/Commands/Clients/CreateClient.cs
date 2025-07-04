using Veveve.Domain.Database.Entities;
using MediatR;
using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities.Builders;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Veveve.Domain.Exceptions;

namespace Veveve.Domain.Commands.Clients;

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
            return newClient;
        }
    }
}