using Veveve.Api.Domain.Commands.Users;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Users;

public class UpdateUserIsAdminTests : TestBase
{
    private IRequestHandler<UpdateUserIsAdmin.Command, UserEntity> _sut;

    public UpdateUserIsAdminTests()
    {
        _sut = new UpdateUserIsAdmin.Handler(new AppDbContext(_dbOptions));
    }

    [Fact]
    public async void UpdateUserIsAdmin_ThrowsNotFoundException_WhenIdDoesntExist()
    {
        // Arrange
        var command = new UpdateUserIsAdmin.Command(123, false);

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public async void UpdateUserIsAdmin_UpdatesUserWithParams()
    {
        // Arrange
        UserEntity User;
        using (var context = new AppDbContext(_dbOptions))
        {
            User = new UserEntity("asdasd", "jkh214h21@mail.com");
            context.Users.Add(User);
            await context.SaveChangesAsync();
        }
        var command = new UpdateUserIsAdmin.Command(User.Id, true);

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var accAfter = await context.Users
                .Include(x => x.Claims)
                .FirstAsync(x => x.Id == User.Id);

            Assert.Equal(command.IsAdmin, accAfter.HasAdminClaim());
        }
    }
}