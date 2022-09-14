using System;
using Veveve.Domain.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Api.Tests.Domain.Commands.Users;

public class TestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly DbContextOptions<AppDbContext> _dbOptions;

    public TestBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        using (var context = new AppDbContext(_dbOptions))
        {
            context.Database.EnsureCreated();
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}