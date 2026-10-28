using AzureTangyFunc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureTangyFunc;

public static class Startup
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AzureTangyDbContext>(options =>
            options.UseSqlServer(configuration["AzureSqlDatabase"]));
    }
}
