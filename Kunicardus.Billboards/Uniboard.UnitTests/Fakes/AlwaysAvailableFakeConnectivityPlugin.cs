using Kunicardus.Billboards.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uniboard.UnitTests.Fakes
{
    public class AlwaysAvailableFakeConnectivityPlugin : IConnectivityPlugin
    {

        public bool IsNetworkReachable
        {
            get { return true; }
        }

        public bool IsDataReachable
        {
            get { return true; }
        }

        public bool IsWifiReachable
        {
            get { return true; }
        }
    }
}
