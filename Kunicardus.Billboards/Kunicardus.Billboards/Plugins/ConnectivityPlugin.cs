using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Net;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Kunicardus.Billboards.Core.Plugins;

namespace Kunicardus.Billboards.Plugins
{

    public class ConnectivityPlugin : IConnectivityPlugin
    {
        private readonly ConnectivityManager _cm;

        public ConnectivityPlugin()
        {
            _cm = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
        }

        #region IConnectivityPlugin implementation

        public bool IsNetworkReachable
        {
            get
            {
                var activeNetwork = _cm.ActiveNetworkInfo;
                return null != activeNetwork && activeNetwork.IsConnectedOrConnecting;
            }
        }

        public bool IsDataReachable
        {
            get
            {
                return _cm.GetAllNetworkInfo().Any(t => t.IsConnectedOrConnecting && t.Type == ConnectivityType.Mobile);
            }
        }

        public bool IsWifiReachable
        {
            get
            {
                return _cm.GetAllNetworkInfo().Any(t => t.IsConnected && t.Type == ConnectivityType.Wifi);
            }
        }

        #endregion


    }
}