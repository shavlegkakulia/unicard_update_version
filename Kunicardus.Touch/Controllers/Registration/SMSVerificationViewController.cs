using System;
using Kunicardus.Core.ViewModels.iOSSpecific;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreGraphics;
using Foundation;

namespace Kunicardus.Touch
{
	public class SMSVerificationViewController : BaseRegistrationViewController
	{
		public new iSMSVerificationViewModel ViewModel {
			get { return (iSMSVerificationViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public SMSVerificationViewController () : base (ApplicationStrings.UWillGetSMSOnNumberBellow)
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			InitUI ();
		}

		private void InitUI ()
		{
			UILabel sms = new UILabel ();
			sms.TextColor = UIColor.Clear.FromHexString (Styles.Colors.PlaceHolderColor);
			sms.Font = UIFont.SystemFontOfSize (22);
			sms.TextAlignment = UITextAlignment.Center;
			sms.Frame = new CoreGraphics.CGRect (0, SubHeading.Frame.Bottom + 3, View.Frame.Width, 30);
			View.AddSubview (sms);

			UserNumberHint userNumberHint = new UserNumberHint (new CGRect (30, sms.Frame.Bottom + 5, View.Frame.Width - 60, 0));
			if (ViewModel.PhoneNumberRetrieved)
				View.AddSubview (userNumberHint);

			RegistrationNextButton next = 
				new RegistrationNextButton (UIButtonType.System);			
			next.Frame = new CGRect ((
			    View.Frame.Width - Styles.RegistrationNextButton.Width) / 2.0f,
				View.Frame.Height - Styles.RegistrationNextButton.Width - Styles.RegistrationNextButton.bottom, 
				Styles.RegistrationNextButton.Width, 
				Styles.RegistrationNextButton.Width);
			View.AddSubview (next);

			var smsCode = new KuniTextField (
				              new CoreGraphics.CGRect (30, userNumberHint.Frame.Bottom + 15, View.Frame.Width - 60, 30), 
				              ApplicationStrings.EnterSMSCode,
				              UIKeyboardType.NumberPad){ TextMaxLength = 4 };
			smsCode.Field.TextAlignment = UITextAlignment.Center;
			smsCode.Field.EditingChanged += delegate {				
				if (smsCode.Field.IsFirstResponder && smsCode.Field.Text.Length == 4) {
					smsCode.Field.ResignFirstResponder ();
				}
			};		
			smsCode.Field.BecomeFirstResponder ();
			View.AddSubview (smsCode);

		


			UIButton resend = new UIButton (UIButtonType.System);
			resend.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 4);
			var underlineregistration = new NSAttributedString (
				                            ApplicationStrings.ResendSMS, 
				                            underlineStyle: NSUnderlineStyle.Single, foregroundColor: UIColor.Clear.FromHexString (Styles.Colors.Yellow));
			resend.SetAttributedTitle (underlineregistration, UIControlState.Normal);
			resend.SizeToFit ();
			resend.Frame = 
				new CGRect ((View.Frame.Width - resend.Frame.Width) / 2.0f,
				smsCode.Frame.Bottom + 5,
				resend.Frame.Width,
				resend.Frame.Height);
			View.AddSubview (resend);

			SMSVerificationHint smsVerificationHint = new SMSVerificationHint (new CGRect (30, resend.Frame.Bottom + 5, View.Frame.Width - 60, 0));
			View.AddSubview (smsVerificationHint);

			this.CreateBinding (sms).To ((iSMSVerificationViewModel vm) => vm.PhoneNumberFormated).Apply ();
			this.CreateBinding (smsCode.Field).To ((iSMSVerificationViewModel vm) => vm.VerificationCode).Apply ();
			this.CreateBinding (next).To ((iSMSVerificationViewModel vm) => vm.ContinueCommand).Apply ();
			this.CreateBinding (resend).To ((iSMSVerificationViewModel vm) => vm.Resend).Apply ();
		}
	}
}

