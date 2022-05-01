using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class EnsureDefaultAdminAccounts
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
            var existingAccounts = await _dbContext.Accounts.Select(x => x.Email).ToListAsync();
            foreach (var defaultAcc in _appSettings.DefaultAdminAccounts)
            {
                if (!existingAccounts.Contains(defaultAcc.Email, StringComparer.InvariantCultureIgnoreCase))
                    await _mediator.Send(new CreateAccount.Command(defaultAcc.FullName, defaultAcc.Email, true));
            }
        }
    }
}