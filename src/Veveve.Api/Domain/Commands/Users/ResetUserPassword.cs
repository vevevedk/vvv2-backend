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

namespace Veveve.Api.Domain.Commands.Users;

public static class ResetUserPassword
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
            var User = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (User == null)
                throw new NotFoundException(ErrorCodesEnum.User_EMAIL_DOESNT_EXIST);

            var resetToken = Guid.NewGuid();
            User.ResetPasswordToken = resetToken;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _mediator.Send(new SendResetPasswordMail.Command(User.Email, User.FullName, resetToken));
        }
    }
}