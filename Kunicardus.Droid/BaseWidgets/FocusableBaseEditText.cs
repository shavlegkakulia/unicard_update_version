using System;
using Android.Content;
using Android.Util;
using Android.Runtime;
using Android.Widget;
using Android.Views.InputMethods;
using Android.Views;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.FocusableBaseEditText")]
	public class FocusableBaseEditText : EditText
	{
		Boolean _booleanValue;

		public bool BooleanValue {
			get { return _booleanValue; }
			set {
				_booleanValue = value;
				if (ValueChanged != null)
					ValueChanged (value);
			}
		}

		public event ValueChangedEventHandler ValueChanged;

		public delegate void ValueChangedEventHandler (bool value);

		public FocusableBaseEditText (Context context) : base (context)
		{
			setTypeFace ();
		}

		public FocusableBaseEditText (Context context, IAttributeSet attrs) : base (context, attrs)
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


		public FocusableBaseEditText (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			setTypeFace ();
		}

		public FocusableBaseEditText (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
			setTypeFace ();
		}

		private void setTypeFace ()
		{
			Android.Graphics.Typeface tf = global::Android.Graphics.Typeface.CreateFromAsset (Context.Assets, "fonts/bpg_extrasquare_mtavruli.ttf");
			this.SetTypeface (tf, Android.Graphics.TypefaceStyle.Normal);
		}

		public override IInputConnection OnCreateInputConnection (EditorInfo outAttrs)
		{
			var inputConnection = new BaseInputConnection (base.OnCreateInputConnection (outAttrs),
				                      true);
			inputConnection.ValueChanged += (bool value) => {
				if (value)
					this.BooleanValue = value;
			};
			return inputConnection;
		}

		#region BaseInputConenction class implementation

		private class BaseInputConnection : InputConnectionWrapper
		{
			Boolean _booleanValue;

			public bool BooleanValue {
				get { return _booleanValue; }
				set {
					_booleanValue = value;
					if (ValueChanged != null)
						ValueChanged (value);
				}
			}

			public event ValueChangedEventHandler ValueChanged;

			public delegate void ValueChangedEventHandler (bool value);

			public BaseInputConnection (IInputConnection target, bool mutable) : base (target, mutable)
			{
				
			}

			public override bool DeleteSurroundingText (int beforeLength, int afterLength)
			{
				this.BooleanValue = true;
				return base.DeleteSurroundingText (beforeLength, afterLength);
			}

			public override bool SendKeyEvent (Android.Views.KeyEvent e)
			{
				if (e.Action == KeyEventActions.Down
				    && e.KeyCode == Keycode.Del) {
					this.BooleanValue = true;
				}
				return base.SendKeyEvent (e);
			}
		}

		#endregion
	}

}

