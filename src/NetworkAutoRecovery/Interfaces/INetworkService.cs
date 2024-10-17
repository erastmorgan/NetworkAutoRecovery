using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkAutoRecovery.Interfaces
{
    public interface INetworkService
    {
        /// <summary>
        /// Reconnect first found network if the speed less than <paramref name="recconnectNetworkSpeedMbps"/> parameter.
        /// </summary>
        /// <param name="networkNamePattern">The pattern of network name to search.</param>
        /// <param name="recconnectNetworkSpeedMbps">The network speed, less than which the connection will reconnect.</param>
        /// <returns>The true, when successfully reconnected.</returns>
        Task<bool> ReconnectNetwork(string networkNamePattern, int recconnectNetworkSpeedMbps);
    }
}
