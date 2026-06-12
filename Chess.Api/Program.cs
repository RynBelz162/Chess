using Chess.Api.Services;
using Chess.Api.Hubs;
using Serilog;
using Orleans.Serialization;
using Scalar.AspNetCore;
using StackExchange.Redis;

await Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, lc) => lc.MinimumLevel.Warning().WriteTo.Console())
    .UseOrleans((ctx, siloBuilder) =>
    {
        var redis = ctx.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis connection string is not configured.");
        Console.WriteLine($"---> using redis connection string: {redis}");

        siloBuilder
            .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
            .UseRedisClustering(redis)
            .AddRedisGrainStorage(
                "chess", 
                configureOptions: options =>
                {
                    options.ConfigurationOptions = new ConfigurationOptions
                    {
                        DefaultDatabase = 1,
                        EndPoints = { redis },
                        AbortOnConnectFail = false
                    };
                })
            .UseSignalR(options =>
            {
                options.UseFireAndForgetDelivery = true;
            })
            .RegisterHub<GameHub>();

            siloBuilder.Services.AddSerializer(serializerBuilder =>
            {
                serializerBuilder.AddJsonSerializer(
                    isSupported: type => type.Namespace!.StartsWith("Chess.Shared.Models"),
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            });
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services => 
        { 
            services.AddControllers(); 
            services.AddSignalR();
            services.AddOpenApi();
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
                
                endpoints.MapOpenApi();
                endpoints.MapScalarApiReference();
            });
        });
    })
    .ConfigureServices(collection =>
    {
        collection.AddTransient<ISetupService, SetupService>();
        collection.AddTransient<IAlgebraicNotationService, AlgebraicNotationService>();
    })
    .RunConsoleAsync();