using Veveve.Api.Domain.Commands.Accounts;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Accounts;

public class DeleteAccountTests : TestBase
{
    private IRequestHandler<DeleteAccount.Command> _sut;

    public DeleteAccountTests()
    {
        _sut = new DeleteAccount.Handler(new AppDbContext(_dbOptions));
    }

    [Fact]
    public async void DeleteAccount_ThrowsNotFoundException_WhenIdDoesntExist()
    {
        // Arrange
        var command = new DeleteAccount.Command(123);

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public async void DeleteAccount_DeletesAccount()
    {
        // Arrange
        AccountEntity account;
        using (var context = new AppDbContext(_dbOptions))
        {
            account = new AccountEntity("asdasd", "jkh214h21@mail.com");
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }
        var command = new DeleteAccount.Command(account.Id);

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var accAfter = await context.Accounts
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == account.Id);

            Assert.Null(accAfter);
        }
    }
}