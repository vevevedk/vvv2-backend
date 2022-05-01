using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Domain.Commands.Emails;
using Npgsql;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Commands.Users;

public static class CreateUser
{
    public record Command(string FullName, string Email, bool IsAdmin) : IRequest<UserEntity>;

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

            var newUser = new UserEntity(request.FullName, request.Email);
            newUser.ResetPasswordToken = Guid.NewGuid();
            newUser.Claims.Add(new UserClaimEntity(ClaimTypeEnum.User));
            if(request.IsAdmin)
                newUser.Claims.Add(new UserClaimEntity(ClaimTypeEnum.Admin));

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
                    throw new ConflictException(ErrorCodesEnum.User_EMAIL_ALREADY_EXIST);

                throw;
            }

            await _mediator.Send(new SendResetPasswordMail.Command(request.Email, request.FullName, newUser.ResetPasswordToken.Value));
            return newUser;
        }
    }
}