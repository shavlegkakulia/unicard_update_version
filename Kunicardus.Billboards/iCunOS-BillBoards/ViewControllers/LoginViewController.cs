using System;
using UIKit;
using CoreGraphics;
using Autofac;
using Kunicardus.Billboards.Core.ViewModels;
using Facebook.CoreKit;
using Foundation;
using CoreText;
using Facebook.LoginKit;
using System.Threading.Tasks;

namespace iCunOS.BillBoards
{
	public class LoginViewController : BaseViewController
	{
		#region Variables

		private LoginViewModel _viewModel;

		#endregion

		#region Constructors

		public LoginViewController ()
		{
			HideMenuIcon = true;
			using (var scope = App.Container.BeginLifetimeScope ()) {
				_viewModel = scope.Resolve<LoginViewModel> ();
			}
		}

		#endregion

		#region Overrides

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
			NavigationController.NavigationBarHidden = false;
			NavigationController.NavigationBar.Translucent = false;
			_manager = new LoginManager ();
			InitView ();
		}

		#endregion

		#region Methods

		private void InitView ()
		{
			var imageSize = 110f;
			var uniboardImageView = new UIImageView ();
			uniboardImageView.Image = UIImage.FromBundle ("uniboard");
			uniboardImageView.Frame = new CGRect ((View.Frame.Width - imageSize) / 2, 20f, imageSize, imageSize);

			var welcomeText = new UILabel ();
			welcomeText.Text = ApplicationStrings.Welcome;
			welcomeText.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize + 7f);
			welcomeText.SizeToFit ();
			var wFrame = welcomeText.Frame;
			wFrame.X = (View.Frame.Width - welcomeText.Frame.Width) / 2;
			wFrame.Y = uniboardImageView.Frame.Bottom + 20f;
			welcomeText.Frame = wFrame;
			welcomeText.TextColor = UIColor.White;

			var buttonWidth = View.Frame.Width - 50f;

