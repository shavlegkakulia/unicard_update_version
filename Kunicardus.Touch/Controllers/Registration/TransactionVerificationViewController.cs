using System;
using Kunicardus.Core.ViewModels.iOSSpecific;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;
using SharpMobileCode.ModalPicker;

namespace Kunicardus.Touch
{
	public class TransactionVerificationViewController : BaseRegistrationViewController
	{

		#region Props

		public new iTransactionVerificationViewModel ViewModel {
			get { return (iTransactionVerificationViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		#endregion

		#region Ctors

		public TransactionVerificationViewController () : base (ApplicationStrings.EnterLastTransactionDetails)
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
			
			// Init radiobuttons -------------------------------------------------------
			nfloat radioButtonWidth = 30f;
			nfloat tmpRadioButtonLeft = 35f;
			var viewModel = (iTransactionVerificationViewModel)ViewModel;
			var merchants = viewModel.Merchants;
			if (merchants == null || merchants.Count != 4)
				return;
			
			RadioButton merchant1 = new RadioButton (
				                        new CGRect (tmpRadioButtonLeft, 
					                        SubHeading.Frame.Bottom + 20f,
					                        radioButtonWidth, 
					                        radioButtonWidth));
			merchant1.Value = merchants [0].MerchantId;

			RadioButton merchant2 = new RadioButton (
				                        new CGRect (tmpRadioButtonLeft, 
					                        merchant1.Frame.Bottom + 10f,
					                        radioButtonWidth, 
					                        radioButtonWidth));
			merchant2.Value = merchants [1].MerchantId;

			RadioButton merchant3 = new RadioButton (
				                        new CGRect (tmpRadioButtonLeft, 
					                        merchant2.Frame.Bottom + 10f,
					                        radioButtonWidth, 
					                        radioButtonWidth));
			merchant3.Value = merchants [2].MerchantId;

			RadioButton merchant4 = new RadioButton (
				                        new CGRect (tmpRadioButtonLeft, 
					                        merchant3.Frame.Bottom + 10f,
					                        radioButtonWidth, 
					                        radioButtonWidth));
			merchant4.Value = merchants [2].MerchantId;

			// Adding eventhandlers
			merchant1.Checked += delegate {
				merchant2.IsChecked = merchant3.IsChecked = merchant4.IsChecked = false;
				((iTransactionVerificationViewModel)ViewModel).SelectedMerchant = merchants [0];
			};
			merchant2.Checked += delegate {
				merchant1.IsChecked = merchant3.IsChecked = merchant4.IsChecked = false;
				((iTransactionVerificationViewModel)ViewModel).SelectedMerchant = merchants [1];
			};
			merchant3.Checked += delegate {
				merchant1.IsChecked = merchant2.IsChecked = merchant4.IsChecked = false;
				((iTransactionVerificationViewModel)ViewModel).SelectedMerchant = merchants [2];
			};
			merchant4.Checked += delegate {
				merchant1.IsChecked = merchant2.IsChecked = merchant3.IsChecked = false;
				((iTransactionVerificationViewModel)ViewModel).SelectedMerchant = merchants [3];
			};


			// Adding radiobuttons to view
			View.AddSubview (merchant1);
			View.AddSubview (merchant2);
			View.AddSubview (merchant3);
			View.AddSubview (merchant4);

			// End of init radiobuttons -------------------------------------------------------

			// Init radioButton texts   -------------------------------------------------------
			nfloat fontSizeForMerchantText = 16f;
			UILabel merchant1Text = new UILabel (
				                        new CGRect (
					                        merchant1.Frame.Right + 10f,
					                        merchant1.Frame.Top,
					                        View.Frame.Width - merchant1.Frame.Right - 20f,
					                        merchant1.Frame.Height
				                        ));
			merchant1Text.TextColor = UIColor.White;
			merchant1Text.TextAlignment = UITextAlignment.Left;
			merchant1Text.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, fontSizeForMerchantText);
			merchant1Text.Text = merchants [0].MerchantName;

			UILabel merchant2Text = new UILabel (
				                        new CGRect (
					                        merchant2.Frame.Right + 10f,
					                        merchant2.Frame.Top,
					                        View.Frame.Width - merchant2.Frame.Right - 20f,
					                        merchant2.Frame.Height
				                        ));
			merchant2Text.TextColor = UIColor.White;
			merchant2Text.TextAlignment = UITextAlignment.Left;
			merchant2Text.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, fontSizeForMerchantText);
			merchant2Text.Text = merchants [1].MerchantName;

			UILabel merchant3Text = new UILabel (
				                        new CGRect (
					                        merchant3.Frame.Right + 10f,
					                        merchant3.Frame.Top,
					                        View.Frame.Width - merchant3.Frame.Right - 20f,
					                        merchant3.Frame.Height
				                        ));
			merchant3Text.TextColor = UIColor.White;
			merchant3Text.TextAlignment = UITextAlignment.Left;
			merchant3Text.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, fontSizeForMerchantText);
			merchant3Text.Text = merchants [2].MerchantName;

