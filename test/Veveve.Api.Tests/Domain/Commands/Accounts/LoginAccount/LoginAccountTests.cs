using Veveve.Api.Domain.Commands.Accounts;
using Veveve.Api.Domain.Exceptions;
using Veveve.Api.Domain.Services;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Infrastructure.ErrorHandling;
using MediatR;
using NSubstitute;
using Xunit;

namespace Veveve.Api.Tests.Domain.Commands.Accounts;

public class LoginAccountTests : TestBase
{
    private readonly PasswordService _passwordService;
    private readonly IJwtTokenHelper _mockJwtTokenHelper;
    private IRequestHandler<LoginAccount.Command, LoginAccountResult> _sut;

    public LoginAccountTests()
    {
        _passwordService = new PasswordService();
        _mockJwtTokenHelper = Substitute.For<IJwtTokenHelper>();
        _sut = new LoginAccount.Handler(new AppDbContext(_dbOptions), _passwordService, _mockJwtTokenHelper);
    }

    [Fact]
    public async void LoginAccount_ThrowsBusinessRuleException_WhenEmailDoesntExist()
    {
        // Arrange
        var command = new LoginAccount.Command("asdasd@gmail.com", "somepass");

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<BusinessRuleException>(exception);
    }

    [Fact]
    public async void LoginAccount_ThrowsBusinessRuleException_WhenAccountHasNoPassword()
    {
        // Arrange
        var command = new LoginAccount.Command("asdasd@gmail.com", "somepass");
        using (var context = new AppDbContext(_dbOptions))
        {
            context.Accounts.Add(new AccountEntity("some name", command.Email));
            await context.SaveChangesAsync();
        }

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<BusinessRuleException>(exception);
        Assert.Equal(ErrorCodesEnum.ACCOUNT_LOGIN_EMAIL_OR_PASSWORD_INVALID, ((BusinessRuleException)exception).ErrorCode);
    }

    [Fact]
    public async void LoginAccount_ThrowsBusinessRuleException_WhenPasswordIsInvalid()
    {
        // Arrange
        var command = new LoginAccount.Command("asdasd@gmail.com", "somepass");
        using (var context = new AppDbContext(_dbOptions))
        {
            var pw = _passwordService.EncryptPassword("someotherpass");
            context.Accounts.Add(new AccountEntity("some name", command.Email){
                Password = pw.Hash,
                Salt = pw.Salt
            });
            await context.SaveChangesAsync();
        }

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<BusinessRuleException>(exception);
        Assert.Equal(ErrorCodesEnum.ACCOUNT_LOGIN_EMAIL_OR_PASSWORD_INVALID, ((BusinessRuleException)exception).ErrorCode);
    }

    [Fact]
    public async void LoginAccount_ReturnsJwtToken_WhenSuccesful()
    {
        // Arrange
        var command = new LoginAccount.Command("asdasd@gmail.com", "somepass");
        using (var context = new AppDbContext(_dbOptions))
        {
            var pw = _passwordService.EncryptPassword(command.Password);
            context.Accounts.Add(new AccountEntity("some name", command.Email){
                Password = pw.Hash,
                Salt = pw.Salt
            });
            await context.SaveChangesAsync();
        }

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.NotNull(result.JwtToken);
    }
}