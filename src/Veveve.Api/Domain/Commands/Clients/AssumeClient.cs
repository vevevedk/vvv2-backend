using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Infrastructure.Authorization;

namespace Veveve.Api.Domain.Commands.Clients;

public static class AssumeClient
{
    /// <summary>
    /// Returns JWT token
    /// </summary>
    public record Command(int ClientId) : IRequest<string>;

    public class Handler : IRequestHandler<Command, string>
    {
        private readonly AppDbContext _dbContext;
        private readonly IJwtTokenHelper _jwtTokenHelper;

        public Handler(
            AppDbContext dbContext,
            IJwtTokenHelper jwtTokenHelper)
        {
            _dbContext = dbContext;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _jwtTokenHelper.GetUserId();
            if (userId == null)
                throw new Exception("UserId is null");

            var user = await _dbContext.Users
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == userId);
           
            if (user == null)
                throw new Exception("user is null");

            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(x => x.Id == request.ClientId);
            if (client == null)
                throw new NotFoundException(ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST);

            var token = _jwtTokenHelper.GenerateJwtToken(user, client);
            return token;
        }
    }
}