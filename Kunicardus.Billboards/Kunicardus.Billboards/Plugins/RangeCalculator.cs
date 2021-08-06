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

namespace Kunicardus.Billboards.Plugins
{
    public static class RangeCalculator
    {
        public static bool InRange(decimal bearing, decimal startBearing, decimal endBearing)
        {
            return (startBearing <= bearing && endBearing >= bearing);
        }
    }
}