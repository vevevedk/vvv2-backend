using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Veveve.Api.Tests.TestUtils;
using System;
using Veveve.Api.Controllers.Users;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Services;
using Veveve.Api.Infrastructure.Database.Entities.Builders;

namespace Veveve.Api.Tests.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UsersControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateUser_Returns201()
    {
        // Arrange
        ClientEntity client = await _factory.GetTestClientEntity();

        var email = "try54y4tre@asd.com";
        // Act
        var httpClient = _factory.CreateNewHttpClient(true);
        var response = await httpClient.PostAsync($"/api/v1/Users",
            new CreateUserRequest
            {
                ClientId = client.Id,
                Email = email,
                FullName = "donald trump",
                IsAdmin = false
            }.ToHttpStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseObj = await response.DeserializeHttpResponse<UserResponse>();
        Assert.Equal(email, responseObj?.Email);
    }

    [Fact]
    public async Task UpdateUserPassword_Returns204()
    {
        // Arrange
        UserEntity User;
        var resetPwToken = Guid.NewGuid();
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            User = new UserBuilder("donald trump", "fdgdfgdfs@asd.com")
                .WithTestClient()
                .WithResetPasswordToken(resetPwToken);

            appDbContext.Users.Add(User);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(false);
        var response = await httpClient.PutAsync($"/api/v1/Users/updatePassword",
            new UpdateUserPasswordRequest
            {
                Password = "somenewpassword",
                ResetPasswordToken = resetPwToken
            }.ToHttpStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task LoginUser_Returns200()
    {
        // Arrange
        UserEntity User;
        var password = "somepassword";
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            var pwService = _factory.GetScopedServiceProvider().GetService<IPasswordService>()!;
            var pw = pwService.EncryptPassword(password);
            User = new UserBuilder("donald trump", "6345dfsg@asd.com")
                .WithTestClient()
                .WithPassword(pw.Hash, pw.Salt);

            appDbContext.Users.Add(User);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(false);
        var response = await httpClient.PostAsync($"/api/v1/Users/login",
            new LoginUserRequest
            {
                Email = User.Email,
                Password = password
            }.ToHttpStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseObj = await response.DeserializeHttpResponse<LoginResponse>();
        Assert.NotNull(responseObj?.Jwt);
    }

    [Fact]
    public async Task ResetUserPassword_Returns204()
    {
        // Arrange
        UserEntity User;
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            User = new UserBuilder("donald trump", "dfsgsfdgdf@asd.com")
                .WithTestClient();
            appDbContext.Users.Add(User);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(false);
        var response = await httpClient.PostAsync($"/api/v1/Users/resetPassword",
            new ResetUserPasswordRequest
            {
                Email = User.Email
            }.ToHttpStringContent());


        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_Returns200()
    {
        // Arrange
        UserEntity User;
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            User = new UserBuilder("donald trump", "324141324@asd.com")
                .WithTestClient();
            appDbContext.Users.Add(User);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(true);
        var response = await httpClient.PutAsync($"/api/v1/Users/{User.Id}",
            new UpdateUserRequest
            {
                Email = "somenewemail@asd.com",
                FullName = "kim jong un"
            }.ToHttpStringContent());


        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseObj = await response.DeserializeHttpResponse<UserResponse>();
        Assert.Equal(User.Id, responseObj?.Id);
    }

    [Fact]
    public async Task DeleteUser_Returns204()
    {
        // Arrange
        UserEntity User;
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            User = new UserBuilder("donald trump", "sdfgsdfgdsf@asd.com")
                .WithTestClient();
            appDbContext.Users.Add(User);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(true);
        var response = await httpClient.DeleteAsync($"/api/v1/Users/{User.Id}/");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}