using System.Threading;
using Veveve.Api.Domain.Commands.Accounts;
using Veveve.Api.Domain.Commands.Emails;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Accounts;

public class ResetAccountPasswordTests : TestBase
{
    private readonly IMediator _mockMediatr;
    private IRequestHandler<ResetAccountPassword.Command> _sut;

    public ResetAccountPasswordTests()
    {
        _mockMediatr = Substitute.For<IMediator>();
        _sut = new ResetAccountPassword.Handler(new AppDbContext(_dbOptions), _mockMediatr);
    }

    [Fact]
    public async void CreateAccount_ThrowsNotFoundException_WhenEmailDoesntExist()
    {
        // Arrange
        var email = "dfsgdfg@asd.com";
        var command = new ResetAccountPassword.Command(email);

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
        await _mockMediatr.Received(0).Send(Arg.Any<SendResetPasswordMail.Command>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async void ResetAccountPassword_CreatesResetPasswordToken()
    {
        // Arrange
        var email = "jhlk@asd.com";
        var command = new ResetAccountPassword.Command(email);
        
        using (var context = new AppDbContext(_dbOptions))
        {
            var newAcc = new AccountEntity("asdasd", email);
            context.Accounts.Add(newAcc);
            await context.SaveChangesAsync();
            Assert.Null(newAcc.ResetPasswordToken);
        }

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var account = await context.Accounts
                .Include(x => x.Claims)
                .FirstAsync(x => x.Email == email);

            Assert.NotNull(account.ResetPasswordToken);
        }
    }

    [Fact]
    public async void CreateAccount_InvokeMediatorSendResetPasswordMail()
    {
        // Arrange
        var email = "dfsgdfg@asd.com";
        var command = new ResetAccountPassword.Command(email);

        using (var context = new AppDbContext(_dbOptions))
        {
            var newAcc = new AccountEntity("asdasd", email);
            context.Accounts.Add(newAcc);
            await context.SaveChangesAsync();
        }

        // Act
        await _sut.Handle(command, default);

        // Assert
        await _mockMediatr.Received(1).Send(Arg.Any<SendResetPasswordMail.Command>(), Arg.Any<CancellationToken>());
    }
}