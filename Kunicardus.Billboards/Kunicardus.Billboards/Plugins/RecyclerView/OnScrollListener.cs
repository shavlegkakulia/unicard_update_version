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
using Android.Support.V7.Widget;
using Kunicardus.Billboards.Fragments;

namespace Kunicardus.Billboards.Plugins
{
    public class OnScrollListener : Android.Support.V7.Widget.RecyclerView.OnScrollListener
    {
        AdsFragment _fragment;

        public OnScrollListener(AdsFragment fragment)
        {
            _fragment = fragment;
        }

        public override void OnScrolled(Android.Support.V7.Widget.RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            _fragment.allPixels += dx;
        }

        public override void OnScrollStateChanged(Android.Support.V7.Widget.RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            if (newState == Android.Support.V7.Widget.RecyclerView.ScrollStateIdle)
            {
               // _fragment.CalculatePositionAndScroll(recyclerView);
            }
        }
    }
}