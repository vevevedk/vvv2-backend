using Veveve.Api.Domain.Commands.Accounts;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Accounts;

public class UpdateAccountIsAdminTests : TestBase
{
    private IRequestHandler<UpdateAccountIsAdmin.Command, AccountEntity> _sut;

    public UpdateAccountIsAdminTests()
    {
        _sut = new UpdateAccountIsAdmin.Handler(new AppDbContext(_dbOptions));
    }

    [Fact]
    public async void UpdateAccountIsAdmin_ThrowsNotFoundException_WhenIdDoesntExist()
    {
        // Arrange
        var command = new UpdateAccountIsAdmin.Command(123, false);

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public async void UpdateAccountIsAdmin_UpdatesAccountWithParams()
    {
        // Arrange
        AccountEntity account;
        using (var context = new AppDbContext(_dbOptions))
        {
            account = new AccountEntity("asdasd", "jkh214h21@mail.com");
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }
        var command = new UpdateAccountIsAdmin.Command(account.Id, true);

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var accAfter = await context.Accounts
                .Include(x => x.Claims)
                .FirstAsync(x => x.Id == account.Id);

            Assert.Equal(command.IsAdmin, accAfter.HasAdminClaim());
        }
    }
}