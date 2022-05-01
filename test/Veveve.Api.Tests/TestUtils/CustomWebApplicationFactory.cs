using System;
using System.Net.Http;
using Veveve.Api.Tests.TestUtils.Stubs;
using Veveve.Api.Infrastructure.Database.Entities;
using Veveve.Api.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using Veveve.Api.Infrastructure.Database.Entities.Builders;

namespace Veveve.Api.Tests.TestUtils;
/// <summary>
/// Used to create a fake instance of the API.
/// This is done during testing.
/// </summary>
public class CustomWebApplicationFactory<TStartup>
: WebApplicationFactory<TStartup> where TStartup : class
{
    public CustomWebApplicationFactory()
    {
    }

    public string GenerateJwtToken(UserEntity? User = null)
    {
        // will not be written to db. this is just a fake User object to create a token.
        var acc = new UserBuilder("Donald Trump", "donaltrump@gmail.com")
            .WithClaim(new UserClaimEntity(ClaimTypeEnum.Admin));
        var jwtTokenHelper = Services.CreateScope().ServiceProvider.GetService<IJwtTokenHelper>()!;
        var token = jwtTokenHelper.GenerateJwtToken(acc);
        return token;
    }

    public IServiceProvider GetScopedServiceProvider() => Services.CreateScope().ServiceProvider;

    public HttpClient CreateNewHttpClient(bool withBearer = false)
    {
        var httpClient = Server.CreateClient();
        if (withBearer)
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {GenerateJwtToken()}");

        return httpClient;
    }

    public async Task<ClientEntity> GetTestClientEntity()
    {
        ClientEntity client;
        using (var appDbContext = GetScopedServiceProvider().GetService<AppDbContext>()!)
        {
            client = new ClientBuilder("testclient");
            appDbContext.Clients.Add(client);
            await appDbContext.SaveChangesAsync();
        }

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Remove the app's ApplicationDbContext registration which happened during Startup.cs. Override it for testing.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if(descriptor != null)
                services.Remove(descriptor);

            var dbName = Guid.NewGuid().ToString();
                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                    options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)); // to avoid transaction errors
                    });

            services.RemoveAll(typeof(ISendGridClientFacade));
            services.AddScoped<ISendGridClientFacade, STUB_SendGridClientFacade>();
        });
    }
}