			UIButton fbButton = new UIButton (UIButtonType.System);
			fbButton.SetTitle (ApplicationStrings.FbLogin, UIControlState.Normal);
			fbButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 3);
			fbButton.Frame = new CGRect ((View.Frame.Width - buttonWidth) / 2, welcomeText.Frame.Bottom + 10f, buttonWidth, 40f);
			fbButton.BackgroundColor = UIColor.Clear.FromHexString ("#425d9e");
			fbButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			fbButton.Layer.CornerRadius = 20f;
			var fbImage = ImageHelper.MaxResizeImage (UIImage.FromBundle ("fb_letter"), 15f, 20f);
			fbButton.SetImage (fbImage, UIControlState.Normal);
			fbButton.TintColor = UIColor.White;	
			fbButton.ImageEdgeInsets = new UIEdgeInsets (8f, 0f, 8f, 0f);
			fbButton.TitleEdgeInsets = new UIEdgeInsets (0.0f, 20.0f, 0.0f, 0.0f);

			UIButton authButton = new UIButton (UIButtonType.System);
			authButton.SetTitle (ApplicationStrings.Auth, UIControlState.Normal);
			authButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 3);
			authButton.Frame = new CGRect ((View.Frame.Width - buttonWidth) / 2, fbButton.Frame.Bottom + 10f, buttonWidth, 40f);
			authButton.BackgroundColor = UIColor.Clear.FromHexString ("#f2bb2c");
			authButton.TintColor = UIColor.Black;
			authButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			authButton.Layer.CornerRadius = 20f; 

			var loginInfo = new UILabel ();
			loginInfo.Frame = new CGRect ();
			loginInfo.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f);
			loginInfo.TextColor = UIColor.White;
			loginInfo.TextAlignment = UITextAlignment.Center;
			loginInfo.LineBreakMode = UILineBreakMode.WordWrap;
			loginInfo.Lines = 0;
			var attribute = new UIStringAttributes {
				ForegroundColor = UIColor.Clear.FromHexString ("#ffc115"),
				BackgroundColor = UIColor.Clear,
				Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f)
			};
			var prettyString = new NSMutableAttributedString (ApplicationStrings.LoginInfo);
			prettyString.SetAttributes (attribute.Dictionary, new NSRange (35, 10));
			loginInfo.AttributedText = prettyString;

			loginInfo.SizeToFit ();
			var lFrame = loginInfo.Frame;
			lFrame.X = 0;
			lFrame.Y = authButton.Frame.Bottom + 20f;
			lFrame.Width = View.Frame.Width;
			loginInfo.Frame = lFrame;

			AddRegisterView ();
			View.AddSubviews (fbButton, 
				authButton, 
				uniboardImageView, 
				welcomeText,
				loginInfo);

			authButton.TouchUpInside += AuthButtonClicked;
			fbButton.TouchUpInside += FbButtonClicked;
		}


		private void AddRegisterView ()
		{//dc745a
			var regHeight = 50f;
			var registerButton = new UIButton (UIButtonType.System);
			registerButton.SetTitle (ApplicationStrings.Register, UIControlState.Normal);
			registerButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 3f);
			var attributes = new UIStringAttributes {
				ForegroundColor = UIColor.Clear.FromHexString ("#f2bb2c"),
				BackgroundColor = UIColor.Clear,
				UnderlineStyle = NSUnderlineStyle.Single
			};
			registerButton.BackgroundColor = UIColor.Clear.FromHexString ("#df5a38");
			registerButton.TintColor = UIColor.Clear.FromHexString ("f2bb2c");
			registerButton.TitleLabel.AttributedText = new NSAttributedString (ApplicationStrings.Register, attributes);
			registerButton.Frame = new CGRect (0, View.Frame.Height - (regHeight + GetStatusBarHeight ()), View.Frame.Width, regHeight);
			View.AddSubview (registerButton);

			registerButton.TouchUpInside += OpenRegisterWebPage;
		}

		void OpenRegisterWebPage (object sender, EventArgs e)
		{
			string address = "http://unicard.ge/ge/registration";
			UIApplication.SharedApplication.OpenUrl (new NSUrl (address));
		}

		#endregion

		#region Events

		void FbButtonClicked (object sender, EventArgs e)
		{
			_manager.LogInWithReadPermissions (EXTENDED_PERMISSIONS, (res, err) => FbLoginHandler (res, err));
		}

		void AuthButtonClicked (object sender, EventArgs e)
		{
			NavigationController.PushViewController (new AuthViewController (), true);
		}

		#endregion

		#region Facebook

		private LoginManager _manager;
		private readonly string[] EXTENDED_PERMISSIONS = new [] { "email" };

		void FbLoginHandler (LoginManagerLoginResult result, Foundation.NSError error)
		{
			if (error != null && error.Code > 0) {

			} else if (result.IsCancelled) {

			} else if (!result.GrantedPermissions.Contains ("email")) {				
				ShowErrorPermissionDialog ();
			} else {

				var request = new GraphRequest ("me", null, AccessToken.CurrentAccessToken.TokenString, null, "GET");
				var requestConnection = new GraphRequestConnection ();

				requestConnection.AddRequest (request, (c, lres, lerr) => {
					if (lres != null) {
						var res_parameters = (lres as NSDictionary);
						if (res_parameters != null) {
							string fb_email = res_parameters ["email"].ToString ();
							string fb_id = result.Token.TokenString;
							string fb_firstname = res_parameters ["first_name"].ToString ();
							string fb_lastname = res_parameters ["last_name"].ToString ();
							DialogPlugin.ShowProgressDialog ("");
							System.Threading.Tasks.Task.Run (() => {
								var response = _viewModel.FacebookConnect (fb_firstname, fb_lastname, fb_email, fb_id);
								InvokeOnMainThread (() => {
									DialogPlugin.DismissProgressDialog ();
									if (response.Success) {
										NavigationController.SetViewControllers (new UIViewController[] { new RootViewController () }, true);				
									} else {
										DialogPlugin.ShowToast (response.DisplayMessage);
									}
								});
							});
						}
					}
				});
				requestConnection.Start ();
			}				
		}

		private void ShowErrorPermissionDialog ()
		{
			new UIAlertView (ApplicationStrings.Error,
				ApplicationStrings.WeNeedAccessToYourEmailToConnectWithFB, 
				null, 
				ApplicationStrings.GotIt).Show ();
		}

		#endregion
	}
}

