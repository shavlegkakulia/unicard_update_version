using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Runtime;

namespace Kunicardus.Billboards
{
	[Register ("Kunicardus.Billboards.BaseButton")]
	public class BaseButton:Button
	{
		public BaseButton (Context context) : base (context)
		{
			setTypeFace ();
		}

		public BaseButton (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			setTypeFace ();
		}

		public BaseButton (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			setTypeFace ();
		}

		public BaseButton (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
			setTypeFace ();
		}

		private void setTypeFace ()
		{
			Android.Graphics.Typeface tf = global::Android.Graphics.Typeface.CreateFromAsset (Context.Assets, "fonts/bpg_extrasquare_mtavruli.ttf");
			this.SetTypeface (tf, Android.Graphics.TypefaceStyle.Normal);
		}
	}
}

