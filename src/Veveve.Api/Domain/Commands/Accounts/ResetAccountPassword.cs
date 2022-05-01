using System;
using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Domain.Services;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Domain.Commands.Emails;

namespace Veveve.Api.Domain.Commands.Accounts;

public static class ResetAccountPassword
{
    public record Command(string Email) : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(
            AppDbContext dbContext,
            IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (account == null)
                throw new NotFoundException(ErrorCodesEnum.ACCOUNT_EMAIL_DOESNT_EXIST);

            var resetToken = Guid.NewGuid();
            account.ResetPasswordToken = resetToken;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _mediator.Send(new SendResetPasswordMail.Command(account.Email, account.FullName, resetToken));
        }
    }
}