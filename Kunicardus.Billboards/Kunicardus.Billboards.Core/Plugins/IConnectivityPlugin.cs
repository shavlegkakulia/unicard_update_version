using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;








namespace Kunicardus.Billboards.Core.Plugins
{
    /// <summary>
    /// Provides basic connectitivy information from device
    /// </summary>
    public interface IConnectivityPlugin
    {
        /// <summary>
        /// returns true if the internet is reachable
        /// </summary>
        bool IsNetworkReachable { get; }

        /// <summary>
        /// Returns true if wireless data is reachable
        /// </summary>
        bool IsDataReachable { get; }

        /// <summary>
        /// returns true if wifi is enabled and working on the device
        /// </summary>
        bool IsWifiReachable { get; }
    }
}