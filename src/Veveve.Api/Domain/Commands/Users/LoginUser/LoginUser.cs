using Veveve.Api.Domain.Services;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Infrastructure.Authorization;

namespace Veveve.Api.Domain.Commands.Users;

public static class LoginUser
{
    public record Command(string Email, string Password) : IRequest<LoginUserResult>;

    public class Handler : IRequestHandler<Command, LoginUserResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenHelper _jwtTokenHelper;

        public Handler(
            AppDbContext dbContext,
            IPasswordService passwordService,
            IJwtTokenHelper jwtTokenHelper)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task<LoginUserResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var User = await _dbContext.Users
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Email == request.Email);
            if (User == null)
                throw new BusinessRuleException(ErrorCodesEnum.User_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            if (string.IsNullOrEmpty(User.Password))
                throw new BusinessRuleException(ErrorCodesEnum.User_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            if (_passwordService.VerifyPassword(request.Password, User.Salt!, User.Password) == false)
                throw new BusinessRuleException(ErrorCodesEnum.User_LOGIN_EMAIL_OR_PASSWORD_INVALID);

            var token = _jwtTokenHelper.GenerateJwtToken(User);
            return new LoginUserResult(User, token);
        }
    }
}