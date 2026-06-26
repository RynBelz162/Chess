using System;
using Chess.Api.Services;
using Chess.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.Serialization;
using Orleans.TestingHost;

namespace Chess.Api.Tests.Grains;

public sealed class ClusterFixture : IDisposable
{
    public TestCluster Cluster { get; }

    public ClusterFixture()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
        builder.AddClientBuilderConfigurator<TestClientConfigurator>();
        Cluster = builder.Build();
        Cluster.Deploy();
    }

    public void Dispose() => Cluster.StopAllSilos();

    private sealed class TestSiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.AddMemoryGrainStorage("chess");

            siloBuilder.Services.AddSingleton(CreateSetupServiceMock());
            siloBuilder.Services.AddTransient<IAlgebraicNotationService, AlgebraicNotationService>();

            siloBuilder.Services.AddChessJsonSerializer();
        }

        // PlayerOne (game creator) is always assigned White, so tests can rely on a
        // fixed color/turn order instead of recomputing which player got which color.
        private static ISetupService CreateSetupServiceMock()
        {
            var real = new SetupService();
            var mock = new Mock<ISetupService>();

            mock.Setup(s => s.DeterminePlayerColor())
                .Returns(ChessColor.White);
            mock.Setup(s => s.GetOppositeColor(It.IsAny<ChessColor>()))
                .Returns((ChessColor color) => real.GetOppositeColor(color));
            mock.Setup(s => s.InitializeBoard())
                .Returns(() => real.InitializeBoard());

            return mock.Object;
        }
    }

    private sealed class TestClientConfigurator : IClientBuilderConfigurator
    {
        public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
        {
            clientBuilder.Services.AddChessJsonSerializer();
        }
    }
}

internal static class SerializerConfiguration
{
    public static void AddChessJsonSerializer(this IServiceCollection services)
    {
        services.AddSerializer(serializerBuilder =>
        {
            serializerBuilder.AddJsonSerializer(
                isSupported: type => type.Namespace!.StartsWith("Chess.Shared.Models"),
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        });
    }
}

[CollectionDefinition(Name)]
public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>
{
    public const string Name = "ClusterCollection";
}
