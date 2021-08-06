using System;
using UIKit;
using Kunicardus.Core.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using CoreGraphics;
using Foundation;
using Kunicardus.Touch.Helpers.UI;
using MonoTouch.Dialog;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;

namespace Kunicardus.Touch
{
	public class ChangePasswordViewController : BaseMvxViewController
	{
		#region Properties

		public new ChangePasswordViewModel ViewModel {
			get { return (ChangePasswordViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		private bool _passwordChanged;

		public bool PasswordChanged {
			get {
				return _passwordChanged;
			}
			set { 
				if (value) {
					NavigationController.PopToRootViewController (false);
				}
				_passwordChanged = value;
			}
		}

		#endregion

		#region Constructor Implementation

		public ChangePasswordViewController ()
		{
			HideMenuIcon = true;
			Title = ApplicationStrings.ChangePassword;
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff");
			InitUI ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
			NavigationController.NavigationBar.Translucent = false;
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationController.NavigationBarHidden = true;
			base.ViewWillDisappear (animated);
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			var startPoint = 20f;
			var textFieldHeight = 35f;
			nfloat padding = 7f, leftPaddingValue = 15f;

			var placeholderSize = UIFont.LabelFontSize - 2;
			UIView leftPadding = new UIView (new CGRect (0, 0, leftPaddingValue, textFieldHeight));
			var oldPassword = new UITextField (new CGRect (30, startPoint, View.Frame.Width - 60, textFieldHeight));
			oldPassword.Layer.BorderColor = UIColor.LightGray.CGColor;
			oldPassword.Layer.BorderWidth = 1f;
			oldPassword.Layer.CornerRadius = 3f;
			oldPassword.AttributedPlaceholder = new NSAttributedString (ApplicationStrings.OldPassword, 
				UIFont.FromName (Styles.Fonts.BPGExtraSquare, placeholderSize), 
				UIColor.Clear.FromHexString (Styles.Colors.Gray));
			oldPassword.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			oldPassword.LeftView = leftPadding;


			oldPassword.LeftViewMode = UITextFieldViewMode.Always;
			oldPassword.SecureTextEntry = true;
			oldPassword.TintColor = UIColor.Black;

			leftPadding = new UIView (new CGRect (0, 0, leftPaddingValue, textFieldHeight));
			startPoint += textFieldHeight + (float)padding;
			var newPassword = new UITextField (new CGRect (30, startPoint, View.Frame.Width - 60, textFieldHeight));
			newPassword.Layer.BorderColor = UIColor.LightGray.CGColor;
			newPassword.Layer.BorderWidth = 1f;
			newPassword.Layer.CornerRadius = 3f;
			newPassword.AttributedPlaceholder = new NSAttributedString (ApplicationStrings.NewPassword, 
				UIFont.FromName (Styles.Fonts.BPGExtraSquare, placeholderSize), 
				UIColor.Clear.FromHexString (Styles.Colors.Gray));
			newPassword.LeftView = leftPadding;
			newPassword.LeftViewMode = UITextFieldViewMode.Always;
			newPassword.SecureTextEntry = true;
			newPassword.TintColor = UIColor.Black;

			leftPadding = new UIView (new CGRect (0, 0, leftPaddingValue, textFieldHeight));
			startPoint += textFieldHeight + (float)padding;
			var confirmNewPassword = new UITextField (new CGRect (30, startPoint, View.Frame.Width - 60, textFieldHeight));
			confirmNewPassword.Layer.BorderColor = UIColor.LightGray.CGColor;
			confirmNewPassword.Layer.BorderWidth = 1f;
			confirmNewPassword.Layer.CornerRadius = 3f;
			confirmNewPassword.AttributedPlaceholder = new NSAttributedString (ApplicationStrings.ConfirmPassword, 
				UIFont.FromName (Styles.Fonts.BPGExtraSquare, placeholderSize), 
				UIColor.Clear.FromHexString (Styles.Colors.Gray));
			confirmNewPassword.LeftView = leftPadding;
			confirmNewPassword.LeftViewMode = UITextFieldViewMode.Always;
			confirmNewPassword.SecureTextEntry = true;
			confirmNewPassword.TintColor = UIColor.Black;

			var checkBoxHeight = 18f;
			startPoint += checkBoxHeight + (float)padding + 15f;
			var checkBox = UIButton.FromType (UIButtonType.Custom);
			checkBox.Frame = new CGRect (30, startPoint + padding, checkBoxHeight, checkBoxHeight);
			checkBox.SetImage (ImageHelper.MaxResizeImage (UIImage.FromFile ("checkbox_green_checked.png"), checkBoxHeight, 0), UIControlState.Selected);
			checkBox.SetImage (ImageHelper.MaxResizeImage (UIImage.FromFile ("checkbox_green_unchecked.png"), checkBoxHeight, 0), UIControlState.Normal);
			checkBox.TitleLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 6);
			checkBox.SetTitle (ApplicationStrings.ShowPassword, UIControlState.Normal);
			checkBox.SetTitleColor (UIColor.Black, UIControlState.Normal);
			checkBox.SizeToFit ();
			checkBox.Frame = new CGRect (27, startPoint + padding, checkBox.Frame.Width + 10f, checkBoxHeight);
			checkBox.TitleEdgeInsets = new UIEdgeInsets (0, 10, 0, 0);

			checkBox.TouchUpInside += (sender, e) => {
				checkBox.Selected = !checkBox.Selected;
				if (checkBox.Selected) {
					oldPassword.SecureTextEntry = false;
					newPassword.SecureTextEntry = false;
					confirmNewPassword.SecureTextEntry = false;
				} else {
					oldPassword.SecureTextEntry = true;
					newPassword.SecureTextEntry = true;
					confirmNewPassword.SecureTextEntry = true;
				}
			};

			var buttonHeight = 45f;
			startPoint += textFieldHeight + 2;

			UILabel hint = new UILabel (new CGRect (30, startPoint, View.Frame.Width - 60, 0));
			hint.Lines = 0;
			hint.LineBreakMode = UILineBreakMode.WordWrap;
			hint.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 8);
			hint.Text = ApplicationStrings.PasswordHint;
			hint.TextColor = UIColor.Gray;
			hint.SizeToFit ();

			startPoint = (float)hint.Frame.Bottom + 8;

			var changeButton = new UIButton (UIButtonType.System);
			changeButton.Frame = new CGRect (30, startPoint + padding, View.Frame.Width - 60, buttonHeight);
			changeButton.SetTitle (ApplicationStrings.Change, UIControlState.Normal);
			changeButton.BackgroundColor = UIColor.Clear.FromHexString ("#8DBD3B");
			changeButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			changeButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 2);
			changeButton.Layer.CornerRadius = 23;
			changeButton.DropShadowDependingOnBGColor ();


			View.AddSubview (checkBox);
			View.AddSubview (hint);
			View.AddSubview (changeButton);
			View.AddSubview (oldPassword);
			View.AddSubview (newPassword);
			View.AddSubview (confirmNewPassword);

			var set = this.CreateBindingSet<ChangePasswordViewController,ChangePasswordViewModel> ();
			set.Bind (changeButton).To (vm => vm.ChangePasswordCommand);
			set.Bind (oldPassword).To (vm => vm.OldPassword);
			set.Bind (newPassword).To (vm => vm.NewPassword);
			set.Bind (confirmNewPassword).To (vm => vm.ConfirmNewPassword);
			set.Bind (this).For (v => v.PasswordChanged).To (vm => vm.PasswordChanged);
			set.Apply ();
		}

		#endregion
	}
}

