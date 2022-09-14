using Veveve.Domain.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Domain.Database;
using Veveve.Domain.Commands.Emails;
using Npgsql;
using Veveve.Domain.Exceptions;
using Veveve.Domain.Database.Entities.Builders;

namespace Veveve.Domain.Commands.Users;

public static class CreateUser
{
    public record Command(int ClientId, string FullName, string Email, bool IsAdmin) : IRequest<UserEntity>;

    public class Handler : IRequestHandler<Command, UserEntity>
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

        public async Task<UserEntity> Handle(Command request, CancellationToken cancellationToken)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Id == request.ClientId);
            if (client == null)
                throw new NotFoundException(ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST);

            var builder = new UserBuilder(request.FullName, request.Email)
                .WithClient(client)
                .WithResetPasswordToken(Guid.NewGuid())
                .WithClaim(new UserClaimBuilder(ClaimTypeEnum.User));
            if(request.IsAdmin)
                builder.WithClaim(new UserClaimBuilder(ClaimTypeEnum.Admin));

            var newUser = builder.Build();

            await _dbContext.Users.AddAsync(newUser);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is PostgresException ex &&
                    ex.SqlState == "23505" && 
                    ex.ConstraintName?.Contains(nameof(UserEntity.Email)) == true)
                    throw new ConflictException(ErrorCodesEnum.USER_EMAIL_ALREADY_EXIST);

                throw;
            }

            await _mediator.Send(new SendResetPasswordMail.Command(request.Email, request.FullName, newUser.ResetPasswordToken!.Value));
            return builder;
        }
    }
}