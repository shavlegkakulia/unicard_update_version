using System;
using Kunicardus.Core.ViewModels.iOSSpecific;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace Kunicardus.Touch
{
	public class UnicardNumberInputViewController : BaseRegistrationViewController
	{
		public new iUnicardNumberInputViewModel ViewModel {
			get { return (iUnicardNumberInputViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public UnicardNumberInputViewController () : base (ApplicationStrings.EnterUnicardNumber)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			InitUI ();
		}

		private void InitUI ()
		{
			
			nfloat top = 200f;
			if (!Screen.IsTall) {
				top = 180f;
			}
			nfloat leftRightPadding = 30f;
			nfloat width = 55f;
			nfloat height = 30f;
			nfloat innerPadding = (View.Frame.Width - (2f * leftRightPadding) - (4f * width)) / 3.0f; 

			KuniTextField part1 = new KuniTextField (
				                      new CoreGraphics.CGRect (leftRightPadding, top, width, height), 
				                      string.Empty,
				                      UIKeyboardType.NumberPad){ TextMaxLength = 4 };
			part1.Field.TextAlignment = UITextAlignment.Center;

			KuniTextField part2 = new KuniTextField (
				                      new CoreGraphics.CGRect (part1.Frame.Right + innerPadding, top, width, height), 
				                      string.Empty,
				                      UIKeyboardType.NumberPad){ TextMaxLength = 4 };
			part2.Field.TextAlignment = UITextAlignment.Center;

			KuniTextField part3 = new KuniTextField (
				                      new CoreGraphics.CGRect (part2.Frame.Right + innerPadding, top, width, height), 
				                      string.Empty,
				                      UIKeyboardType.NumberPad){ TextMaxLength = 4 };
			part3.Field.TextAlignment = UITextAlignment.Center;

			KuniTextField part4 = new KuniTextField (
				                      new CoreGraphics.CGRect (part3.Frame.Right + innerPadding, top, width, height), 
				                      string.Empty,
				                      UIKeyboardType.NumberPad){ TextMaxLength = 4 };
			part4.Field.TextAlignment = UITextAlignment.Center;

			// Configuring next and prev fields
			part1.SetNextField (part2);
			part2.SetNextField (part3);
			part2.SetPrevField (part1);
			part3.SetNextField (part4);
			part3.SetPrevField (part2);
			part4.SetPrevField (part3);

			part1.Field.EditingChanged += delegate {
				if (part1.Field.IsFirstResponder && part1.Field.Text.Length == 4) {
					part1.Field.ResignFirstResponder ();
					part2.Field.BecomeFirstResponder ();
				}
			};
			part2.Field.EditingChanged += delegate {
				if (part2.Field.IsFirstResponder && part2.Field.Text.Length == 4) {
					part2.Field.ResignFirstResponder ();
					part3.Field.BecomeFirstResponder ();
				}
				if (part2.Field.IsFirstResponder && part2.Field.Text.Length == 0) {
					part2.Field.ResignFirstResponder ();
					part1.Field.BecomeFirstResponder ();
				}
			};
			part3.Field.EditingChanged += delegate {
				if (part3.Field.IsFirstResponder && part3.Field.Text.Length == 4) {
					part3.Field.ResignFirstResponder ();
					part4.Field.BecomeFirstResponder ();
				}
				if (part3.Field.IsFirstResponder && part3.Field.Text.Length == 0) {
					part3.Field.ResignFirstResponder ();
					part2.Field.BecomeFirstResponder ();
				}
			};
			part4.Field.EditingChanged += delegate {
				if (part4.Field.IsFirstResponder && part4.Field.Text.Length == 0) {
					part4.Field.ResignFirstResponder ();
					part3.Field.BecomeFirstResponder ();
				}
				if (part4.Field.IsFirstResponder && part4.Field.Text.Length == 4) {
					part4.Field.ResignFirstResponder ();
				}
			};				

			RegistrationNextButton next = 
				new RegistrationNextButton (UIButtonType.System);			
			next.Frame = new CGRect ((
			    View.Frame.Width - Styles.RegistrationNextButton.Width) / 2.0f,
				View.Frame.Height - Styles.RegistrationNextButton.Width - Styles.RegistrationNextButton.bottom, 
				Styles.RegistrationNextButton.Width, 
				Styles.RegistrationNextButton.Width);


			// Bindings
			this.CreateBinding (part1.Field).To ((iUnicardNumberInputViewModel vm) => vm.Part1).Apply ();
			this.CreateBinding (part2.Field).To ((iUnicardNumberInputViewModel vm) => vm.Part2).Apply ();
			this.CreateBinding (part3.Field).To ((iUnicardNumberInputViewModel vm) => vm.Part3).Apply ();
			this.CreateBinding (part4.Field).To ((iUnicardNumberInputViewModel vm) => vm.Part4).Apply ();
			this.CreateBinding (next).To ((iUnicardNumberInputViewModel vm) => vm.ContinueCommand).Apply ();

			View.AddSubview (part1);
			View.AddSubview (part2);
			View.AddSubview (part3);
			View.AddSubview (part4);
			View.AddSubview (next);
		}
	}
}

