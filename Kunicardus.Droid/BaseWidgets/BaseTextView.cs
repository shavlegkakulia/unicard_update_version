using System;
using Android.Widget;
using Android.Util;
using Android.Runtime;
using Android.Content;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.BaseTextView")]
	public class BaseTextView:TextView
	{
		public bool IsSelected {
			get;
			set;
		}

		public BaseTextView (Context context) : base (context)
		{
			setTypeFace ();
		}

		public BaseTextView (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			setTypeFace ();
		}

		public BaseTextView (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			setTypeFace ();
		}

		public BaseTextView (IntPtr javaReference, JniHandleOwnership transfer)
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

