using System;
using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Domain.Services;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Commands.Users;

public static class UpdateUserPassword
{
    public record Command(Guid ResetPasswordToken, string Password) : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordService _passwordService;

        public Handler(AppDbContext dbContext, IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var User = await _dbContext.Users.FirstOrDefaultAsync(x => x.ResetPasswordToken == request.ResetPasswordToken);
            if (User == null)
                throw new BusinessRuleException(ErrorCodesEnum.User_RESETPASSWORD_TOKEN_INVALID);

            var hashSalt = _passwordService.EncryptPassword(request.Password);
            User.Password = hashSalt.Hash;
            User.Salt = hashSalt.Salt;
            User.ResetPasswordToken = null;

            await _dbContext.SaveChangesAsync();
        }
    }
}