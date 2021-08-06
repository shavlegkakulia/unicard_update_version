using System;
using UIKit;
using CoreGraphics;
using GameController;
using iCunOSBillBoards;

namespace iCunOS.BillBoards
{
	public class LandingViewController : BaseViewController
	{
		#region Constructor

		public LandingViewController ()
		{
			HideMenuIcon = true;
		}

		#endregion

		#region Overrides

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = true;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBar.Translucent = false;
			AddBackground ();
			InitUI ();
		}

		#endregion

		#region Methods

		private void AddBackground ()
		{
			UIImage image;
			image = UIImage.FromBundle ("landing_page");

			UIImageView imageView = new UIImageView ();
			var frame = View.Frame;
			if (!Screen.IsTall) {
				var proportion = image.Size.Width / View.Frame.Width;
				frame.Height = (proportion * frame.Height) - frame.Height;
			}

			imageView.Frame = frame;
			imageView.Image = image;
			View.AddSubview (imageView);
		}

		private void InitUI ()
		{
			var buttonWidth = View.Frame.Width / 2f + 40f;
			UIButton enterButton = new UIButton (UIButtonType.System);
			enterButton.SetTitle (ApplicationStrings.Enter, UIControlState.Normal);
			enterButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize);
			enterButton.Frame = new CGRect ((View.Frame.Width - buttonWidth) / 2, View.Frame.Height - 80f, buttonWidth, 50f);
			enterButton.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
			enterButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			enterButton.Layer.CornerRadius = 23f;
			enterButton.DropShadowDependingOnBGColor ();

			var labelHeight = 100f;
			var collectPoints = new UILabel ();
			collectPoints.Text = ApplicationStrings.CollectPoints;
			collectPoints.SizeToFit ();
			collectPoints.LineBreakMode = UILineBreakMode.WordWrap;
			collectPoints.Lines = 0;
			collectPoints.Frame = new CGRect (0, (View.Frame.Height - labelHeight) / 2 - 40f, 
				View.Frame.Width, labelHeight);
			collectPoints.TextAlignment = UITextAlignment.Center;
			collectPoints.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize + 5f);
			collectPoints.TextColor = UIColor.White;

			View.AddSubviews (enterButton, collectPoints);
			enterButton.TouchUpInside += OpenLoginPage;
		}

		void OpenLoginPage (object sender, EventArgs e)
		{
			NavigationController.PushViewController (new LoginViewController (), true);
		}

		#endregion
	}
}

