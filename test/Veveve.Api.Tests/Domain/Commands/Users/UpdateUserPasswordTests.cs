using Veveve.Domain.Database;
using Veveve.Domain.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;
using Veveve.Domain.Database.Entities.Builders;
using Veveve.Domain.Services;
using Veveve.Domain.Commands.Users;
using Veveve.Domain.Exceptions;

namespace Veveve.Api.Tests.Domain.Commands.Users;

public class UpdateUserPasswordTests : TestBase
{
    private readonly IPasswordService _mockPasswordService;
    private IRequestHandler<UpdateUserPassword.Command> _sut;

    public UpdateUserPasswordTests()
    {
        _mockPasswordService = Substitute.For<IPasswordService>();
        _sut = new UpdateUserPassword.Handler(new AppDbContext(_dbOptions), _mockPasswordService);
    }

    [Fact]
    public async void UpdateUserPassword_ThrowsBusinessRuleException_WhenPasswordTokenDoesntExist()
    {
        // Arrange
        var pwToken = Guid.NewGuid();
        var command = new UpdateUserPassword.Command(pwToken, "newpassword");

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<BusinessRuleException>(exception);
    }

    [Fact]
    public async void UpdateUserPassword_UpdatesUserWithParams()
    {
        // Arrange
        var pwToken = Guid.NewGuid();
        var password = "somepassword";
        UserEntity User;
        using (var context = new AppDbContext(_dbOptions))
        {
            User = new UserBuilder("asdasd", "jkh214h21@mail.com")
                .WithTestClient()
                .WithClaim(ClaimTypeEnum.User)
                .WithResetPasswordToken(pwToken);
            context.Users.Add(User);
            await context.SaveChangesAsync();
        }

        var pwDto = new PasswordDto("somehash", new byte[] { 0x20, 0x20 });
        _mockPasswordService.EncryptPassword(password).Returns(pwDto);

        var command = new UpdateUserPassword.Command(pwToken, password);

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var accAfter = await context.Users.FirstAsync(x => x.Id == User.Id);

            Assert.Equal(pwDto.Hash, accAfter.Password);
            Assert.Equal(pwDto.Salt, accAfter.Salt);
            Assert.Null(accAfter.ResetPasswordToken);
        }
    }
}