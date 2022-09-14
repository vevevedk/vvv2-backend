using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Veveve.Domain.Database.Entities.Builders;
using Veveve.Domain.Commands.Users;
using Veveve.Domain.Exceptions;

namespace Veveve.Api.Tests.Domain.Commands.Users;

public class DeleteUserTests : TestBase
{
    private IRequestHandler<DeleteUser.Command> _sut;

    public DeleteUserTests()
    {
        _sut = new DeleteUser.Handler(new AppDbContext(_dbOptions));
    }

    [Fact]
    public async void DeleteUser_ThrowsNotFoundException_WhenIdDoesntExist()
    {
        // Arrange
        var command = new DeleteUser.Command(123);

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public async void DeleteUser_DeletesUser()
    {
        // Arrange
        UserEntity User;
        using (var context = new AppDbContext(_dbOptions))
        {
            User = new UserBuilder("asdasd", "jkh214h21@mail.com")
                .WithTestClient()
                .WithClaim(ClaimTypeEnum.User);
            context.Users.Add(User);
            await context.SaveChangesAsync();
        }
        var command = new DeleteUser.Command(User.Id);

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var accAfter = await context.Users
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == User.Id);

            Assert.Null(accAfter);
        }
    }
}