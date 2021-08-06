using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Content;
using Android.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Support.V4.App;
using Kunicardus.Billboards.Core.ViewModels;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Util;
using Autofac;


namespace Kunicardus.Billboards.Activities
{
    [Activity(Label = "Unicard", NoHistory = true,
		LaunchMode = Android.Content.PM.LaunchMode.SingleTop, 
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
		Theme = "@android:style/Theme.NoTitleBar.Fullscreen",
		WindowSoftInputMode = Android.Views.SoftInput.StateAlwaysVisible, Exported = true)]
	public class LoginAuthView : FragmentActivity
	{
        AuthViewModel _viewModel;
        ProgressDialog _progressDialog;

		#region Native Methods

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoginAuthView);

            using (var scope = App.Container.BeginLifetimeScope())
            {
                _viewModel = scope.Resolve<AuthViewModel>();
            }

            var backToolbar = FindViewById<RelativeLayout>(Resource.Id.reset_password_back_button);
            var back = FindViewById<ImageButton>(Resource.Id.back);
            back.Click += delegate
            {
                back.RequestFocus();
                HideKeyboard();
                Intent intent = new Intent(this, typeof(LoginView));
                intent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
                this.Finish();
            };

            var username = FindViewById<TextView>(Resource.Id.txtUsername);
            var password = FindViewById<TextView>(Resource.Id.txtPassword);

#if DEBUG
            username.Text = "smamuchishvili@gmail.com";
            password.Text = "bulvari111";
#else
#endif

            var resetPassword = FindViewById<TextView>(Resource.Id.resetPassword);
            resetPassword.Click += delegate
            {
                try
                {
                    var uri = Android.Net.Uri.Parse("http://unicard.ge/ge/reset");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                    //PackageInfo packageInfo = PackageManager.GetPackageInfo("com.wandio.unicard", PackageInfoFlags.Activities);
                    //var activitiesInfos = packageInfo.Activities.ToList();
                    //var activity = activitiesInfos.Where(x => x.Name.Contains("BaseResetPasswordView")).FirstOrDefault();
                    //
                    //Intent intent = PackageManager.GetLaunchIntentForPackage(packageInfo.PackageName);
                    //if (intent == null)
                    //{
                    //    // Bring user to the market or let them choose an app?
                    //    intent = new Intent(Intent.ActionView);
                    //    intent.SetData(Android.Net.Uri.Parse("market://details?id=" + packageInfo.PackageName));
                    //}
                    //intent.SetComponent(new ComponentName(packageInfo.PackageName, activity.Name));
                    //intent.AddFlags(ActivityFlags.NewTask);
                    //StartActivity(intent);
                }
                catch (Exception ex)
                {
                    Log.Debug("Auth Activity: ", ex.ToString());
                    Toast toast = Toast.MakeText(this, Resource.String.no_baseApp, ToastLength.Short);
                    TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                    if (v != null)
                        v.Gravity = GravityFlags.Center;
                    toast.Show();
                }
            };

            var authButton = FindViewById<TextView>(Resource.Id.btnAuth);
            authButton.Click += (o, e) =>
            {
                _progressDialog = ProgressDialog.Show(this, null, "მიმდინარეობს ავტორიზაცია...");
                Task.Run(() =>
                {
                    var response = _viewModel.Auth(username.Text, password.Text);
                    RunOnUiThread(() =>
                    {
                        if (response)
                        {
                            Intent intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                            this.Finish();
                        }
                        else
                        {
                            _progressDialog.Dismiss();
                            if (!string.IsNullOrEmpty(_viewModel.DisplayMessage))
                            {
                                Toast.MakeText(this, _viewModel.DisplayMessage, ToastLength.Short).Show();
                            }
                        }
                    });
                });
            };

            var toolbarTitle = backToolbar.FindViewById<BaseTextView>(Resource.Id.toolbar_title);
            toolbarTitle.Text = Resources.GetString(Resource.String.authorization);
            var currentLayout = FindViewById<RelativeLayout>(Resource.Id.login_auth_view_layout);
            currentLayout.Click += (sender, e) =>
            {
                currentLayout.RequestFocus();
                HideKeyboard();
			};
		}

        private void HideKeyboard()
		{
			View view = CurrentFocus;
            if (view != null)
            {
                InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);
			}
		}

        protected override void OnDestroy()
        {
            if (_progressDialog!=null)
            {
                _progressDialog.Dismiss();
            }
            base.OnDestroy();
        }

        public override void OnBackPressed()
		{
            Intent intent = new Intent(this, typeof(LoginView));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            this.Finish();
		}
		#endregion
	}
}