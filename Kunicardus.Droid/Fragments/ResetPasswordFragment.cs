
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross;
using Kuni.Core;
using Kunicardus.Droid.Views;
using Android.Content.PM;
using Android.Text.Method;
using System.Threading.Tasks;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Kunicardus.Droid.Fragments
{
	[Activity (Label = "Unicard", LaunchMode = LaunchMode.SingleTask)]
	public class ResetPasswordFragment : BaseMvxFragment
	{
		#region Private Variables

		BaseResetPasswordView _currentActivity;
		Android.Support.V4.App.FragmentManager _fragmentManager;
		BaseEditText _passwordEditText, _confirmPasswordEdittext;
		private int _selectionStart, _selectionEnd;
		private bool _confirmIsFocused;

		#endregion

		#region Constructor Implementation

		public ResetPasswordFragment ()
		{
			if (this.ViewModel == null)
                this.ViewModel = Mvx.IoCProvider.IoCConstruct<ResetPasswordViewModel>();
		}

		#endregion

		#region Native Methods

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.ResetPasswordView, null);
			_currentActivity = this.Activity as BaseResetPasswordView;
			_fragmentManager = _currentActivity.SupportFragmentManager;
			var toolbar = View.FindViewById <RelativeLayout> (Resource.Id.backbuttonToolbar);
			var backButtonLayout = toolbar.FindViewById<LinearLayout> (Resource.Id.back_button_layout);
	
			backButtonLayout.Click += (sender, e) => {
				this.Activity.OnBackPressed ();
			};
			var currentLayout = View.FindViewById<RelativeLayout> (Resource.Id.regRelativeLayout);
			currentLayout.Click += (sender, e) => {
				_currentActivity.HideKeyboard ();
			};

			var toolbarTitle = toolbar.FindViewById<BaseTextView> (Resource.Id.toolbar_title);
			toolbarTitle.Text = Resources.GetString (Resource.String.resetPassword);
			var parentViewModel = (this.Activity as BaseResetPasswordView).ViewModel as BaseResetPasswordViewModel;
			if (!string.IsNullOrWhiteSpace (parentViewModel.Email)) {
				(this.ViewModel as ResetPasswordViewModel).Email = parentViewModel.Email;
			}
			View.FindViewById<ImageButton> (Resource.Id.finish_registering_user).Click += (o, e) => {
				var vm = ((ResetPasswordViewModel)this.ViewModel);
				Task.Run (() => {
					vm.GetUserData ();
					if (!string.IsNullOrEmpty (vm.DisplayMessage)) {
						Toast.MakeText (_currentActivity, vm.DisplayMessage, ToastLength.Short).Show ();
					}
					if (vm.UserData == null) {
						return;
					}
					_currentActivity.Open (vm.UserData);
				});
			};
			var checkBox = View.FindViewById<CheckBox> (Resource.Id.toggle_password_checkbox);
			checkBox.CheckedChange += TogglePassoword;

			return View;
		}


		#endregion

		#region Receiving Message

		void TogglePassoword (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (_passwordEditText == null)
				_passwordEditText = _currentActivity.FindViewById<BaseEditText> (Resource.Id.reset_password_pasword);
			if (_confirmPasswordEdittext == null)
				_confirmPasswordEdittext = _currentActivity.FindViewById<BaseEditText> (Resource.Id.reset_password_confirm_pasword);

			if (_passwordEditText.HasFocus) {
				_selectionStart = _passwordEditText.SelectionStart;
				_selectionEnd = _passwordEditText.SelectionEnd;
				_confirmIsFocused = false;
			} else {
				_selectionStart = _confirmPasswordEdittext.SelectionStart;
				_selectionEnd = _confirmPasswordEdittext.SelectionEnd;
				_confirmIsFocused = true;
			}	

			if (e.IsChecked) {
				_passwordEditText.TransformationMethod = HideReturnsTransformationMethod.Instance;
				_confirmPasswordEdittext.TransformationMethod = HideReturnsTransformationMethod.Instance;
			} else {
				_passwordEditText.TransformationMethod = PasswordTransformationMethod.Instance;
				_confirmPasswordEdittext.TransformationMethod = PasswordTransformationMethod.Instance;
			}

			if (!_confirmIsFocused)
				_passwordEditText.SetSelection (_selectionStart, _selectionEnd);
			else
				_confirmPasswordEdittext.SetSelection (_selectionStart, _selectionEnd);
		}

		#endregion
	}
}