			UILabel merchant4Text = new UILabel (
				                        new CGRect (
					                        merchant4.Frame.Right + 10f,
					                        merchant4.Frame.Top,
					                        View.Frame.Width - merchant4.Frame.Right - 20f,
					                        merchant4.Frame.Height
				                        ));
			merchant4Text.TextColor = UIColor.White;
			merchant4Text.TextAlignment = UITextAlignment.Left;
			merchant4Text.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, fontSizeForMerchantText);
			merchant4Text.Text = merchants [3].MerchantName;


			// adding texts to view
			View.AddSubview (merchant1Text);
			View.AddSubview (merchant2Text);
			View.AddSubview (merchant3Text);
			View.AddSubview (merchant4Text);
			// End of init radiobutton texts ---------------------------------------------------

			// Init amount
			KuniTextField amount = new KuniTextField (
				                       new CoreGraphics.CGRect (tmpRadioButtonLeft, merchant4.Frame.Bottom + 15, 95f, 30f), 
				                       ApplicationStrings.Amount,
				                       UIKeyboardType.DecimalPad);
			View.AddSubview (amount);

			KuniTextField date = new KuniTextField (
				                     new CoreGraphics.CGRect (View.Frame.Width - 95f - tmpRadioButtonLeft, 
					                     merchant4.Frame.Bottom + 15, 95f, 30f), 
				                     ApplicationStrings.Date,
				                     UIKeyboardType.DecimalPad);
			View.AddSubview (date);


			// Datepicker
			var modalPicker = new ModalPickerViewController (ModalPickerType.Date, "Choose Date", this) {
				HeaderBackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen),
				HeaderTextColor = UIColor.White,
				TransitioningDelegate = new ModalPickerTransitionDelegate (),
				ModalPresentationStyle = UIModalPresentationStyle.Custom,
			};


			modalPicker.DatePicker.Mode = UIDatePickerMode.Date;
			modalPicker.DatePicker.MaximumDate = DateTime.Today.DateTimeToNSDate ();
			modalPicker.DatePicker.MinimumDate = DateTime.Today.AddYears (-10).DateTimeToNSDate ();
			modalPicker.OnModalPickerDismissed += (s, ea) => {
				var dateFormatter = new NSDateFormatter () {
					DateFormat = "dd/MM/yyyy"
				};

				date.Field.Text = dateFormatter.ToString (modalPicker.DatePicker.Date);
				((iTransactionVerificationViewModel)ViewModel).Date = modalPicker.DatePicker.Date.NSDateToDateTime ();
				date.Field.ResignFirstResponder ();
			};
			date.Field.ShouldBeginEditing += (sender) => { 
				date.Field.ResignFirstResponder ();
				PresentViewController (modalPicker, true, null);
				return false;
			};


			// Init next button
			RegistrationNextButton next = 
				new RegistrationNextButton (UIButtonType.System);			
			next.Frame = new CGRect ((
			    View.Frame.Width - Styles.RegistrationNextButton.Width) / 2.0f,
				View.Frame.Height - Styles.RegistrationNextButton.Width - Styles.RegistrationNextButton.bottom, 
				Styles.RegistrationNextButton.Width, 
				Styles.RegistrationNextButton.Width);
			View.AddSubview (next);

			// Set bindings
			this.CreateBinding (amount.Field).To ((iTransactionVerificationViewModel vm) => vm.Price).Apply ();
			this.CreateBinding (next).To ((iTransactionVerificationViewModel vm) => vm.ContinueCommand).Apply ();
		}

		#endregion
	}
}

