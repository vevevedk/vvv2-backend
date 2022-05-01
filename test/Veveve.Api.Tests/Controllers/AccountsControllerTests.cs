using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Veveve.Api.Tests.TestUtils;
using System;
using Veveve.Api.Controllers.Accounts;
using Veveve.Api.Infrastructure.Database;
using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Services;

namespace Veveve.Api.Tests.Controllers;

public class AccountsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AccountsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAccount_Returns201()
    {
        // Arrange
        var email = "try54y4tre@asd.com";
        // Act
        var httpClient = _factory.CreateNewHttpClient(true);
        var response = await httpClient.PostAsync($"/api/v1/accounts",
            new CreateAccountRequest
            {
                Email = email,
                FullName = "donald trump",
                IsAdmin = false
            }.ToHttpStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseObj = await response.DeserializeHttpResponse<AccountResponse>();
        Assert.Equal(email, responseObj?.Email);
    }

    [Fact]
    public async Task UpdateAccountPassword_Returns204()
    {
        // Arrange
        AccountEntity account;
        var resetPwToken = Guid.NewGuid();
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            account = new AccountEntity("donald trump", "fdgdfgdfs@asd.com"){
                ResetPasswordToken = resetPwToken
            };

            appDbContext.Accounts.Add(account);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(false);
        var response = await httpClient.PutAsync($"/api/v1/accounts/updatePassword",
            new UpdateAccountPasswordRequest
            {
                Password = "somenewpassword",
                ResetPasswordToken = resetPwToken
            }.ToHttpStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task LoginAccount_Returns200()
    {
        // Arrange
        AccountEntity account;
        var password = "somepassword";
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            var pwService = _factory.GetScopedServiceProvider().GetService<IPasswordService>()!;
            var pw = pwService.EncryptPassword(password);
            account = new AccountEntity("donald trump", "6345dfsg@asd.com"){
                Password = pw.Hash,
                Salt = pw.Salt
            };

            appDbContext.Accounts.Add(account);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(false);
        var response = await httpClient.PostAsync($"/api/v1/accounts/login",
            new LoginAccountRequest
            {
                Email = account.Email,
                Password = password
            }.ToHttpStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseObj = await response.DeserializeHttpResponse<LoginResponse>();
        Assert.NotNull(responseObj?.Jwt);
    }

    [Fact]
    public async Task ResetAccountPassword_Returns204()
    {
        // Arrange
        AccountEntity account;
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            account = new AccountEntity("donald trump", "dfsgsfdgdf@asd.com");
            appDbContext.Accounts.Add(account);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(false);
        var response = await httpClient.PostAsync($"/api/v1/accounts/resetPassword",
            new ResetAccountPasswordRequest
            {
                Email = account.Email
            }.ToHttpStringContent());


        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAccount_Returns200()
    {
        // Arrange
        AccountEntity account;
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            account = new AccountEntity("donald trump", "324141324@asd.com");
            appDbContext.Accounts.Add(account);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(true);
        var response = await httpClient.PutAsync($"/api/v1/accounts/{account.Id}",
            new UpdateAccountRequest
            {
                Email = "somenewemail@asd.com",
                FullName = "kim jong un"
            }.ToHttpStringContent());


        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseObj = await response.DeserializeHttpResponse<AccountResponse>();
        Assert.Equal(account.Id, responseObj?.Id);
    }

    [Fact]
    public async Task UpdateAccountIsAdmin_Returns204()
    {
        // Arrange
        AccountEntity account;
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            account = new AccountEntity("donald trump", "sdfgsdfgdsf@asd.com");
            appDbContext.Accounts.Add(account);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(true);
        var response = await httpClient.PutAsync($"/api/v1/accounts/{account.Id}/updateIsAdmin",
            new UpdateAccountIsAdminRequest
            {
                IsAdmin = true
            }.ToHttpStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_Returns204()
    {
        // Arrange
        AccountEntity account;
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            account = new AccountEntity("donald trump", "sdfgsdfgdsf@asd.com");
            appDbContext.Accounts.Add(account);
            await appDbContext.SaveChangesAsync();
        }

        // Act
        var httpClient = _factory.CreateNewHttpClient(true);
        var response = await httpClient.DeleteAsync($"/api/v1/accounts/{account.Id}/");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}