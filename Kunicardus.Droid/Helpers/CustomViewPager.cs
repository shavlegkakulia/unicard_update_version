using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Content;
using Android.Runtime;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.CustomViewPager")]
	public class CustomViewPager: ViewPager
	{
		private bool isTouchEnabled = false;

		public CustomViewPager (Context context)
			: base (context)
		{
		}

		public CustomViewPager (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
		}

		override public bool OnTouchEvent (MotionEvent evt)
		{
			return isTouchEnabled && base.OnTouchEvent (evt);
		}

		override public bool OnInterceptTouchEvent (MotionEvent evt)
		{
			return isTouchEnabled && base.OnInterceptTouchEvent (evt);
		}

		public void EnableTouchEvents (bool isTouchEnabled)
		{
			this.isTouchEnabled = isTouchEnabled;
		}
	}
}

