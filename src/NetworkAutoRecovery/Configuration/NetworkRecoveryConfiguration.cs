using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkAutoRecovery.Configuration
{
    public class NetworkRecoveryConfiguration
    {
        /// <summary>
        /// Gets or sets the pattern of network name to search.
        /// </summary>
        public string NetworkNamePattern { get; set; }

        /// <summary>
        /// Gets or sets the network speed, less than which the connection will reconnect.
        /// </summary>
        public int RecconnectNetworkSpeedMbps { get; set; }

        /// <summary>
        /// Gets or sets the refresh period in minutes to check recovery possibility.
        /// </summary>
        public int RefreshPeriodMinutes { get; set; }
    }
}
