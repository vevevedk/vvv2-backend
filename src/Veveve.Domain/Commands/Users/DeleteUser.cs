using Veveve.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Veveve.Domain.Database;

namespace Veveve.Domain.Commands.Users;

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
                throw new NotFoundException(ErrorCodesEnum.USER_ID_DOESNT_EXIST);    
            
            _dbContext.Remove(existingUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}