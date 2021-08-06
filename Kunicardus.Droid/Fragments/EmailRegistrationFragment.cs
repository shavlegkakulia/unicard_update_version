using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Kuni.Core;
using Android.Views.InputMethods;
using Android.Text.Method;
using MvvmCross.Binding.BindingContext;

namespace Kunicardus.Droid.Fragments
{
    public class EmailRegistrationFragment : BaseMvxFragment
    {
        #region Private variables

        private BaseRegisterView _activity;
        private BaseEditText _passwordEditText, _confirmPasswordEdittext;
        private int _selectionStart, _selectionEnd;
        private bool _confirmIsFocused;
        private EmailRegistrationViewModel _currentViewModel;

        #endregion

        #region Fragment native methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var View = base.GetAndInflateView(Resource.Layout.EmailRegistrationView);
            _currentViewModel = this.ViewModel as EmailRegistrationViewModel;

            var layout = View.FindViewById<RelativeLayout>(Resource.Id.regRelativeLayout);
            layout.Click += HideKeyboard;

            _activity = ((BaseRegisterView)base.Activity);

            var checkBox = View.FindViewById<CheckBox>(Resource.Id.email_toggle_password_checkbox);
            checkBox.CheckedChange += (sender, e) => TogglePassword(e.IsChecked);

            var backToolbar = View.FindViewById<RelativeLayout>(Resource.Id.backbuttonToolbar);
            var backLayout = backToolbar.FindViewById<LinearLayout>(Resource.Id.back_button_layout);
            backLayout.Click += BackClick;

            var toolbarTitle = backToolbar.FindViewById<BaseTextView>(Resource.Id.toolbar_title);
            toolbarTitle.Text = Resources.GetString(Resource.String.registration);

            if (Arguments != null)
            {
                var unicardNumber = Arguments.GetString("unicard_number");
                (this.ViewModel as EmailRegistrationViewModel).CardNumber = unicardNumber;
            }

            var set = this.CreateBindingSet<EmailRegistrationFragment,EmailRegistrationViewModel>();
            set.Bind(this).For(p => p.RegisrationCompleted).To(vm => vm.RegisrationCompleted);

            set.Apply();
            return View;
        }

        #endregion

        public bool RegisrationCompleted
        {
            get { return false; }
            set
            {
                if (value)
                {
                    var dialog = new Dialog(_activity,
                                     Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
                    dialog.SetContentView(Resource.Layout.RegistrationComplete);
                    dialog.SetCancelable(false);

                    dialog.FindViewById<BaseButton>(Resource.Id.btnOK).Click += (o, e) =>
                    {
                        _currentViewModel.RegistrationCompletedClick();
                        dialog.Dismiss();
                    };
                    dialog.Show();
                }
            }
        }

        #region Methods

        private void HideKeyboard(object sender, EventArgs e)
        {
            View View = base.Activity.CurrentFocus;
            if (View != null)
            {
                InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(View.WindowToken, HideSoftInputFlags.NotAlways);
            }
        }

        void BackClick(object sender, EventArgs e)
        {
            HideKeyboard(this, null);
            _activity.BackToPreviousFragment();
        }

        #endregion

        #region Retrieving messages

        private void TogglePassword(bool showPassword)
        {
            _passwordEditText = _activity.FindViewById<BaseEditText>(Resource.Id.mail_register_pasword);
            _confirmPasswordEdittext = _activity.FindViewById<BaseEditText>(Resource.Id.mail_register_confirm_pasword);


            if (_passwordEditText.HasFocus)
            {
                _selectionStart = _passwordEditText.SelectionStart;
                _selectionEnd = _passwordEditText.SelectionEnd;
                _confirmIsFocused = false;
            }
            else
            {
                _selectionStart = _confirmPasswordEdittext.SelectionStart;
                _selectionEnd = _confirmPasswordEdittext.SelectionEnd;
                _confirmIsFocused = true;
            }

            if (showPassword)
            {
                _passwordEditText.TransformationMethod = null;
                _confirmPasswordEdittext.TransformationMethod = null;
            }
            else
            {
                _passwordEditText.TransformationMethod = new PasswordTransformationMethod();
                _confirmPasswordEdittext.TransformationMethod = new PasswordTransformationMethod();
            }
            _passwordEditText.Invalidate();
            _confirmPasswordEdittext.Invalidate();
            if (!_confirmIsFocused)
                _passwordEditText.SetSelection(_selectionStart, _selectionEnd);
            else
                _confirmPasswordEdittext.SetSelection(_selectionStart, _selectionEnd);
        }

        #endregion
    }
}