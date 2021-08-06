using System;
using Kunicardus.Core.Plugins.Connectivity;

namespace Kunicardus.Touch.Plugins.Connectivity
{
	public class TouchConnectivityPlugin : IConnectivityPlugin
	{
		#region INetworkService

		public bool IsNetworkReachable {
			get { return IsDataReachable || IsWifiReachable; }
		}

		public bool IsDataReachable {
			get {
				var status = Reachability.InternetConnectionStatus ();
				return status == NetworkStatus.ReachableViaCarrierDataNetwork;
			}
		}

		public bool IsWifiReachable {
			get {
				var status = Reachability.InternetConnectionStatus ();
				return status == NetworkStatus.ReachableViaWiFiNetwork;
			}
		}

		#endregion

	}
}

