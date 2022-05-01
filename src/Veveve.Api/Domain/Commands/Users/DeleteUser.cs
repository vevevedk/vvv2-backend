using Veveve.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Commands.Users;

public static class DeleteUser
{
    public record Command(int Id) : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if(existingUser == null)
                throw new NotFoundException(ErrorCodesEnum.User_ID_DOESNT_EXIST);    
            
            _dbContext.Remove(existingUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}