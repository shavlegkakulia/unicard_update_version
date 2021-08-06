using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core;
using Accelerate;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Foundation;
using Kunicardus.Core.Models;
using Kunicardus.Core.Helpers.Device;
using SceneKit;

namespace Kunicardus.Touch
{
	public class SettingsViewController : BaseMvxViewController
	{
		#region Private Variables

		private UIButton _changePasswordButton, _setPinButton, _changePinButton, _removePinButton;
		private PinStatus _pinStatus;
		public  readonly string version = 
			NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleShortVersionString").ToString () + "." +
			NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleVersion").ToString ();


		#endregion

		#region Properties

		public new SettingsViewModel ViewModel {
			get { return (SettingsViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		#endregion

		#region Constructor Implementation

		public SettingsViewController ()
		{
			HideMenuIcon = false;
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			Title = ApplicationStrings.Settings;

			NavigationController.NavigationBarHidden = false;
			NavigationController.NavigationBar.Translucent = false;

			var subViews = View.Subviews;
			foreach (var subView in subViews) {
				subView.RemoveFromSuperview ();
			}
			_pinStatus = PinHelper.GetPinStatus (this.ViewModel.UserInfo.UserId);
			InitUI ();
			var set = this.CreateBindingSet<SettingsViewController, SettingsViewModel> ();
			set.Bind (_changePasswordButton).To (vm => vm.OpenChangePasswordPageCommand);
			set.Bind (_removePinButton).To (vm => vm.OpenRemovePinViewModelCommand);
			set.Bind (_changePinButton).To (vm => vm.OpenChangePinViewModelCommand);
			set.Bind (_setPinButton).To (vm => vm.OpenSetPinViewModellCommand);
			set.Apply ();

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}


		#endregion

		#region  Methods

		private void InitUserView ()
		{
			UIView userView = new UIView (new CGRect (0, 0, View.Frame.Width, View.Frame.Height));
			userView.BackgroundColor = UIColor.Clear.FromHexString ("#f3f3f3");

			var imageViewSize = 50f;
			UIImageView userImage = new UIImageView (new CGRect ((80f - imageViewSize) / 2, (80f - imageViewSize) / 2, imageViewSize, imageViewSize));
			userImage.Image = UIImage.FromBundle ("user_settings");
			userView.AddSubview (userImage);

			UILabel userName = new UILabel (new CGRect (userImage.Frame.Right + 10f, 5f, View.Frame.Width - userImage.Frame.Right, 17f));
			userName.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 3f);
			userName.Text = String.Format ("{0} {1}", ViewModel.UserInfo.FirstName, ViewModel.UserInfo.LastName);
			userName.SizeToFit ();
			var usernameFrame = userName.Frame;
			usernameFrame.X = userImage.Frame.Right + 10f;
			usernameFrame.Y = 10f;
			userName.Frame = usernameFrame;
			userView.AddSubview (userName);

			if (ViewModel.UserInfo.IsFacebookUser) {
				UIImageView fbUser = new UIImageView ();
				fbUser.Image = UIImage.FromBundle ("fb_user");
				fbUser.Frame = new CGRect (userName.Frame.Right + 3f, 15f, 10f, 10f);
				userView.AddSubview (fbUser);
			}

			UILabel userEmail = new UILabel (new CGRect (userImage.Frame.Right + 10f, userName.Frame.Bottom - 3f, View.Frame.Width - userImage.Frame.Right, 20f));
			userEmail.Text = ViewModel.UserInfo.Username;
			userEmail.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
			userEmail.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			userView.AddSubview (userEmail);

			UIButton logoutButton = new UIButton (UIButtonType.System);
			logoutButton.Frame = new CGRect (userImage.Frame.Right + 10f, userEmail.Frame.Bottom, 70f, 23f);
			var frame = logoutButton.Frame;
			logoutButton.SetTitle (ApplicationStrings.Logout, UIControlState.Normal);
			logoutButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 8f);
			logoutButton.BackgroundColor = UIColor.Clear.FromHexString ("#ed9407");
			logoutButton.Layer.CornerRadius = 3f;
			logoutButton.TintColor = UIColor.White;

			logoutButton.TouchUpInside += (sender, e) => {
				ViewModel.Logout ();
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
			wandio.SetTitleColor (UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen), UIControlState.Normal);
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
			versionLabel.Text = string.Format ("ვერსია: {0}", version);
			versionLabel.Font = UIFont.SystemFontOfSize (UIFont.LabelFontSize - 6f);
			versionLabel.SizeToFit ();
			versionLabel.Frame = new CGRect ((View.Frame.Width - versionLabel.Frame.Width) / 2.0f, totalFrame.Bottom + 10f,
				versionLabel.Frame.Width, versionLabel.Frame.Height);

			userView.AddSubview (created);
			userView.AddSubview (byWandio);
			userView.AddSubview (wandio);
			userView.AddSubview (versionLabel);
			View.AddSubview (userView);

			wandio.TouchUpInside += (sender, e) => {
				OpenWebPage ("http://www.wandio.com");
			};
		}

		private void OpenWebPage (string webAddress)
		{
			UIApplication.SharedApplication.OpenUrl (new NSUrl (webAddress));
		}

		private void InitUI ()
		{

			InitUserView ();

			nfloat linePosition = 80f;

			nfloat tempPadding = 0;
			if (Screen.IsTall)
				tempPadding = 45f;
			else
				tempPadding = 40f;
			if (ViewModel.ShouldShowChangePassword ()) {
				_changePasswordButton = new UIButton (UIButtonType.System);
				AddButtonView (linePosition, tempPadding, ApplicationStrings.ChangePassword, _changePasswordButton);
				linePosition += tempPadding;
			}

			if (_pinStatus == PinStatus.FirstInit || _pinStatus == PinStatus.NoPin) {
				_setPinButton = new UIButton (UIButtonType.System);
				AddButtonView (linePosition, tempPadding, ApplicationStrings.SetPin, _setPinButton);
				linePosition += tempPadding;
			} 

			if (_pinStatus == PinStatus.ShouldEnterPin) {
				_changePinButton = new UIButton (UIButtonType.System);
				AddButtonView (linePosition, tempPadding, ApplicationStrings.ChangePin, _changePinButton);
				linePosition += tempPadding;
			} 
			if (_pinStatus == PinStatus.ShouldEnterPin) {
				_removePinButton = new UIButton (UIButtonType.System);
				AddButtonView (linePosition, tempPadding, ApplicationStrings.RemovePin, _removePinButton);
			}

		}



		private UIView LineDivider (nfloat top)
		{
			UIView view = new UIView (new CGRect (0, 0, View.Frame.Width, 1f));
			view.BackgroundColor = UIColor.Clear.FromHexString ("#aaa");
			return view;
		}

		private void AddSwitchSubView (UISwitch sw, nfloat top, nfloat height, string name, UIView v)
		{
			v = new UIView (new CGRect (0, top, View.Frame.Width, height));
			v.AddSubview (LineDivider (height));
			v.BackgroundColor = UIColor.White;

			var label = new UILabel (new CGRect (20, 0, 200, height));
			label.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 4);
			label.TextColor = UIColor.Clear.FromHexString ("8DBD3B");
			label.Text = name;
			v.AddSubview (label);
			sw.Transform = CGAffineTransform.MakeScale (0.65f, 0.65f);
			v.AddSubview (sw);
			View.AddSubview (v);
		}

		private UIView AddButtonView (nfloat top, nfloat height, string name, UIButton myButton)
		{
			UIView v = new UIView (new CGRect (0, top, View.Frame.Width, height));
			v.AddSubview (LineDivider (height));
			v.BackgroundColor = UIColor.White;
			var imageSize = 20f;
			myButton.Frame = new CGRect (0, 0, View.Frame.Width, height);
			myButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 4);
			myButton.SetTitle (name, UIControlState.Normal);
			myButton.SetTitleColor (UIColor.Clear.FromHexString ("#8DBD3B"), UIControlState.Normal);
			myButton.SetImage (ImageHelper.MaxResizeImage (UIImage.FromBundle ("right_arrow"), imageSize, imageSize), UIControlState.Normal);
			myButton.ImageEdgeInsets = new UIEdgeInsets (0.0f, View.Frame.Width - 2 * imageSize, 3.0f, 0.0f);
			myButton.TitleEdgeInsets = new UIEdgeInsets (0.0f, 0.0f, 0.0f, 0.0f);
			myButton.TintColor = UIColor.LightGray;
			myButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			v.AddSubview (myButton);	
			View.AddSubview (v);
			return v;
		}

		#endregion
	}
}

