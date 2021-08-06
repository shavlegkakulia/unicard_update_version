using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using Foundation;
using Kunicardus.Billboards.Core.ViewModels;
using Autofac;

namespace iCunOS.BillBoards
{
	public class SettingsViewController : BaseViewController
	{
		#region Variables

		private  readonly string _version = 
			NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleShortVersionString").ToString () + "." +
			NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleVersion").ToString ();
		private SettingsViewModel _viewModel;

		#endregion

		#region Constructor

		public SettingsViewController ()
		{
			using (var scope = App.Container.BeginLifetimeScope ()) {
				_viewModel = scope.Resolve<SettingsViewModel> ();
			}
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			Title = ApplicationStrings.Settings;
			InitUI ();
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			UIView userView = new UIView (new CGRect (0f, 0f, View.Frame.Width, 80f));
			View.BackgroundColor = UIColor.Clear.FromHexString ("#E0E0E0");

			var imageViewSize = 50f;
			UIImageView userImage = new UIImageView (new CGRect ((80f - imageViewSize) / 2, (80f - imageViewSize) / 2, imageViewSize, imageViewSize));
			userImage.Image = UIImage.FromBundle ("user_settings");
			userView.AddSubview (userImage);

			UILabel userName = new UILabel (new CGRect (userImage.Frame.Right + 10f, 5f, View.Frame.Width - userImage.Frame.Right, 17f));
			userName.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 3f);
			userName.Text = String.Format ("{0} {1}", _viewModel.UserInfoFromDB.FirstName, _viewModel.UserInfoFromDB.LastName);
			userName.SizeToFit ();
			var usernameFrame = userName.Frame;
			usernameFrame.X = userImage.Frame.Right + 10f;
			usernameFrame.Y = 10f;
			userName.Frame = usernameFrame;
			userView.AddSubview (userName);

			if (_viewModel.UserInfoFromDB.IsFacebookUser) {
				UIImageView fbUser = new UIImageView ();
				fbUser.Image = UIImage.FromBundle ("fb_user");
				fbUser.Frame = new CGRect (userName.Frame.Right + 3f, 15f, 10f, 10f);
				userView.AddSubview (fbUser);
			}

			UILabel userEmail = new UILabel (new CGRect (userImage.Frame.Right + 10f, userName.Frame.Bottom - 3f, View.Frame.Width - userImage.Frame.Right, 20f));
			userEmail.Text = _viewModel.UserInfoFromDB.Username;
			userEmail.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
			userEmail.TextColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
			userView.AddSubview (userEmail);

			UIButton logoutButton = new UIButton (UIButtonType.System);
			logoutButton.Frame = new CGRect (userImage.Frame.Right + 10f, userEmail.Frame.Bottom, 70f, 23f);
			var frame = logoutButton.Frame;
			logoutButton.SetTitle (ApplicationStrings.Logout, UIControlState.Normal);
			logoutButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 8f);
			logoutButton.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
			logoutButton.Layer.CornerRadius = 3f;
			logoutButton.TintColor = UIColor.White;

			logoutButton.TouchUpInside += (sender, e) => {
				MenuViewController.LogOut ();
			};
			userView.AddSubview (logoutButton);

			UILabel created = new UILabel ();
			created.Font = UIFont.SystemFontOfSize (UIFont.LabelFontSize - 5f);
			created.Text = "შექმნილია";
			created.SizeToFit ();
			created.TintColor = UIColor.Black;

			UIButton wandio = new UIButton (UIButtonType.System);
			wandio.SetTitle ("ვანდიოს", UIControlState.Normal);
			wandio.Font = UIFont.SystemFontOfSize (UIFont.LabelFontSize - 4f);
			wandio.TintColor = UIColor.LightGray;
			wandio.SetTitleColor (UIColor.Clear.FromHexString (Styles.Colors.Red), UIControlState.Normal);
			wandio.SizeToFit ();

			UILabel byWandio = new UILabel ();
			byWandio.Text = "მიერ";
			byWandio.TintColor = UIColor.Black;
			byWandio.Font = UIFont.SystemFontOfSize (UIFont.LabelFontSize - 5f);
			byWandio.SizeToFit ();

			var cFrame = created.Frame;
			var wFrame = wandio.Frame;
			var bFrame = byWandio.Frame;

			var totalWidth = wFrame.Width + cFrame.Width + bFrame.Width;
			var totalFrame = cFrame;
			totalFrame.X = (View.Frame.Width - totalWidth) / 2;
			totalFrame.Y = View.Frame.Height - 120f;
			created.Frame = totalFrame;

			wFrame.X = totalFrame.Right + 2f;
			wFrame.Y = View.Frame.Height - 127f;
			wandio.Frame = wFrame;

			bFrame.X = wFrame.Right + 2f;
			bFrame.Y = View.Frame.Height - 120f;
			byWandio.Frame = bFrame;

			UILabel versionLabel = new UILabel ();
			versionLabel.Text = string.Format ("ვერსია: {0}", _version);
			versionLabel.Font = UIFont.SystemFontOfSize (UIFont.LabelFontSize - 6f);
			versionLabel.SizeToFit ();
			versionLabel.Frame = new CGRect ((View.Frame.Width - versionLabel.Frame.Width) / 2.0f, totalFrame.Bottom + 10f,
				versionLabel.Frame.Width, versionLabel.Frame.Height);

			View.AddSubview (created);
			View.AddSubview (byWandio);
			View.AddSubview (wandio);
			View.AddSubview (versionLabel);

			userView.BackgroundColor = UIColor.White;

			View.AddSubview (userView);
			wandio.TouchUpInside += (sender, e) => {
				OpenWebPage ("http://www.wandio.com");
			};
		}

		private void OpenWebPage (string webAddress)
		{
			UIApplication.SharedApplication.OpenUrl (new NSUrl (webAddress));
		}


		#endregion
	}
}

