using Microsoft.Extensions.Options;
using Microsoft.WindowsAPICodePack.Net;
using NetworkAutoRecovery.Configuration;
using NetworkAutoRecovery.Interfaces;
using NetworkAutoRecovery.Models;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace NetworkAutoRecovery
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly NetworkRecoveryConfiguration _networkRecoveryConfig;
        private readonly INetworkService _networkService;

        public Worker(
            ILogger<Worker> logger,
            IOptions<NetworkRecoveryConfiguration> networkRecoveryConfig,
            INetworkService networkService)
        {
            _logger = logger;
            _networkRecoveryConfig = networkRecoveryConfig.Value;
            _networkService = networkService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var isSuccessReconnect = await _networkService.ReconnectNetwork(
                    _networkRecoveryConfig.NetworkNamePattern, _networkRecoveryConfig.RecconnectNetworkSpeedMbps);
                
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    var reconnectStatus = isSuccessReconnect ? "success" : "-";

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    _logger.LogInformation($"Reconnect status: {reconnectStatus}");
                }

                await Task.Delay(_networkRecoveryConfig.RefreshPeriodMinutes * 1000 * 60, stoppingToken);
            }
        }
    }
}
