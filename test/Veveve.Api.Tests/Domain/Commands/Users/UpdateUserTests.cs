using Veveve.Api.Domain.Commands.Users;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Users;

public class UpdateUserTests : TestBase
{
    private readonly IMediator _mockMediatr;
    private IRequestHandler<UpdateUser.Command, UserEntity> _sut;

    public UpdateUserTests()
    {
        _mockMediatr = Substitute.For<IMediator>();
        _sut = new UpdateUser.Handler(new AppDbContext(_dbOptions), _mockMediatr);
    }

    [Fact]
    public async void UpdateUser_ThrowsNotFoundException_WhenIdDoesntExist()
    {
        // Arrange
        var command = new UpdateUser.Command(123, "Donald Trump", "fhgddfgh@asd.com");

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public async void UpdateUser_UpdatesUserWithParams()
    {
        // Arrange
        UserEntity User;
        using (var context = new AppDbContext(_dbOptions))
        {
            User = new UserEntity("asdasd", "jkh214h21@mail.com");
            context.Users.Add(User);
            await context.SaveChangesAsync();
        }
        var command = new UpdateUser.Command(User.Id, "Donald Trump", "jhlk@asd.com");

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var accAfter = await context.Users
                .Include(x => x.Claims)
                .FirstAsync(x => x.Id == User.Id);

            Assert.Equal(command.Email, accAfter.Email);
            Assert.Equal(command.FullName, accAfter.FullName);
        }
    }
}