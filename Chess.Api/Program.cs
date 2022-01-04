using Orleans.Hosting;
using Orleans;
using Microsoft.AspNetCore.ResponseCompression;
using Chess.Api.Hubs;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services => 
        { 
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddControllers(); 
            services.AddSignalR()
                .AddOrleans();
        });

        webBuilder.Configure((ctx, app) =>
        {
            app.UseResponseCompression();

            if (ctx.HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/game");
            });
        });
    })
    .UseOrleans((ctx, siloBuilder) =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryGrainStorage("chess");

        siloBuilder.UseSignalR(config =>
        {
            config.UseFireAndForgetDelivery = true;

            config.Configure((siloBuilder, signalRConstants) =>
            {
                siloBuilder.AddMemoryGrainStorage(signalRConstants.StorageProvider);
                siloBuilder.AddMemoryGrainStorage(signalRConstants.PubSubProvider /*Same as "PubSubStore"*/);
            });
        })
        .RegisterHub<GameHub>();
    })
    .RunConsoleAsync();