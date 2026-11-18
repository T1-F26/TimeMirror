
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TimeMirror.Infrastructure.Data;

public class TimeMirrorDbContextFactory
    : IDesignTimeDbContextFactory<TimeMirrorDbContext>
{
    public TimeMirrorDbContext CreateDbContext(string[] args)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(
                "TIMEMIRROR_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "TIMEMIRROR_CONNECTION_STRING is not configured.");

        var options = new DbContextOptionsBuilder<TimeMirrorDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new TimeMirrorDbContext(options);
    }
}
