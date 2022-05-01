using Veveve.Api.Domain.Commands.Accounts;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Accounts;

public class UpdateAccountTests : TestBase
{
    private readonly IMediator _mockMediatr;
    private IRequestHandler<UpdateAccount.Command, AccountEntity> _sut;

    public UpdateAccountTests()
    {
        _mockMediatr = Substitute.For<IMediator>();
        _sut = new UpdateAccount.Handler(new AppDbContext(_dbOptions), _mockMediatr);
    }

    [Fact]
    public async void UpdateAccount_ThrowsNotFoundException_WhenIdDoesntExist()
    {
        // Arrange
        var command = new UpdateAccount.Command(123, "Donald Trump", "fhgddfgh@asd.com");

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public async void UpdateAccount_UpdatesAccountWithParams()
    {
        // Arrange
        AccountEntity account;
        using (var context = new AppDbContext(_dbOptions))
        {
            account = new AccountEntity("asdasd", "jkh214h21@mail.com");
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }
        var command = new UpdateAccount.Command(account.Id, "Donald Trump", "jhlk@asd.com");

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var accAfter = await context.Accounts
                .Include(x => x.Claims)
                .FirstAsync(x => x.Id == account.Id);

            Assert.Equal(command.Email, accAfter.Email);
            Assert.Equal(command.FullName, accAfter.FullName);
        }
    }
}