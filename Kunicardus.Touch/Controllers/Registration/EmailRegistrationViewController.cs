using System;
using Kunicardus.Core.ViewModels.iOSSpecific;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace Kunicardus.Touch
{
	public class EmailRegistrationViewController : BaseRegistrationViewController
	{
		
		public new iEmailRegistrationViewModel ViewModel {
			get { return (iEmailRegistrationViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public EmailRegistrationViewController () : base (ApplicationStrings.EnterEmailAndPassword)
		{
			ScrollViewOnKeyboardShow = true;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			InitUI ();
		}

		private void InitUI ()
		{			
			var userName = new KuniTextField (
				               new CoreGraphics.CGRect (30, SubHeading.Frame.Bottom + 20, View.Frame.Width - 60, 30), 
				               ApplicationStrings.Email,
				               UIKeyboardType.EmailAddress);
			var password = new KuniTextField (
				               new CoreGraphics.CGRect (30, userName.Frame.Bottom + 10, View.Frame.Width - 60, 30), 
				               ApplicationStrings.Password,
				               UIKeyboardType.Default);
			password.Field.SecureTextEntry = true;

			var confirmPassword = new KuniTextField (
				                      new CoreGraphics.CGRect (30, password.Frame.Bottom + 10, View.Frame.Width - 60, 30), 
				                      ApplicationStrings.ConfirmPassword,
				                      UIKeyboardType.Default);
			confirmPassword.Field.SecureTextEntry = true;

			var checkboxPadding = 30;
			var checkBoxHeight = 18f;
			var checkBox = UIButton.FromType (UIButtonType.Custom);
			checkBox.Frame = new CGRect (30, confirmPassword.Frame.Bottom + 8f, checkBoxHeight, checkBoxHeight);
			checkBox.SetImage (ImageHelper.MaxResizeImage (UIImage.FromFile ("checkbox_white_checked.png"), checkBoxHeight, 0), UIControlState.Selected);
			checkBox.SetImage (ImageHelper.MaxResizeImage (UIImage.FromFile ("checkbox_white_unchecked.png"), checkBoxHeight, 0), UIControlState.Normal);
			checkBox.TitleLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 6);
			checkBox.SetTitle (ApplicationStrings.ShowPassword, UIControlState.Normal);
			checkBox.SetTitleColor (UIColor.White, UIControlState.Normal);
			checkBox.SizeToFit ();
			checkBox.Frame = new CGRect (27, confirmPassword.Frame.Bottom + checkboxPadding + 5f, checkBox.Frame.Width + 10f, checkBoxHeight);
			checkBox.TitleEdgeInsets = new UIEdgeInsets (0, 10, 0, 0);

			checkBox.TouchUpInside += (sender, e) => {
				checkBox.Selected = !checkBox.Selected;
				if (checkBox.Selected) {
					password.SecureField (false);
					confirmPassword.SecureField (false);
				} else {
					password.SecureField (true);
					confirmPassword.SecureField (true);
				}
			};
			View.AddSubview (checkBox);

			// Setting next prev fields
			userName.SetNextField (password);
			password.SetNextField (confirmPassword);
			password.SetPrevField (userName);
			confirmPassword.SetPrevField (password);

			PasswordHint hint = new PasswordHint (new CGRect (30, confirmPassword.Frame.Bottom + 25, View.Frame.Width - 60, 0));
			View.AddSubview (hint);

			RegistrationNextButton next = 
				new RegistrationNextButton (UIButtonType.System);			
			next.Frame = new CGRect ((
			    View.Frame.Width - Styles.RegistrationNextButton.Width) / 2.0f,
				View.Frame.Height - Styles.RegistrationNextButton.Width - Styles.RegistrationNextButton.bottom, 
				Styles.RegistrationNextButton.Width, 
				Styles.RegistrationNextButton.Width);

			// Bindings
			this.CreateBinding (userName.Field).To ((iEmailRegistrationViewModel vm) => vm.Email).Apply ();
			this.CreateBinding (password.Field).To ((iEmailRegistrationViewModel vm) => vm.Password).Apply ();
			this.CreateBinding (confirmPassword.Field).To ((iEmailRegistrationViewModel vm) => vm.ConfirmPassword).Apply ();
			this.CreateBinding (next).To ((iEmailRegistrationViewModel vm) => vm.ContinueCommand).Apply ();

			View.AddSubview (userName);
			View.AddSubview (password);
			View.AddSubview (confirmPassword);
			View.AddSubview (next);
		}
	}
}

