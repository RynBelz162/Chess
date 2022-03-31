using Orleans.Hosting;
using Orleans;
using Chess.Api.Services;
using Orleans.Configuration;
using Chess.Api.Hubs;

await Host.CreateDefaultBuilder(args)
    .UseOrleans((ctx, siloBuilder) =>
    {
        var redisConnectionString = ctx.Configuration.GetConnectionString("Redis");
        Console.WriteLine($"---> using redis connection string: {redisConnectionString}");

        siloBuilder
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "chess-silo";
                options.ServiceId = "ChessApi";
            })
            .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
            .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
            .UseRedisClustering(redisConnectionString)
            .AddRedisGrainStorage("chess", options => options.Configure(opt =>{
                opt.ConnectionString = redisConnectionString;
                opt.UseJson = true;
                opt.DatabaseNumber = 1;
            }));

        siloBuilder.UseSignalR(opt =>
        {
            opt.UseFireAndForgetDelivery = true;
            opt.Configure((builder, constants) => 
            {
                builder
                    .AddMemoryGrainStorage(constants.PubSubProvider)
                    .AddMemoryGrainStorage(constants.StorageProvider);
            });
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
            });
        });
    })
    .ConfigureServices(collection =>
    {
        collection.AddTransient<ISetupService, SetupService>();
    })
    .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning).AddConsole())
    .RunConsoleAsync();