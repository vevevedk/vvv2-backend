using System.Threading;
using System.Threading.Tasks;
using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Microsoft.Data.SqlClient;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Commands.Users;

public static class UpdateUser
{
    public record Command(int Id, string FullName, string Email) : IRequest<UserEntity>;

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
                throw new NotFoundException(ErrorCodesEnum.User_ID_DOESNT_EXIST);    
            
            existingUser.FullName = request.FullName;
            existingUser.Email = request.Email;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is SqlException sqlEx &&
                    sqlEx.Number == 2601)
                    throw new ConflictException(ErrorCodesEnum.User_EMAIL_ALREADY_EXIST);

                throw;
            }

            return existingUser;
        }
    }
}