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
using Android.Support.V4.View;
using Android.Util;

namespace Kunicardus.Billboards.Helpers
{
    public class CustomViewPager : ViewPager
    {
        private bool isTouchEnabled = true;

        public CustomViewPager(Context context) : base(context)
        {
        }

        public CustomViewPager(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        override public bool OnTouchEvent(MotionEvent evt)
        {
            return isTouchEnabled && base.OnTouchEvent(evt);
        }

        override public bool OnInterceptTouchEvent(MotionEvent evt)
        {
            return isTouchEnabled && base.OnInterceptTouchEvent(evt);
        }

        public void EnableTouchEvents(bool isTouchEnabled)
        {
            this.isTouchEnabled = isTouchEnabled;
        }
    }
}