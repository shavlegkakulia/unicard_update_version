using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class RadioButton : UIButton
	{
		#region Vars

		static object locker = new object ();

		#endregion

		#region Eventhandlers

		public event EventHandler Checked;

		#endregion

		#region Props

		public string Value {
			get;
			set;
		}

		public bool IsChecked {
			get {
				return this.Title (UIControlState.Normal) != "";
			}
			set { 				
				this.SetTitle (value ? "•" : "", UIControlState.Normal);
			}
		}

		#endregion

		#region Ctors

		public RadioButton (CGRect frame) : base ()
		{			
			this.Frame = frame;
			this.BackgroundColor = UIColor.Clear.FromHexString ("#a4cf4a");
			this.Layer.BorderColor = UIColor.Clear.FromHexString (Styles.Colors.PlaceHolderColor).CGColor;
			this.Layer.BorderWidth = 2f;
			this.Layer.CornerRadius = this.Frame.Width / 2.0f;
			this.SetTitleColor (UIColor.White, UIControlState.Normal);
			this.TintColor = UIColor.White;
			this.Font = UIFont.SystemFontOfSize (54);
			this.TitleEdgeInsets = new UIEdgeInsets (1.2f, 0.4f, 1, 1.2f);
			this.TouchUpInside += RadioButton_TouchUpInside;
		}

		#endregion

		#region Events

		void RadioButton_TouchUpInside (object sender, EventArgs e)
		{
			lock (locker) {
				if (string.IsNullOrWhiteSpace (this.Title (UIControlState.Normal))) {
					IsChecked = true;
				} else {
					IsChecked = false;
				}
				if (Checked != null) {
					Checked (this, null);
				}
			}				
		}

		#endregion
	}
}

