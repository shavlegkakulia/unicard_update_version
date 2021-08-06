using System;
using Android.Content;
using Android.Util;
using Android.Runtime;
using Xamarin.Facebook.Login.Widget;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.BaseFBButton")]
	public class BaseFBButton:LoginButton
	{
		public BaseFBButton (Context context) : base (context)
		{
			setTypeFace ();
		}

		public BaseFBButton (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			setTypeFace ();
		}

		public BaseFBButton (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			setTypeFace ();
		}

		public BaseFBButton (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
			setTypeFace ();
		}

		private void setTypeFace ()
		{
			this.SetCompoundDrawablesWithIntrinsicBounds (0, 0, 0, 0);
			this.SetBackgroundResource (Resource.Drawable.login_fbAuthorization);
			this.SetText (Resource.String.empty);
			Android.Graphics.Typeface tf = global::Android.Graphics.Typeface.CreateFromAsset (Context.Assets, "fonts/bpg_extrasquare_mtavruli.ttf");
			this.SetTypeface (tf, Android.Graphics.TypefaceStyle.Normal);
		}
	}
}

