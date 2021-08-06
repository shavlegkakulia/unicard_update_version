using Kuni.Core.Plugins.Connectivity;
using Android.Net;
using Android.App;
using Android.Content;
using System.Linq;

namespace Kunicardus.Droid.Plugins.Connectivity
{
	public class DroidConnectivityProviderPlugin : IConnectivityPlugin
	{

		private readonly ConnectivityManager _cm;

		public DroidConnectivityProviderPlugin ()
		{
			_cm = (ConnectivityManager)Application.Context.GetSystemService (Context.ConnectivityService);
		}

		#region IConnectivityPlugin implementation

		public bool IsNetworkReachable {
			get {
				var activeNetwork = _cm.ActiveNetworkInfo;
				return null != activeNetwork && activeNetwork.IsConnectedOrConnecting;
			}
		}

		public bool IsDataReachable {
			get {
				return _cm.GetAllNetworkInfo ().Any (t => t.IsConnectedOrConnecting && t.Type == ConnectivityType.Mobile);
			}
		}

		public bool IsWifiReachable {
			get {
				return _cm.GetAllNetworkInfo ().Any (t => t.IsConnected && t.Type == ConnectivityType.Wifi);
			}
		}

		#endregion


	}
}

