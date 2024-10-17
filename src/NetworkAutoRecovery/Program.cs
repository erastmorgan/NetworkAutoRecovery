using Microsoft.Extensions.DependencyInjection;
using NetworkAutoRecovery;
using NetworkAutoRecovery.Configuration;
using NetworkAutoRecovery.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Network Afto Recovery Service";
});

var qwe = configuration.GetSection("NetworkRecovery");
builder.Services.Configure<NetworkRecoveryConfiguration>(configuration.GetSection("NetworkRecovery"));
builder.Services.AddSingleton<INetworkService, NetworkService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
