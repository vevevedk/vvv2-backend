using Veveve.Domain.Services;
using Veveve.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;

namespace Veveve.Domain.Commands.Users;

public static class LoginUser
{
    public record Command(string Email, string Password) : IRequest<UserEntity>;

    public class Handler : IRequestHandler<Command, UserEntity>
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordService _passwordService;

        public Handler(
            AppDbContext dbContext,
            IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }

        public async Task<UserEntity> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(x => x.Client)
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
                throw new BusinessRuleException(ErrorCodesEnum.USER_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            if (string.IsNullOrEmpty(user.Password))
                throw new BusinessRuleException(ErrorCodesEnum.USER_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            if (_passwordService.VerifyPassword(request.Password, user.Salt!, user.Password) == false)
                throw new BusinessRuleException(ErrorCodesEnum.USER_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            return user;
        }
    }
}