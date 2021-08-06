using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Kunicardus.Droid.BaseWidgets
{
	public class BaseScrollView : ScrollView
	{
		public BaseScrollView (Context context) : base (context)
		{
		}

		public BaseScrollView (Context context, IAttributeSet attrs) : base (context, attrs)
		{
		}

		public BaseScrollView (Context context, IAttributeSet attrs, int defStyleAttr) : base (context, attrs, defStyleAttr)
		{
		}

		public BaseScrollView (Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base (context, attrs, defStyleAttr, defStyleRes)
		{
		}

		protected BaseScrollView (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		}

		public override void RequestChildFocus (View child, View focused)
		{
			if (focused is LinearLayout || focused is GridView) {
				return;
			}
			base.RequestChildFocus (child, focused);
		}
	}
}