using System;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.Util;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.BaseIngiriTextView")]
	class BaseIngiriTextView:TextView
	{
		public BaseIngiriTextView (Context context) : base (context)
		{
			setTypeFace ();
		}

		public BaseIngiriTextView (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			setTypeFace ();
		}

		public BaseIngiriTextView (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			setTypeFace ();
		}

		public BaseIngiriTextView (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
			setTypeFace ();
		}

		private void setTypeFace ()
		{
			Android.Graphics.Typeface tf = global::Android.Graphics.Typeface.CreateFromAsset (Context.Assets, "fonts/BPG-INGIRI-ARIAL.ttf");
			this.SetTypeface (tf, Android.Graphics.TypefaceStyle.Normal);

		}
			
	}
}