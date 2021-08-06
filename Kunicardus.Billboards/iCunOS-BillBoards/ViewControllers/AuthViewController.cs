using System;
using UIKit;
using CoreGraphics;
using CoreText;
using Kunicardus.Billboards.Core.ViewModels;
using Autofac;
using System.Threading.Tasks;
using Foundation;

namespace iCunOS.BillBoards
{
	public class AuthViewController : BaseViewController
	{
		#region Variables

		private AuthViewModel _viewModel;

		#endregion

		#region UI

		KuniTextField _userName, _password;

		#endregion

		#region Ctors

		public AuthViewController ()
		{
			ScrollViewOnKeyboardShow = true;
			HideMenuIcon = true;
			using (var scope = App.Container.BeginLifetimeScope ()) {
				_viewModel = scope.Resolve<AuthViewModel> ();
			}
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			InitUI ();
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			Title = ApplicationStrings.Auth;	
			View.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Red);

			var topPosition = 90f;
			if (!Screen.IsTall)
				topPosition = 30f;
			_userName = new KuniTextField (
				new CoreGraphics.CGRect (30, topPosition, View.Frame.Width - 60, 30), 
				ApplicationStrings.UserName,
				UIKeyboardType.EmailAddress);
			_userName.Field.AutocapitalizationType = UITextAutocapitalizationType.None;

			_password = new KuniTextField (new CoreGraphics.CGRect (30, _userName.Frame.Bottom + 10, View.Frame.Width - 60, 30),
				ApplicationStrings.Password);
			_password.Field.SecureTextEntry = true;

			// Setting next and prev fields
			_userName.SetNextField (_password);
			_password.SetPrevField (_userName);

			UIButton justAuthorize = new UIButton (UIButtonType.RoundedRect);
			justAuthorize.Frame = new CGRect (30, _password.Frame.Bottom + 30, View.Frame.Width - 60, 50);
			justAuthorize.BackgroundColor = UIColor.Clear.FromHexString ("#f2bb2c");
			justAuthorize.SetTitle (ApplicationStrings.Auth, UIControlState.Normal);
			justAuthorize.Layer.CornerRadius = 25;
			justAuthorize.TintColor = UIColor.White;
			justAuthorize.SetTitleColor (UIColor.White, UIControlState.Normal);
			justAuthorize.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 2);
			justAuthorize.DropShadowDependingOnBGColor ();
			justAuthorize.TouchUpInside += AuthUser;

			UIButton restorePassword = new UIButton (UIButtonType.System);
			restorePassword.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 2);
			var underlineregistration = new NSAttributedString (
				                            ApplicationStrings.RestorePassword, 
				                            underlineStyle: NSUnderlineStyle.Single, foregroundColor: UIColor.Clear.FromHexString (Styles.Colors.Yellow));
			restorePassword.SetAttributedTitle (underlineregistration, UIControlState.Normal);
			restorePassword.SizeToFit ();
			restorePassword.Frame = 
				new CGRect ((View.Frame.Width - restorePassword.Frame.Width) / 2.0f,
				justAuthorize.Frame.Bottom + 20,
				restorePassword.Frame.Width,
				restorePassword.Frame.Height);			

			// Adding subviews
			View.AddSubview (_userName);
			View.AddSubview (_password);
			View.AddSubview (justAuthorize);
			View.AddSubview (restorePassword);

			restorePassword.TouchUpInside += OpenRestorePasswordWebPage;

			#if DEBUG   
			_userName.Field.Text = "smamuchishvili@gmail.com";
			_password.Field.Text = "bulvari111";
			#endif

		}

		#endregion

		#region Events

		void AuthUser (object sender, EventArgs e)
		{
			var userName = _userName.Field.Text;
			var password = _password.Field.Text;
			DialogPlugin.ShowProgressDialog (ApplicationStrings.Loading);
			Task.Run (() => {
				var response = _viewModel.Auth (userName, password);
				InvokeOnMainThread (() => {
					DialogPlugin.DismissProgressDialog ();				
					if (response) {
						NavigationController.SetViewControllers (new UIViewController[] { new RootViewController () }, true);				
					} else {
						DialogPlugin.ShowToast (_viewModel.DisplayMessage);
					}						
				});
			});
		}

		void OpenRestorePasswordWebPage (object sender, EventArgs e)
		{
			string address = "http://unicard.ge/ge/reset";
			UIApplication.SharedApplication.OpenUrl (new NSUrl (address));
		}

		#endregion
	}
}

