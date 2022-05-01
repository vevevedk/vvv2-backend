using System;
using System.Threading;
using Veveve.Api.Domain.Commands.Users;
using Veveve.Api.Domain.Commands.Emails;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Users;

public class CreateUserTests : TestBase
{
    private readonly IMediator _mockMediatr;
    private IRequestHandler<CreateUser.Command, UserEntity> _sut;

    public CreateUserTests()
    {
        _mockMediatr = Substitute.For<IMediator>();
        _sut = new CreateUser.Handler(new AppDbContext(_dbOptions), _mockMediatr);
    }

    [Fact]
    public async void CreateUser_CreatesUserWithParams()
    {
        // Arrange
        var command = new CreateUser.Command("Donald Trump", "jhlk@asd.com", false);

        // Act
        var User = await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var createdUser = await context.Users
                .Include(x => x.Claims)
                .FirstAsync(x => x.Id == User.Id);

            Assert.Equal(command.Email, createdUser.Email);
            Assert.Equal(command.FullName, createdUser.FullName);
            Assert.Equal(command.Email, createdUser.Email);
            Assert.NotNull(createdUser.ResetPasswordToken);
        }
    }

    [Fact]
    public async void CreateUser_InvokeMediatorSendResetPasswordMail()
    {
        // Arrange
        var command = new CreateUser.Command("Donald Trump", "fhgddfgh@asd.com", false);

        // Act
        var User = await _sut.Handle(command, default);

        // Assert
        await _mockMediatr.Received(1).Send(Arg.Any<SendResetPasswordMail.Command>(), Arg.Any<CancellationToken>());
    }
}