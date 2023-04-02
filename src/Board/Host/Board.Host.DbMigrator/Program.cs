using Board.Host.DbMigrator;
using System.Runtime.CompilerServices;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
{
    services.AddServices(hostContext.Configuration);
}).Build();

await DbMigrator.MigrateDatabaseAsync(host.Services);
await host.RunAsync();


