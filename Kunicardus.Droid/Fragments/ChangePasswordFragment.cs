using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross;
using Kuni.Core.ViewModels;
using Android.Views.InputMethods;
using Android.Text.Method;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;

namespace Kunicardus.Droid.Fragments
{
	public class ChangePasswordFragment : BaseMvxFragment
	{
		#region Private Variables

		private MvxFragmentActivity _mainView;
		private InputMethodManager imm;
		private IBinder _windowToken;
		BaseEditText _passwordEditText, _confirmPasswordEdittext;
		private int _selectionStart, _selectionEnd;
		private bool _confirmIsFocused;

		#endregion

		#region Properties

		private bool _passwordChanged;

		public bool PasswordChanged {
			get{ return _passwordChanged; }
			set {
				if (value) {
					HideKeyboard ();
					_mainView.OnBackPressed ();
				}
				_passwordChanged = value;
			}
		}

		#endregion

		#region Constructor Implementation

		public ChangePasswordFragment ()
		{
			if (this.ViewModel == null)
				this.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCProvider.IoCConstruct<ChangePasswordViewModel>();
		}

		#endregion

		#region Native Methods

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override void OnViewCreated (View View, Bundle savedInstanceState)
		{
			base.OnViewCreated (View, savedInstanceState);
			_windowToken = this.View.WindowToken;
			imm = (InputMethodManager)_mainView.GetSystemService	(
				Context.InputMethodService);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.ChangePasswordDialogLayout, null);
			_mainView = this.Activity as MainView;

			var rootView = View.RootView;
			rootView.Click += (sender, e) => {
				HideKeyboard ();
			};

			var backButton = View.FindViewById<ImageButton> (Resource.Id.change_password_back);
			backButton.Click += (sender, e) => {
				HideKeyboard ();
				_mainView.OnBackPressed ();
			};
			var showPasswordCheckbox = View.FindViewById<CheckBox> (Resource.Id.change_password_checkbox);
			showPasswordCheckbox.CheckedChange += TogglePassword;

			var set = this.CreateBindingSet<ChangePasswordFragment,ChangePasswordViewModel> ();
			set.Bind (this).For (v => v.PasswordChanged).To (vm => vm.PasswordChanged);
			set.Apply ();

			return View;
		}

		void TogglePassword (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			var old_password = _mainView.FindViewById<BaseEditText> (Resource.Id.old_password);
			_passwordEditText = _mainView.FindViewById<BaseEditText> (Resource.Id.new_password);
			_confirmPasswordEdittext = _mainView.FindViewById<BaseEditText> (Resource.Id.confirm_new_password);

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
				_passwordEditText.TransformationMethod = null;
				_confirmPasswordEdittext.TransformationMethod = null;
				old_password.TransformationMethod = null;
			} else {
				old_password.TransformationMethod = new PasswordTransformationMethod ();
				_passwordEditText.TransformationMethod = new PasswordTransformationMethod ();
				_confirmPasswordEdittext.TransformationMethod = new PasswordTransformationMethod ();
			}
			_passwordEditText.Invalidate ();
			_confirmPasswordEdittext.Invalidate ();
			if (!_confirmIsFocused)
				_passwordEditText.SetSelection (_selectionStart, _selectionEnd);
			else
				_confirmPasswordEdittext.SetSelection (_selectionStart, _selectionEnd);
		}

		#endregion

		#region implemented abstract members of BaseMvxFragment

		public override void OnActivate ()
		{
			//
		}

		#endregion

		#region Methods

		private void HideKeyboard ()
		{
			imm.HideSoftInputFromWindow (_windowToken, 0);
		}

		#endregion
	}
}