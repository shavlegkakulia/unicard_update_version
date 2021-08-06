using System;
using Kunicardus.Billboards.Core.Plugins;
using Connectivity.Plugin;

namespace iCunOS.BillBoards.Plugins.Connectivity
{
	public class TouchConnectivityPlugin : IConnectivityPlugin
	{
		#region INetworkService

		public bool IsNetworkReachable {
			get { 
				return CrossConnectivity.Current.IsConnected;
			}
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

