using System;
using System.Threading;
using Veveve.Api.Domain.Commands.Accounts;
using Veveve.Api.Domain.Commands.Emails;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Accounts;

public class CreateAccountTests : TestBase
{
    private readonly IMediator _mockMediatr;
    private IRequestHandler<CreateAccount.Command, AccountEntity> _sut;

    public CreateAccountTests()
    {
        _mockMediatr = Substitute.For<IMediator>();
        _sut = new CreateAccount.Handler(new AppDbContext(_dbOptions), _mockMediatr);
    }

    [Fact]
    public async void CreateAccount_CreatesAccountWithParams()
    {
        // Arrange
        var command = new CreateAccount.Command("Donald Trump", "jhlk@asd.com", false);

        // Act
        var account = await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var createdAccount = await context.Accounts
                .Include(x => x.Claims)
                .FirstAsync(x => x.Id == account.Id);

            Assert.Equal(command.Email, createdAccount.Email);
            Assert.Equal(command.FullName, createdAccount.FullName);
            Assert.Equal(command.Email, createdAccount.Email);
            Assert.NotNull(createdAccount.ResetPasswordToken);
        }
    }

    [Fact]
    public async void CreateAccount_InvokeMediatorSendResetPasswordMail()
    {
        // Arrange
        var command = new CreateAccount.Command("Donald Trump", "fhgddfgh@asd.com", false);

        // Act
        var account = await _sut.Handle(command, default);

        // Assert
        await _mockMediatr.Received(1).Send(Arg.Any<SendResetPasswordMail.Command>(), Arg.Any<CancellationToken>());
    }
}