using System;
using Android.Content;
using Android.Util;
using Android.Runtime;
using Android.Widget;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.BaseEditText")]
	public class BaseEditText : EditText
	{
		public BaseEditText (Context context) : base (context)
		{
			setTypeFace ();
		}

		public BaseEditText (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			setTypeFace ();
		}

		public override void SetError (Java.Lang.ICharSequence error, Android.Graphics.Drawables.Drawable icon)
		{	
			if (error != null && error.Length () > 0) {		
				var icon2 = Resources.GetDrawable (Resource.Drawable.validation_error);
				SetCompoundDrawablesWithIntrinsicBounds (null, null, icon2, null);
			} else {
				SetCompoundDrawables (null, null, null, null);
			}		
			//base.SetError ("", icon);
		}


		public BaseEditText (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			setTypeFace ();
		}

		public BaseEditText (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
			setTypeFace ();
		}

		private void setTypeFace ()
		{
//			IntPtr IntPtrtextViewClass = JNIEnv.FindClass (typeof(TextView));
//			IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID (IntPtrtextViewClass, "mCursorDrawableRes", "I");
//			IntPtr jObject = JNIEnv.GetObjectClass (IntPtrtextViewClass);
//			JNIEnv.SetField (IntPtrtextViewClass,
//				mCursorDrawableResProperty, 
//				Resource.Drawable.cursor_design);
//
			Android.Graphics.Typeface tf = global::Android.Graphics.Typeface.CreateFromAsset (Context.Assets, "fonts/bpg_extrasquare_mtavruli.ttf");
			this.SetTypeface (tf, Android.Graphics.TypefaceStyle.Normal);
		}
	}
}

