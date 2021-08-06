using System;
using Android.Content;
using Android.Widget;
using Android.Util;
using Android.Runtime;

namespace Kunicardus.Droid
{

	[Register ("Kunicardus.Droid.BasePaymentEditText")]
	public class BasePaymentEditText:EditText
	{
		public BasePaymentEditText (Context context) : base (context)
		{
			setTypeFace ();
		}

		public BasePaymentEditText (Context context, IAttributeSet attrs) : base (context, attrs)
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


		public BasePaymentEditText (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			setTypeFace ();
		}

		public BasePaymentEditText (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
			setTypeFace ();
		}

		private void setTypeFace ()
		{
	
			Android.Graphics.Typeface tf = global::Android.Graphics.Typeface.CreateFromAsset (Context.Assets, "fonts/bpg_extrasquare_mtavruli.ttf");
			this.SetTypeface (tf, Android.Graphics.TypefaceStyle.Normal);
			this.SetTextSize (ComplexUnitType.Sp, 14);
//			this.SetPadding (11, 11, 11, 11);
			this.SetHintTextColor (Resources.GetColor (Resource.Color.lightgray));
			this.SetTextColor (Resources.GetColor (Android.Resource.Color.Black));
			this.SetSingleLine (true);
//			this.LineCount = 1;
//			this.MaxLines = 1;
			this.SetBackgroundResource (Resource.Drawable.rounded_background);
//			this.ImeOptions = Android.Views.InputMethods.ImeAction.Next;
		}
	}
}


