using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;
using Veveve.Domain.Database.Entities.Builders;
using Veveve.Domain.Commands.Users;
using Veveve.Domain.Commands.Emails;
using Veveve.Domain.Exceptions;

namespace Veveve.Api.Tests.Domain.Commands.Users;

public class ResetUserPasswordTests : TestBase
{
    private readonly IMediator _mockMediatr;
    private IRequestHandler<ResetUserPassword.Command> _sut;

    public ResetUserPasswordTests()
    {
        _mockMediatr = Substitute.For<IMediator>();
        _sut = new ResetUserPassword.Handler(new AppDbContext(_dbOptions), _mockMediatr);
    }

    [Fact]
    public async void CreateUser_ThrowsNotFoundException_WhenEmailDoesntExist()
    {
        // Arrange
        var email = "dfsgdfg@asd.com";
        var command = new ResetUserPassword.Command(email);

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
        await _mockMediatr.Received(0).Send(Arg.Any<SendResetPasswordMail.Command>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async void ResetUserPassword_CreatesResetPasswordToken()
    {
        // Arrange
        var email = "jhlk@asd.com";
        var command = new ResetUserPassword.Command(email);

        using (var context = new AppDbContext(_dbOptions))
        {
            var newAcc = new UserBuilder("asdasd", email)
                .WithTestClient()
                .WithClaim(ClaimTypeEnum.User)
                .Build();
            context.Users.Add(newAcc);
            await context.SaveChangesAsync();
            Assert.Null(newAcc.ResetPasswordToken);
        }

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var User = await context.Users
                .Include(x => x.Claims)
                .FirstAsync(x => x.Email == email);

            Assert.NotNull(User.ResetPasswordToken);
        }
    }

    [Fact]
    public async void CreateUser_InvokeMediatorSendResetPasswordMail()
    {
        // Arrange
        var email = "dfsgdfg@asd.com";
        var command = new ResetUserPassword.Command(email);

        using (var context = new AppDbContext(_dbOptions))
        {
            var newAcc = new UserBuilder("asdasd", email)
                .WithTestClient()
                .WithClaim(ClaimTypeEnum.User);
            context.Users.Add(newAcc);
            await context.SaveChangesAsync();
        }

        // Act
        await _sut.Handle(command, default);

        // Assert
        await _mockMediatr.Received(1).Send(Arg.Any<SendResetPasswordMail.Command>(), Arg.Any<CancellationToken>());
    }
}