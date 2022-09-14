using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;
using MediatR;
using NSubstitute;
using Xunit;
using Veveve.Domain.Database.Entities.Builders;
using Veveve.Domain.Services;
using Veveve.Domain.Commands.Users;
using Veveve.Domain.Exceptions;
using Veveve.Api.Authorization;

namespace Veveve.Api.Tests.Domain.Commands.Users;

public class LoginUserTests : TestBase
{
    private readonly PasswordService _passwordService;
    private readonly IJwtTokenHelper _mockJwtTokenHelper;
    private IRequestHandler<LoginUser.Command, UserEntity> _sut;

    public LoginUserTests()
    {
        _passwordService = new PasswordService();
        _mockJwtTokenHelper = Substitute.For<IJwtTokenHelper>();
        _sut = new LoginUser.Handler(new AppDbContext(_dbOptions), _passwordService);
    }

    [Fact]
    public async void Loginuser_ThrowsBusinessRuleException_WhenEmailDoesntExist()
    {
        // Arrange
        var command = new LoginUser.Command("asdasd@gmail.com", "somepass");

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<BusinessRuleException>(exception);
    }

    [Fact]
    public async void Loginuser_ThrowsBusinessRuleException_WhenUserHasNoPassword()
    {
        // Arrange
        var command = new LoginUser.Command("asdasd@gmail.com", "somepass");
        using (var context = new AppDbContext(_dbOptions))
        {
            context.Users.Add(new UserBuilder("some name", command.Email)
                .WithTestClient()
                .WithClaim(ClaimTypeEnum.User));
            await context.SaveChangesAsync();
        }

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<BusinessRuleException>(exception);
        Assert.Equal(ErrorCodesEnum.USER_LOGIN_EMAIL_OR_PASSWORD_INVALID, ((BusinessRuleException)exception).ErrorCode);
    }

    [Fact]
    public async void Loginuser_ThrowsBusinessRuleException_WhenPasswordIsInvalid()
    {
        // Arrange
        var command = new LoginUser.Command("asdasd@gmail.com", "somepass");
        using (var context = new AppDbContext(_dbOptions))
        {
            var pw = _passwordService.EncryptPassword("someotherpass");
            context.Users.Add(new UserBuilder("some name", command.Email)
                .WithPassword(pw.Hash, pw.Salt)
                .WithTestClient()
                .WithClaim(ClaimTypeEnum.User));
            await context.SaveChangesAsync();
        }

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<BusinessRuleException>(exception);
        Assert.Equal(ErrorCodesEnum.USER_LOGIN_EMAIL_OR_PASSWORD_INVALID, ((BusinessRuleException)exception).ErrorCode);
    }

    [Fact]
    public async void Loginuser_ReturnsUser_WhenSuccesful()
    {
        // Arrange
        var command = new LoginUser.Command("asdasd@gmail.com", "somepass");
        using (var context = new AppDbContext(_dbOptions))
        {
            var pw = _passwordService.EncryptPassword(command.Password);
            context.Users.Add(new UserBuilder("some name", command.Email)
                .WithPassword(pw.Hash, pw.Salt)
                .WithTestClient()
                .WithClaim(ClaimTypeEnum.User));
            await context.SaveChangesAsync();
        }

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.NotNull(result);
    }
}