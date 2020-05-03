using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.MergeExtension.SampleConsole.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace EntityFrameworkCore.MergeExtension.SampleConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set DB Connection String here
            var dbConnectionString = "";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();
            var loggerFactory = new LoggerFactory(new[] { new SerilogLoggerProvider() });

            var dbOptionsBuilder = new DbContextOptionsBuilder<SampleDbContext>();
                dbOptionsBuilder
                    .UseLoggerFactory(loggerFactory)
                    .UseSqlServer(dbConnectionString);
            var dbOptions = dbOptionsBuilder.Options;

            await SeedSourceSchema(dbOptions);

            using (var ctx = new SourceSampleDbContext(dbOptions))
            {
                await ctx.MergeIntoAsync<SampleItem>("[target].[SampleItems]");
            }

            // ... or ...

            using (var ctx = new TargetSampleDbContext(dbOptions))
            {
                await ctx.MergeFromAsync<SampleItem>("[source].[SampleItems]");
            }

            Log.CloseAndFlush();
        }

        private static async Task SeedSourceSchema(DbContextOptions<SampleDbContext> dbOptions)
        {
            var timestamp = new DateTime(2020, 1, 1);
            using (var ctx = new SourceSampleDbContext(dbOptions))
            {
                if (await ctx.SampleItems.AnyAsync())
                {
                    await ctx.TruncateAsync<SampleItem>();
                }

                ctx.SampleItems.Add(new SampleItem
                {
                    Id = 1,
                    Description = "Item 1",
                    Timestamp = timestamp
                });
                ctx.SampleItems.Add(new SampleItem
                {
                    Id = 2,
                    Description = "Item 2",
                    Timestamp = timestamp.AddDays(-1)
                });
                ctx.SampleItems.Add(new SampleItem
                {
                    Id = 3,
                    Description = "Item 3",
                    Timestamp = timestamp.AddDays(1)
                });
                await ctx.SaveChangesAsync();
                Log.Information("[source].[SampleItems] seeded");
            }
        }
    }
}
