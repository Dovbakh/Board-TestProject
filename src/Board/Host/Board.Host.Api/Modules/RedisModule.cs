namespace Board.Host.Api.Modules
{
    public static class RedisModule
    {
        public static IServiceCollection AddRedisModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetSection("RedisCache").GetRequiredSection("Host").Value;
                options.InstanceName = configuration.GetSection("RedisCache").GetRequiredSection("InstanceName").Value;
            });

            return services;
        }
    }
}
