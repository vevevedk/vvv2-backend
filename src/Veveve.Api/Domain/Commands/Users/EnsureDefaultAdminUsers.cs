using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Veveve.Api.Infrastructure.Database.Entities.Builders;

namespace Veveve.Api.Domain.Commands.Users;

public static class EnsureDefaultAdminUsers
{
    public record Command() : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly IMediator _mediator;
        private readonly AppDbContext _dbContext;
        private readonly Appsettings _appSettings;

        public Handler(
            IMediator mediator,
            AppDbContext dbContext,
            IOptions<Appsettings> appSettings)
        {
            _mediator = mediator;
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Name == _appSettings.DefaultAdminClientName);
            if (client == null)
            {
                client = new ClientBuilder(_appSettings.DefaultAdminClientName);
                _dbContext.Clients.Add(client);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var existingUsers = await _dbContext.Users.Select(x => x.Email).ToListAsync();
            foreach (var defaultAcc in _appSettings.DefaultAdminUsers)
            {
                if (!existingUsers.Contains(defaultAcc.Email, StringComparer.InvariantCultureIgnoreCase))
                    await _mediator.Send(new CreateUser.Command(client.Id, defaultAcc.FullName, defaultAcc.Email, true));
            }
        }
    }
}