using Microsoft.WindowsAPICodePack.Net;
using NetworkAutoRecovery.Interfaces;
using NetworkAutoRecovery.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetworkAutoRecovery
{
    public class NetworkService : INetworkService
    {
        #region private fields and constructors.

        private readonly ILogger<NetworkService> _logger;

        public NetworkService(ILogger<NetworkService> logger) 
        { 
            _logger = logger;
        }

        #endregion

        #region public methods.

        public async Task<bool> ReconnectNetwork(string networkNamePattern, int recconnectNetworkSpeedMbps)
        {
            var networkInterface = SearchForNetworkByNetworkName(networkNamePattern);

            LogInfo(networkInterface.ToString());

            if (networkInterface != null
                && networkInterface.SpeedMbps < recconnectNetworkSpeedMbps)
            {
                if (await DisableAdapter(networkInterface.Name, 10000))
                {
                    return await EnableAdapter(networkInterface.Name, 10000);
                }
            }

            return false;
        }

        #endregion

        #region private methods.

        private NetworkInterfaceInfo SearchForNetworkByNetworkName(string networkNamePattern)
        {
            return SearchForNetwork((networkInterface, network) => 
                network != null && Regex.IsMatch(network.Name, networkNamePattern));
        }

        private NetworkInterfaceInfo SearchForNetworkByInterfaceName(string interfaceNamePattern)
        {
            return SearchForNetwork((networkInterface, network) =>
                Regex.IsMatch(networkInterface.Name, interfaceNamePattern));
        }

        private NetworkInterfaceInfo SearchForNetwork(Func<NetworkInterface, Network, bool> searchForNetwork)
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            var networks = NetworkListManager.GetNetworks(NetworkConnectivityLevels.All);

            foreach (var networkInterface in networkInterfaces)
            {
                var network = networks.FirstOrDefault(network =>
                    network.Connections.Any(connection =>
                        connection.AdapterId == Guid.Parse(networkInterface.Id)));

                if (!searchForNetwork(networkInterface, network))
                {
                    continue;
                }

                return new NetworkInterfaceInfo(network, networkInterface);
            }

            return null;
        }

        private Task<bool> EnableAdapter(string interfaceName, int timeoutMs = 2000)
        {
            bool enabled = false;
            int timeElapsed = 0;
            int timeWait = 100;
            var locker = new object();

            return Task.Run(async () =>
            {
                ExecuteWaitProcess("netsh", "interface set interface \"" + interfaceName + "\" enable");

                do
                {
                    var networkInterface = SearchForNetworkByInterfaceName($"^{interfaceName}$");

                    lock (locker)
                    {
                        enabled = networkInterface != null && networkInterface.Status == OperationalStatus.Up;
                    }

                    await Task.Delay(timeWait);

                    lock (locker)
                    {
                        timeElapsed += timeWait;
                    }

                } while (!enabled && timeElapsed < timeoutMs);
                return enabled;
            });
        }

        private Task<bool> DisableAdapter(string interfaceName, int timeoutMs = 2000)
        {
            bool disabled = false;
            int timeElapsed = 0;
            int timeWait = 100;
            var locker = new object();


            return Task.Run(async () =>
            {
                ExecuteWaitProcess("netsh", "interface set interface \"" + interfaceName + "\" disable");

                do
                {
                    var networkInterface = SearchForNetworkByInterfaceName($"^{interfaceName}$");

                    lock (locker)
                    {
                        disabled = networkInterface == null || networkInterface.Status != OperationalStatus.Up;
                    }

                    await Task.Delay(timeWait);

                    lock (locker)
                    {
                        timeElapsed += timeWait;
                    }

                } while (!disabled && timeElapsed < timeoutMs);
                return disabled;
            });
        }

        private static void ExecuteWaitProcess(string cmd, string args)
        {

            ProcessStartInfo psi = new ProcessStartInfo(cmd, args);
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
        }

        private void LogInfo(string message)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message);
            }
        }

        #endregion
    }
}
