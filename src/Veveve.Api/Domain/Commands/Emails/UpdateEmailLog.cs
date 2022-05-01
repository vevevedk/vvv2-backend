using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Api.Domain.Commands.Emails;

public static class UpdateEmailLog
{
    public record Command(Guid reference, EmailEventEnum emailEvent) : IRequest;

    public class Handler : MediatR.AsyncRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var emailLog = await _dbContext.EmailLogs.FirstOrDefaultAsync(x => x.Reference == request.reference);
            if(emailLog == null)
                throw new Exception($"No email log with reference {request.reference} was found");

            emailLog.Event = request.emailEvent;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}