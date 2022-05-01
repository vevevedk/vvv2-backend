# REST API Veveve
## About this project

This is the REST API behind the Veveve app

Hosted at:

- https://dev-mywebsite.azurewebsites.net
- https://test-mywebsite.azurewebsites.net
- https://mywebsite.azurewebsites.net

**Highlights:**

- .NET 6
- CQRS architecture and mediator pattern
- Errorhandling with Exception middleware and error codes
- Serilog with minimal request-logging
- Swagger UI at `/index.html`.
- Entity Framework and the code-first principle with migrations.
- User API with login, reset password, update password etc.
- Bearer-token Authorization with policies
- SendGrid integration for sending emails (including en email log and SendGrid webhooks for email delivery status updates)
- Test-project with a few example tests

### Prerequisites
- .Net SDK 6
- SendGrid account
- Database connection (SQL Server or LocalDb)

### Configuration
Configurations are contained within the appsettings.json file.
Depending on the `ASPNETCORE_ENVIRONMENT` environment variable, an additional appsettings.{env}.json file will be read, which contains additional environment-specific configuration.

These files should never contain secrets as they will be committed to github.

Instead, set secret values as environment variables when running the project.
Using VS Code, a gitignored launch.json file can have a configuration containing an env object as such:
```
"env": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "ConnectionStrings:DbConnection": "{myConnectionString}",
    "SendGrid:ApiKey": "{myApiKey}";
    "Authorization:JwtKey": "{myJwtKey}"
}
```

The above env variables are required to run the project as of writing this.
It is also required to configure the SendGrid section in appsettings.json with template ids and stuff.

### Entity Framework
Migrations are applied when the application starts and should generally not be applied manually using the command line.
This means that upon deploying the app, migrations will automatically update the database when the newly deployed app starts.

While you are working with a new database scheme you should **not** connect to the Test or Production database,
because you might accidentally apply migrations to the database.
It should instead point at a local database running on your machine.

I use the following connection string to point at a locally running LocalDb, which can be installed as part of the SQL Server Express installation. `Server=(localdb)\\MSSQLLocalDB;Initial Catalog=Veveve-local;Integrated Security=true"`
Connect to the local database in SSMS using server name: (LocalDB)\MSSQLLocalDB.

Install the EF global tool:
```sh
dotnet tool install --global dotnet-ef
```

To add new migrations

```sh
cd ./src/Veveve.Api/
dotnet ef migrations add MyMigrationName -o ./Infrastructure/Database/Migrations
```

### Run the app

Using dotnet command-line

```sh
dotnet run --project ./src/Veveve.Api/
```

### Tests

We're using Xunit and NSubstitute.
The general strategy for testing is the following:

- Unit test all business logic
- Integration test all database integrations with an in-memory database
- Mock/Stub external services. No integrations with external services are tested.
- Acceptance test all endpoints in the API using WebApplicationFactory.

To run tests use a test explorer in the code editor or run the command: `dotnet test`.
The same command should be run as part of the deploy pipeline. Nothing should be deployed if any test fails.

### Deployment & hosting

The application is currently hosted at Simply.com.
We use github actions as CI/CD tool. Upon pushing to the Dev branch, any changes will be deployed to our Test server.
In the csproj file the following applies to the Debug configuration:

```xml
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <AspNetCoreModuleName>AspNetCoreModule</AspNetCoreModuleName>
    <AspNetCoreHostingModel>OutOfProcess</AspNetCoreHostingModel>
</PropertyGroup>
```

This enables running multiples applications in the IIS app-pool on the server.
The main application runs In-process and all others run Out-of-process.
