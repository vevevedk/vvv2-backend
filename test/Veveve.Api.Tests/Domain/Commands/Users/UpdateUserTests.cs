using Veveve.Api.Domain.Commands.Users;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;
using Veveve.Api.Infrastructure.Database.Entities.Builders;

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
    public async void UpdateUser_ThrowsNotFoundException_WhenClientIdDoesntExist(){
        // Arrange
        var command = new UpdateUser.Command(333, 999, "Donald Trump", $"{Guid.NewGuid()}@gmail.com", null);

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public async void UpdateUser_ThrowsNotFoundException_WhenIdDoesntExist()
    {
        // Arrange
        ClientEntity client;
        using (var context = new AppDbContext(_dbOptions))
        {
            client = new ClientBuilder("some client");
            context.Clients.Add(client);
            await context.SaveChangesAsync();
        }
        var command = new UpdateUser.Command(client.Id, 123, "Donald Trump", "fhgddfgh@asd.com", null);

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
            User = new UserBuilder("asdasd", "jkh214h21@mail.com")
                .WithTestClient()
                .WithClaim(ClaimTypeEnum.User);
            context.Users.Add(User);
            await context.SaveChangesAsync();
        }
        var command = new UpdateUser.Command(User.ClientId, User.Id, "Donald Trump", "jhlk@asd.com", null);

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