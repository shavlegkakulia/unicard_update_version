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
using Android.Util;

namespace Kunicardus.Billboards
{
	[Register ("Kunicardus.Billboards.BasePointsTextView")]
	public class BasePointsTextView:TextView
	{
		public BasePointsTextView (Context context) : base (context)
		{
			setTypeFace ();
		}

		public BasePointsTextView (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			setTypeFace ();
		}

		public BasePointsTextView (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			setTypeFace ();
		}

		public BasePointsTextView (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
			setTypeFace ();
		}

		private void setTypeFace ()
		{
			Android.Graphics.Typeface tf = global::Android.Graphics.Typeface.CreateFromAsset (Context.Assets, "fonts/ASAP-BOLD.ttf");
			this.SetTypeface (tf, Android.Graphics.TypefaceStyle.Normal);
		}
	}
}