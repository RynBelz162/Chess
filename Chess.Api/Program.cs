using Chess.Api.Services;
using Chess.Api.Hubs;
using Serilog;
using System.Net;
using Orleans.Configuration;

await Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, lc) => lc.MinimumLevel.Warning().WriteTo.Console())
    .UseOrleans((ctx, siloBuilder) =>
    {
        var redis = ctx.Configuration.GetConnectionString("Redis");
        Console.WriteLine($"---> using redis connection string: {redis}");

        siloBuilder
            .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
            .UseRedisClustering(redis)
            .AddRedisGrainStorage("chess", options =>
            {
                options.DatabaseNumber = 1;
                options.ConnectionString = redis;
            })
            .UseSignalR(options =>
            {
                options.UseFireAndForgetDelivery = true;
            })
            .RegisterHub<GameHub>();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services => 
        { 
            services.AddControllers(); 
            services.AddSignalR();
        });

        webBuilder.Configure((ctx, app) =>
        {
            if (ctx.HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/game");
                endpoints.MapGet("/", () => "Hello World!");
            });
        });
    })
    .ConfigureServices(collection =>
    {
        collection.AddTransient<ISetupService, SetupService>();
        collection.AddTransient<IAlgebraicNotationService, AlgebraicNotationService>();
    })
    .RunConsoleAsync();