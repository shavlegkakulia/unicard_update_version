using System;
using UIKit;

namespace iCunOS.BillBoards
{
	public class KuniBadgeBarButtonItem : UIControl
	{
		#region Vars

		private Action _action;
		private string _icon;
		private int _badgeCount;
		private UIButton _button;

		#endregion

		#region Props

		public int BadgeCount {
			get {
				return _badgeCount;
			}
			set { 
				_badgeCount = value;
				UpdateBadgeCount (value);
			}
		}

		#endregion

		#region Ctors

		public KuniBadgeBarButtonItem (string icon, Action action, int badgeCount = 0)
		{
			this.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Red);

			_action = action;
			_icon = icon;
			_badgeCount = badgeCount;
			this.Frame = new CoreGraphics.CGRect (0, 0, 27, 27);
		}

		#endregion

		#region Methods

		public void UpdateBadgeCount (int count)
		{
			_badgeCount = count;
			if (_badgeCount > 0) {
				_button.SetImage (GetBadgeImage (), UIControlState.Normal);
			} else {
				_button.SetImage (null, UIControlState.Normal);
			}
		}

		private UIImage ImageFromView (UIView view)
		{
			UIGraphics.BeginImageContextWithOptions (view.Frame.Size, view.Opaque, 0.0f);
			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return img;
		}

		private UIImage GetBadgeImage ()
		{
			UIButton badge = new UIButton (UIButtonType.RoundedRect);
			badge.SetTitleColor (UIColor.White, UIControlState.Normal);
			badge.Frame = new CoreGraphics.CGRect (15, 0, 16, 16);
			badge.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.StrangeYellow);
			badge.Layer.BorderColor = UIColor.White.CGColor;
			badge.Layer.BorderWidth = 1f;
			badge.Layer.CornerRadius = 7.5f;
			badge.Font = UIFont.SystemFontOfSize (12);
			badge.TintColor = UIColor.White;

			badge.SetTitle (_badgeCount.ToString (), UIControlState.Normal);

			return ImageFromView (badge);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			_button = new UIButton (UIButtonType.Custom);
			UIImage img = UIImage.FromBundle (_icon);
			_button.SetBackgroundImage (img, UIControlState.Normal);
			_button.TintColor = UIColor.White;
			_button.SetTitleColor (UIColor.White, UIControlState.Normal);
			_button.Frame = new CoreGraphics.CGRect (0, 2, 23, 23);
			if (_badgeCount > 0) {
				_button.SetImage (GetBadgeImage (), UIControlState.Normal);
			}
			_button.ImageEdgeInsets = new UIEdgeInsets (-2, 14, 12, -4);

			AddSubview (_button);

			if (_action != null) {
				this.TouchUpInside += delegate {
					_action.Invoke ();
				};
			}
		}

		#endregion
	
	}
}

