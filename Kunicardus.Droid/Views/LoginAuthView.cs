using Android.App;
using Kuni.Core;
using Android.OS;
using Android.Content;
using Android.Widget;
using Android.Views;
using Android.Views.InputMethods;
using MvvmCross.Droid.Support.V4;

namespace Kunicardus.Droid.Views
{
    [Activity(Label = "Unicard",
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
        NoHistory = true,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginAuthView : MvxFragmentActivity
    {
        #region Native Methods

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoginAuthView);

            ////GAService.GetGASInstance().Track_App_Page(GAServiceHelper.Page.LoginAuthPage);

            var backToolbar = FindViewById<RelativeLayout>(Resource.Id.reset_password_back_button);

            var toolbarTitle = backToolbar.FindViewById<BaseTextView>(Resource.Id.toolbar_title);
            toolbarTitle.Text = Resources.GetString(Resource.String.authorization);
            var currentLayout = FindViewById<RelativeLayout>(Resource.Id.login_auth_view_layout);
            currentLayout.Click += (sender, e) =>
            {
                currentLayout.RequestFocus();
                HideKeyboard();
            };

            var btnLogin = FindViewById<BaseTextView>(Resource.Id.authorization);
            btnLogin.Click += (sender, args)=> (ViewModel as LoginAuthViewModel).Auth();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
        }

        private void HideKeyboard()
        {
            View View = CurrentFocus;
            if (View != null)
            {
                InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(View.WindowToken, HideSoftInputFlags.None);
            }
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(this, typeof(LoginView));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            this.Finish();
        }

        protected override void OnStop()
        {
            (this.ViewModel as LoginAuthViewModel).DismissDialog();
            HideKeyboard();
            base.OnStop();
        }

        #endregion
    }
}