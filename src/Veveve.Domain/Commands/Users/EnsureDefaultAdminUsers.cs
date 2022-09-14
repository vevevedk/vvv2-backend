using Veveve.Domain.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Veveve.Domain.Database.Entities.Builders;
using Veveve.Domain.Models.Options;

namespace Veveve.Domain.Commands.Users;

public static class EnsureDefaultAdminUsers
{
    public record Command() : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly IMediator _mediator;
        private readonly AppDbContext _dbContext;
        private readonly DefaultAdminDataOptions _adminDataOptions;

        public Handler(
            IMediator mediator,
            AppDbContext dbContext,
            IOptions<DefaultAdminDataOptions> adminDataOptions)
        {
            _mediator = mediator;
            _dbContext = dbContext;
            _adminDataOptions = adminDataOptions.Value;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Name == _adminDataOptions.DefaultAdminClientName);
            if (client == null)
            {
                client = new ClientBuilder(_adminDataOptions.DefaultAdminClientName);
                _dbContext.Clients.Add(client);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var existingUsers = await _dbContext.Users.Select(x => x.Email).ToListAsync();
            foreach (var defaultAcc in _adminDataOptions.DefaultAdminUsers)
            {
                if (!existingUsers.Contains(defaultAcc.Email, StringComparer.InvariantCultureIgnoreCase))
                    await _mediator.Send(new CreateUser.Command(client.Id, defaultAcc.FullName, defaultAcc.Email, true));
            }
        }
    }
}