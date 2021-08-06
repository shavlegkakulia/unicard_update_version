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
	public class FacebookRegistrationViewController : BaseRegistrationViewController
	{
		public new iFacebookRegistrationViewModel ViewModel {
			get { return (iFacebookRegistrationViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public FacebookRegistrationViewController () : base (ApplicationStrings.EnterYourPersonalInformation)
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
			nfloat padding = 6;
			if (!Screen.IsTall) {
				padding = 3;
			}

			var fullname = new KuniTextField (
				               new CoreGraphics.CGRect (30, SubHeading.Frame.Bottom + (Screen.IsTall ? 15 : 10), View.Frame.Width - 60, 30), 
				               ApplicationStrings.FullName,
				               UIKeyboardType.Default);
			var email = new KuniTextField (
				            new CoreGraphics.CGRect (30, fullname.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
				            ApplicationStrings.Email,
				            UIKeyboardType.EmailAddress);
			email.Field.Enabled = false;
			email.Field.TextColor = UIColor.Gray;

			var personalId = new KuniTextField (
				                 new CoreGraphics.CGRect (30, email.Frame.Bottom + padding, View.Frame.Width - 60, 30), 
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
				((iFacebookRegistrationViewModel)ViewModel).DateOfBirth = modalPicker.DatePicker.Date.NSDateToDateTime ();
				phone.Field.BecomeFirstResponder ();
			};
			birthday.Field.ShouldBeginEditing += (sender) => { 
				birthday.Field.ResignFirstResponder ();
				PresentViewController (modalPicker, true, null);
				return false;
			};

			// prev nexts
			fullname.SetNextField (personalId);
			personalId.SetNextField (birthday);
			personalId.SetPrevField (fullname);
			birthday.SetNextField (phone);
			birthday.SetPrevField (personalId);
			phone.SetPrevField (birthday);

			// next button init
			RegistrationNextButton next = 
				new RegistrationNextButton (UIButtonType.System);			
			next.Frame = new CGRect ((
			    View.Frame.Width - Styles.RegistrationNextButton.Width) / 2.0f,
				View.Frame.Height - Styles.RegistrationNextButton.Width - Styles.RegistrationNextButton.bottom, 
				Styles.RegistrationNextButton.Width, 
				Styles.RegistrationNextButton.Width);

			// Adding fields
			View.AddSubview (fullname);
			View.AddSubview (personalId);
			View.AddSubview (birthday);
			View.AddSubview (phone);
			View.AddSubview (email);
			View.AddSubview (next);

			// Bindings
			this.CreateBinding (fullname.Field).To ((iFacebookRegistrationViewModel vm) => vm.FullName).Apply ();
			this.CreateBinding (email.Field).To ((iFacebookRegistrationViewModel vm) => vm.Email).Apply ();
			this.CreateBinding (personalId.Field).To ((iFacebookRegistrationViewModel vm) => vm.IdNumber).Apply ();
			this.CreateBinding (phone.Field).To ((iFacebookRegistrationViewModel vm) => vm.PhoneNumber).Apply ();
			this.CreateBinding (next).To ((iFacebookRegistrationViewModel vm) => vm.ContinueCommand).Apply ();
		}
	}
}

