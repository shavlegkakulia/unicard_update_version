using System;
using Kunicardus.Core.ViewModels.iOSSpecific;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using SharpMobileCode.ModalPicker;
using Foundation;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace Kunicardus.Touch
{
	public class RegistrationViewController : BaseRegistrationViewController
	{
		
		#region Properties

		public new iRegistrationViewModel ViewModel {
			get { return (iRegistrationViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		#endregion

		#region Ctors

		public RegistrationViewController () : base (ApplicationStrings.EnterYourPersonalInformation)
		{
			ScrollViewOnKeyboardShow = true;
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			InitUI ();
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			nfloat padding = 3;
			if (!Screen.IsTall) {
				padding = 0;
			}
			var firstName = new KuniTextField (
				                new CoreGraphics.CGRect (30, SubHeading.Frame.Bottom + (Screen.IsTall ? 15 : 3), View.Frame.Width - 60, 30), 
				                ApplicationStrings.FirstName,
				                UIKeyboardType.Default);

			var lastName = new KuniTextField (
				               new CoreGraphics.CGRect (30, firstName.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				               ApplicationStrings.LastName,
				               UIKeyboardType.Default);

			var personalId = new KuniTextField (
				                 new CoreGraphics.CGRect (30, lastName.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				                 ApplicationStrings.PersonalId,
				                 UIKeyboardType.NumberPad) { TextMaxLength = 11 };

			var birthday = new KuniTextField (
				               new CoreGraphics.CGRect (30, personalId.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				               ApplicationStrings.BirthDay,
				               UIKeyboardType.Default);

			var phone = new KuniTextField (
				            new CoreGraphics.CGRect (30, birthday.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				            ApplicationStrings.PhoneFormat,
				            UIKeyboardType.NumberPad){ TextMaxLength = 9 };

			var email = new KuniTextField (
				            new CoreGraphics.CGRect (30, phone.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				            ApplicationStrings.Email,
				            UIKeyboardType.EmailAddress);

			var password = new KuniTextField (
				               new CoreGraphics.CGRect (30, email.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				               ApplicationStrings.Password,
				               UIKeyboardType.Default);
			password.Field.SecureTextEntry = true;

			var confirmPassword = new KuniTextField (
				                      new CoreGraphics.CGRect (30, password.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				                      ApplicationStrings.ConfirmPassword,
				                      UIKeyboardType.Default);
			confirmPassword.Field.SecureTextEntry = true;

			var checkboxPadding = 30;
			var checkBoxHeight = 18f;
			var checkBox = UIButton.FromType (UIButtonType.Custom);
			checkBox.Frame = new CGRect (30, confirmPassword.Frame.Bottom + checkboxPadding + 5f, checkBoxHeight, checkBoxHeight);
			checkBox.SetImage (ImageHelper.MaxResizeImage (UIImage.FromFile ("checkbox_white_checked.png"), checkBoxHeight, 0), UIControlState.Selected);
			checkBox.SetImage (ImageHelper.MaxResizeImage (UIImage.FromFile ("checkbox_white_unchecked.png"), checkBoxHeight, 0), UIControlState.Normal);
			checkBox.TitleLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 6);
			checkBox.SetTitle (ApplicationStrings.ShowPassword, UIControlState.Normal);
			checkBox.SetTitleColor (UIColor.White, UIControlState.Normal);
			checkBox.SizeToFit ();
			checkBox.Frame = new CGRect (27, confirmPassword.Frame.Bottom + 8f, checkBox.Frame.Width + 10f, checkBoxHeight);
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

			PasswordHint hint = new PasswordHint (new CGRect (30, checkBox.Frame.Bottom + padding + (Screen.IsTall ? 10 : 9), View.Frame.Width - 60, 0));
			View.AddSubview (hint);
			// datepicker 
			var modalPicker = new ModalPickerViewController (ModalPickerType.Date, "Choose Date", this) {
				HeaderBackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen),
				HeaderTextColor = UIColor.White,
				TransitioningDelegate = new ModalPickerTransitionDelegate (),
				ModalPresentationStyle = UIModalPresentationStyle.Custom,
			};


			modalPicker.DatePicker.Mode = UIDatePickerMode.Date;
			modalPicker.DatePicker.MaximumDate = DateTime.Today.DateTimeToNSDate ();
			modalPicker.DatePicker.MinimumDate = DateTime.Today.AddYears (-100).DateTimeToNSDate ();
			modalPicker.OnModalPickerDismissed += (s, ea) => {
				var dateFormatter = new NSDateFormatter () {
					DateFormat = "dd/MM/yyyy"
				};
				birthday.Field.Text = dateFormatter.ToString (modalPicker.DatePicker.Date);
				((iRegistrationViewModel)ViewModel).DateOfBirth = modalPicker.DatePicker.Date.NSDateToDateTime ();
				phone.Field.BecomeFirstResponder ();
			};
			birthday.Field.ShouldBeginEditing += (sender) => { 
				birthday.Field.ResignFirstResponder ();
				PresentViewController (modalPicker, true, null);
				return false;
			};


			// prev nexts
			firstName.SetNextField (lastName);
			lastName.SetNextField (personalId);
			lastName.SetPrevField (firstName);
			personalId.SetNextField (birthday);
			personalId.SetPrevField (lastName);
			birthday.SetNextField (phone);
			birthday.SetPrevField (personalId);
			phone.SetNextField (email);
			phone.SetPrevField (birthday);
			email.SetNextField (password);
			email.SetPrevField (phone);
			password.SetNextField (confirmPassword);
			password.SetPrevField (email);
			confirmPassword.SetPrevField (password);
			// -------

			// next button init
			RegistrationNextButton next = 
				new RegistrationNextButton (UIButtonType.System);			
			next.Frame = new CGRect ((
			    View.Frame.Width - Styles.RegistrationNextButton.Width) / 2.0f,
				hint.Frame.Bottom + (Screen.IsTall ? 15 : 6), 
				Styles.RegistrationNextButton.Width, 
				Styles.RegistrationNextButton.Width);


			// Adding fields
			View.AddSubview (firstName);
			View.AddSubview (lastName);
			View.AddSubview (personalId);
			View.AddSubview (birthday);
			View.AddSubview (phone);
			View.AddSubview (email);
			View.AddSubview (password);
			View.AddSubview (confirmPassword);
			View.AddSubview (next);

			// Bindings
			this.CreateBinding (firstName.Field).To ((iRegistrationViewModel vm) => vm.Name).Apply ();
			this.CreateBinding (lastName.Field).To ((iRegistrationViewModel vm) => vm.Surname).Apply ();
			this.CreateBinding (personalId.Field).To ((iRegistrationViewModel vm) => vm.IdNumber).Apply ();
			this.CreateBinding (phone.Field).To ((iRegistrationViewModel vm) => vm.PhoneNumber).Apply ();
			this.CreateBinding (email.Field).To ((iRegistrationViewModel vm) => vm.Email).Apply ();
			this.CreateBinding (password.Field).To ((iRegistrationViewModel vm) => vm.Password).Apply ();
			this.CreateBinding (confirmPassword.Field).To ((iRegistrationViewModel vm) => vm.ConfirmPassword).Apply ();
			this.CreateBinding (next).To ((iRegistrationViewModel vm) => vm.RegisterUserCommand).Apply ();
		}

		#endregion
	}
}

