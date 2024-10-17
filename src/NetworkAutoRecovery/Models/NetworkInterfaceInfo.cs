using Microsoft.WindowsAPICodePack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkAutoRecovery.Models
{
    internal class NetworkInterfaceInfo
    {
        private readonly double _speedMbps;

        public NetworkInterfaceInfo(Network network, NetworkInterface networkInterface)
        {
            Name = networkInterface.Name;
            Status = networkInterface.OperationalStatus;
            NetworkName = network?.Name;

            _speedMbps = networkInterface.Speed / 1e+6;
            SpeedMbps = (int)(_speedMbps);
        }

        public string Name { get; }

        public OperationalStatus Status { get; }

        public string NetworkName { get; }

        public int SpeedMbps { get; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Interface: " + Name);
            builder.Append(", Status: " + (Status == OperationalStatus.Up ? "enabled" : "disabled"));
            builder.Append(", Network: " + NetworkName);
            builder.Append(", Speed: " + (_speedMbps < 0 ? 0 : Math.Round(_speedMbps, 2)) + " Mbps");

            return builder.ToString();
        }
    }
}
