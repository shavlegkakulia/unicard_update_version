using System;
using Kunicardus.Core;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using Foundation;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core.Converters;
using Cirrious.MvvmCross.Binding;

namespace Kunicardus.Touch
{
	public class AuthViewController : BaseMvxViewController
	{
		#region UI

		KuniTextField _userName;
		KuniTextField _password;

		#endregion

		#region Properties

		public new LoginAuthViewModel ViewModel {
			get { return (LoginAuthViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		#endregion

		#region Constructors

		public AuthViewController ()
		{			
			HideMenuIcon = true;
//			_validationConverter = new iOSValidationColorValueConverter ();
//			this.CreateBinding (this)
//				.For (view => view.ViewShouldBeValidated)
//				.To<LoginAuthViewModel> (vm => vm.ShouldValidateModel)
//				.Apply ();
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{			
			base.ViewDidLoad ();
			InitUI ();

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
			if (!string.IsNullOrWhiteSpace (_userName.Field.Text) && string.IsNullOrWhiteSpace (_password.Field.Text)) {
				_password.Field.BecomeFirstResponder ();
			}
				
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			Title = ApplicationStrings.Authorize;	
			View.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);

			_userName = new KuniTextField (
				new CoreGraphics.CGRect (30, 100, View.Frame.Width - 60, 30), 
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
			justAuthorize.SetTitle (ApplicationStrings.Authorize, UIControlState.Normal);
			justAuthorize.Layer.CornerRadius = 25;
			justAuthorize.TintColor = UIColor.White;
			justAuthorize.SetTitleColor (UIColor.White, UIControlState.Normal);
			justAuthorize.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 2);
			justAuthorize.DropShadowDependingOnBGColor ();

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


			// Bindings
			this.CreateBinding (_userName.Field).To ((LoginAuthViewModel vm) => vm.UserName).Apply ();
			this.CreateBinding (_password.Field).To ((LoginAuthViewModel vm) => vm.Password).Apply ();
			this.CreateBinding (justAuthorize).To ((LoginAuthViewModel vm) => vm.AuthCommand).Apply ();
			this.CreateBinding (restorePassword).To ((LoginAuthViewModel vm) => vm.ForgotPassword).Apply ();
			//this.CreateBinding (_password.Line).FullyDescribed ("BackgroundColor ValidationColor(Password, true)").Apply ();
//			this.CreateBinding (_password.Line).For (x => x.BackgroundColor).
//			To ((LoginAuthViewModel vm) => vm.Password).WithConversion (_validationConverter, null).Apply ();

		}

		#endregion
	}
}

