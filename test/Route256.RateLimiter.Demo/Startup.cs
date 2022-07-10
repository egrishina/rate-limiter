using Route256.RateLimiter.Demo.Storage;
using Route256.RateLimiter.Options;

namespace Route256.RateLimiter.Demo;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<RateLimitOptions>(_configuration.GetSection(RateLimitOptions.Name));
        services.Configure<RateLimitPolicies>(_configuration.GetSection(RateLimitPolicies.Name));

        services.AddSingleton<IProductCatalogStorage, ProductCatalogStorage>();
        services.AddInMemoryRateLimiting();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseIpRateLimiting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/",
                async context => { await context.Response.WriteAsync("Hello World!"); });
            endpoints.MapControllers();
        });
    }
}