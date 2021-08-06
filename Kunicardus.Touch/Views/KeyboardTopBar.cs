using System;
using UIKit;

namespace Kunicardus.Touch
{
	public class KeyboardTopBar : UIToolbar
	{
		#region UI

		private UIBarButtonItem prev;
		private UIBarButtonItem next;
		private UIBarButtonItem done;

		#endregion

		#region Props

		public bool PreviousEnabled {
			get { 
				return prev.Enabled;
			}
			set { 
				prev.Enabled = value;
			}
		}

		public bool NextEnabled {
			get { 
				return next.Enabled;
			}
			set { 
				next.Enabled = value;
			}
		}

		public event EventHandler OnPrev;

		public event EventHandler OnNext;

		public event EventHandler OnDone;

		#endregion

		public KeyboardTopBar () : base ()
		{

			float fontSize = 16;
			if (prev == null) {
				prev = new UIBarButtonItem ("Prev", UIBarButtonItemStyle.Plain, (a, b) => {
					if (OnPrev != null) {
						OnPrev (this, null);
					}
				});
				prev.SetTitleTextAttributes (new UITextAttributes (){ Font = UIFont.SystemFontOfSize (fontSize) }, UIControlState.Normal);
				prev.Enabled = false;
			}

			if (next == null) {
				next = new UIBarButtonItem ("Next", UIBarButtonItemStyle.Plain, (a, b) => {
					if (OnNext != null) {
						OnNext (this, null);
					}
				});
				next.SetTitleTextAttributes (new UITextAttributes (){ Font = UIFont.SystemFontOfSize (fontSize) }, UIControlState.Normal);
				next.Enabled = false;
			}


			if (done == null) {
				done = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, (a, b) => {
					if (OnDone != null) {
						OnDone (this, null);
					}
				});
				done.SetTitleTextAttributes (new UITextAttributes (){ Font = UIFont.SystemFontOfSize (fontSize) }, UIControlState.Normal);
			}


			this.BarStyle = UIBarStyle.Default;
			this.Translucent = true;

			this.Frame = new System.Drawing.RectangleF (0, 0, 300, 36.0f);


			this.Items = new UIBarButtonItem[] {
				prev,
				next,
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
				done
			};
			this.SizeToFit ();
		}
	}
}

