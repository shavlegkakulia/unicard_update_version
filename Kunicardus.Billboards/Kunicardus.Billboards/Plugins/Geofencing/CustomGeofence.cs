using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Location;
using Kunicardus.Billboards.Core.Enums;

namespace Kunicardus.Billboards.Plugins.Geofencing
{
    public class CustomGeofence
    {
        public string RequestId { get; set; }
        public int BillboardId { get; set; }
        public bool IsSeen { get; set; }
        public int AdvertismentId { get; set; }
        public decimal StartBearing { get; set; }
        public decimal EndBearing { get; set; }
    }
}