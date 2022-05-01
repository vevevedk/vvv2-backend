using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;
using System.Linq;

namespace Veveve.Api.Domain.Commands.Users;

public static class UpdateUserIsAdmin
{
    public record Command(int Id, bool IsAdmin) : IRequest<UserEntity>;

    public class Handler : IRequestHandler<Command, UserEntity>
    {
        private readonly AppDbContext _dbContext;

        public Handler(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserEntity> Handle(Command request, CancellationToken cancellationToken)
        {

            var existingUser = await _dbContext.Users
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if(existingUser == null)
                throw new NotFoundException(ErrorCodesEnum.User_ID_DOESNT_EXIST);    
            
            var existingAdminClaim = existingUser.Claims.FirstOrDefault(x => x.ClaimType == ClaimTypeEnum.Admin);

            if(request.IsAdmin && existingAdminClaim == null)
                existingUser.Claims.Add(new UserClaimEntity(ClaimTypeEnum.Admin));
            else if(!request.IsAdmin && existingAdminClaim != null)
                existingUser.Claims.Remove(existingAdminClaim);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existingUser;
        }
    }
}