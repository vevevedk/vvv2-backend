using Veveve.Domain.Database.Entities;
using Veveve.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Domain.Database;
using Npgsql;
using Veveve.Domain.Database.Entities.Builders;

namespace Veveve.Domain.Commands.Users;

public static class UpdateUser
{
    public record Command(int Id, int? ClientId, string FullName, string Email, bool? IsAdmin) : IRequest<UserEntity>;

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

            var existingUser = await _dbContext.Users
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if(existingUser == null)
                throw new NotFoundException(ErrorCodesEnum.USER_ID_DOESNT_EXIST);    
            
            var builder = new UserBuilder(existingUser)
                .WithFullName(request.FullName)
                .WithEmail(request.Email);

            if(request.ClientId.HasValue)
            {
                var group = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Id == request.ClientId);
                if(group == null)
                    throw new NotFoundException(ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST);
                builder.WithClient(group);
            }
            if(request.IsAdmin.HasValue)
            {
                var existingAdminClaim = existingUser.Claims.FirstOrDefault(x => x.ClaimType == ClaimTypeEnum.Admin);

                if(request.IsAdmin == true && existingAdminClaim == null)
                    builder.WithClaim(new UserClaimBuilder(ClaimTypeEnum.Admin));
                else if(request.IsAdmin == false && existingAdminClaim != null)
                    builder.RemoveClaim(existingAdminClaim);
            }

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

            return existingUser;
        }
    }
}