using System;
using Kunicardus.Core.ViewModels.iOSSpecific;
using Kunicardus.Touch.Helpers.UI;
using UIKit;
using SharpMobileCode.ModalPicker;
using Foundation;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace Kunicardus.Touch
{
	public class ResetPasswordViewController : BaseRegistrationViewController
	{
		public new iResetPasswordViewModel ViewModel {
			get { return (iResetPasswordViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public ResetPasswordViewController () : base (ApplicationStrings.EnterInfomation)
		{
			ScrollViewOnKeyboardShow = true;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = ApplicationStrings.RestorePassword;
			InitUI ();
		}

		private void InitUI ()
		{
			nfloat padding = 6;
			if (!Screen.IsTall) {
				padding = 3;
			}

			var email = new KuniTextField (
				            new CoreGraphics.CGRect (30, SubHeading.Frame.Bottom + (Screen.IsTall ? 15 : 10), View.Frame.Width - 60, 30), 
				            ApplicationStrings.Email,
				            UIKeyboardType.EmailAddress);
			
			var password = new KuniTextField (
				               new CoreGraphics.CGRect (30, email.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				               ApplicationStrings.NewPassword,
				               UIKeyboardType.Default);
			password.Field.SecureTextEntry = true;

			var confirmPassword = new KuniTextField (
				                      new CoreGraphics.CGRect (30, password.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				                      ApplicationStrings.ConfirmNewPassword,
				                      UIKeyboardType.Default);
			confirmPassword.Field.SecureTextEntry = true;

			email.SetNextField (password);		
			password.SetNextField (confirmPassword);
			password.SetPrevField (email);
			confirmPassword.SetPrevField (password);

			var checkBoxHeight = 18f;
			var checkBox = UIButton.FromType (UIButtonType.Custom);
			checkBox.Frame = new CGRect (30, confirmPassword.Frame.Bottom + padding + 5f, checkBoxHeight, checkBoxHeight);
			checkBox.SetImage (ImageHelper.MaxResizeImage (UIImage.FromFile ("checkbox_white_checked.png"), checkBoxHeight, 0), UIControlState.Selected);
			checkBox.SetImage (ImageHelper.MaxResizeImage (UIImage.FromFile ("checkbox_white_unchecked.png"), checkBoxHeight, 0), UIControlState.Normal);
			checkBox.TitleLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 6);
			checkBox.SetTitle (ApplicationStrings.ShowPassword, UIControlState.Normal);
			checkBox.SetTitleColor (UIColor.White, UIControlState.Normal);
			checkBox.SizeToFit ();
			checkBox.Frame = new CGRect (27, confirmPassword.Frame.Bottom + padding + 5f, checkBox.Frame.Width + 10f, checkBoxHeight);
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

			PasswordHint hint = new PasswordHint (new CGRect (30, checkBox.Frame.Bottom + padding + 3f, View.Frame.Width - 60, 0));
			View.AddSubview (hint);

			// next button init
			RegistrationNextButton next = 
				new RegistrationNextButton (UIButtonType.System);			
			next.Frame = new CGRect ((
			    View.Frame.Width - Styles.RegistrationNextButton.Width) / 2.0f,
				View.Frame.Height - Styles.RegistrationNextButton.Width - Styles.RegistrationNextButton.bottom, 
				Styles.RegistrationNextButton.Width, 
				Styles.RegistrationNextButton.Width);

			View.AddSubview (checkBox);
			View.AddSubview (email);
			View.AddSubview (password);
			View.AddSubview (confirmPassword);
			View.AddSubview (next);
			this.CreateBinding (email.Field).To ((iResetPasswordViewModel vm) => vm.Email).Apply ();
			this.CreateBinding (password.Field).To ((iResetPasswordViewModel vm) => vm.Password).Apply ();
			this.CreateBinding (confirmPassword.Field).To ((iResetPasswordViewModel vm) => vm.ConfirmPassword).Apply ();
			this.CreateBinding (next).To ((iResetPasswordViewModel vm) => vm.ContinueCommand).Apply ();
		}
	}
}

