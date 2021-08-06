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

namespace Kunicardus.Billboards.Fragments
{
    public abstract class BaseFragment : Android.Support.V4.App.Fragment
    {
        public abstract void OnActivate(object o = null);
    }
}