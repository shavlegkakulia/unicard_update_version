using System;
using System.Linq;
using Android.App;
using Xamarin.Facebook;
using Xamarin.Facebook.Share.Widget;
using Xamarin.Facebook.Share;
using Java.Interop;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.AppEvents;
using Android.OS;
using Android.Content;
using Android.Widget;
using Kunicardus.Billboards.Core.ViewModels;
using Kunicardus.Billboards.Activities;
using Android.Content.PM;
using Android.Views;
using Autofac;
using Android.Text;
using Android.Text.Style;

namespace Kunicardus.Billboards
{
    [Activity(Label = "UNIBOARD", NoHistory = false, LaunchMode = Android.Content.PM.LaunchMode.SingleTask, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class LoginView : Activity, GraphRequest.IGraphJSONObjectCallback
    {
        LoginViewModel _viewModel;

        public void OnCompleted(Org.Json.JSONObject p0, GraphResponse p1)
        {

            try
            {
                var email = p1.JSONObject.GetString("email");
                var name = p1.JSONObject.GetString("first_name");
                var lastName = p1.JSONObject.GetString("last_name");
                var loggedIn = _viewModel.FacebookConnect(name, lastName, email, AccessToken.CurrentAccessToken.Token);
                if (!string.IsNullOrEmpty(loggedIn.DisplayMessage))
                {
                    Toast toast = Toast.MakeText(this, loggedIn.DisplayMessage, ToastLength.Long);
                    TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                    if (v != null)
                        v.Gravity = GravityFlags.Center;
                    toast.Show();
                }
                if (loggedIn.Success)
                {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    this.Finish();
                }
                else
                {
                    //Toast.MakeText(this, "თქვენ არ ხართ რეგისტრირებული უნიქარდის სისტემაში. რეგისტრაციის გასავლელად ეწვიეთ ვებ-გვერდს ან გადმოწერეთ უნიქარდის აპლიკაცის", ToastLength.Long).Show();
                    RunOnUiThread(() =>
                    {
                        if (_progressDialog != null)
                        {
                            _progressDialog.Dismiss();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Toast toast = Toast.MakeText(this, Resource.String.no_fbLogin, ToastLength.Short);
                TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                if (v != null)
                    v.Gravity = GravityFlags.Center;
                toast.Show();
            }
            //var loginViewModel = this.ViewModel as LoginViewModel;
            //if (!(ViewModel as LoginViewModel).Completed) {
            //	loginViewModel.FacebookConnect (name, lastName, email, profile.Id);
            //	(ViewModel as LoginViewModel).Completed = true;
            //}
        }

        const String PENDING_ACTION_BUNDLE_KEY = "com.facebook.samples.hellofacebook:PendingAction";
        PendingAction pendingAction = PendingAction.NONE;

        private readonly string[] EXTENDED_PERMISSIONS = new[] { "email" };
        ICallbackManager callbackManager;
        ProfileTracker profileTracker;
        ShareDialog shareDialog;
        FacebookCallback<SharerResult> shareCallback;
        Profile profile;
        ProgressDialog _progressDialog;

        enum PendingAction
        {
            NONE,
            POST_PHOTO,
            POST_STATUS_UPDATE
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            FacebookSdk.SdkInitialize(this);
            using (var scope = App.Container.BeginLifetimeScope())
            {
                _viewModel = scope.Resolve<LoginViewModel>();
            }
            //PackageInfo info = this.PackageManager.GetPackageInfo("com.wandio.unicard.billboards", PackageInfoFlags.Signatures);

            //foreach (Android.Content.PM.Signature signature in info.Signatures)
            //{
            //    MessageDigest md = MessageDigest.GetInstance("SHA");
            //    md.Update(signature.ToByteArray());
            //
            //    string keyhash = Convert.ToBase64String(md.Digest());
            //    Console.WriteLine("KeyHash:", keyhash);
            //}


            //_viewModel.Completed = false;
            callbackManager = CallbackManagerFactory.Create();

            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult =>
                {
                    RunOnUiThread(() =>
                    {
                        _progressDialog = ProgressDialog.Show(this, null, "მიმდინარეობს ავტორიზაცია...");
                    });
                    HandlePendingAction();
                    UpdateUI();
                },
                HandleCancel = () =>
                {
                    if (pendingAction != PendingAction.NONE)
                    {
                        pendingAction = PendingAction.NONE;
                    }
                    UpdateUI();
                },
                HandleError = loginError =>
                {
                    if (pendingAction != PendingAction.NONE
                                       && loginError is FacebookAuthorizationException)
                    {
                        pendingAction = PendingAction.NONE;
                    }
                    UpdateUI();
                }
            };
            LoginManager.Instance.LogOut();
            LoginManager.Instance.RegisterCallback(callbackManager, loginCallback);

            shareCallback = new FacebookCallback<SharerResult>
            {
                HandleSuccess = shareResult =>
                {
                    Console.WriteLine("Facebook: Success!");

                    if (shareResult.PostId != null)
                    {
                        var id = shareResult.PostId;

                    }
                },
                HandleError = shareError =>
                {
                    Console.WriteLine("Facebook: Error: {0}", shareError);
                }
            };

            shareDialog = new ShareDialog(this);
            shareDialog.RegisterCallback(callbackManager, shareCallback);


            SetContentView(Resource.Layout.LoginView);


            var txt = Resources.GetString(Resource.String.loginWarning);
            var text = FindViewById<TextView>(Resource.Id.warning);
            text.TextFormatted = Html.FromHtml(txt);

            var authButton = FindViewById<TextView>(Resource.Id.btnAuth);
            authButton.Click += (o, e) =>
            {
                Intent intent = new Intent(this, typeof(LoginAuthView));
                StartActivity(intent);
                this.Finish();
            };

            var registration = FindViewById<TextView>(Resource.Id.register);
            registration.Click += delegate
            {
                try
                {
                    var uri = Android.Net.Uri.Parse("http://unicard.ge/ge/registration");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                    
                    //PackageInfo packageInfo = PackageManager.GetPackageInfo("com.wandio.unicard", PackageInfoFlags.Activities);
                    //var activitiesInfos = packageInfo.Activities.ToList();
                    //var activity = activitiesInfos.Where(x => x.Name.Contains("BaseRegisterView")).FirstOrDefault();
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
                    Toast toast = Toast.MakeText(this, Resource.String.no_baseApp, ToastLength.Short);
                    TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                    if (v != null)
                        v.Gravity = GravityFlags.Center;
                    toast.Show();
                }
            };

            var loginButton = FindViewById<TextView>(Resource.Id.login_button);
            loginButton.Click += (o, e) =>
            {
                LoginManager.Instance.LogInWithReadPermissions(this, EXTENDED_PERMISSIONS);
            };
            //loginButton.SetReadPermissions(EXTENDED_PERMISSIONS);

            profileTracker = new CustomProfileTracker
            {
                HandleCurrentProfileChanged = (oldProfile, currentProfile) =>
                {
                    UpdateUI();
                    HandlePendingAction();
                }
            };
        }

        private void UpdateUI()
        {
            var ifLoggedIn = AccessToken.CurrentAccessToken != null;
            profile = Profile.CurrentProfile;
            if (ifLoggedIn)
                if (profile != null)
                {
                    var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                    request.ExecuteAsync();
                }
        }

        private void HandlePendingAction()
        {
            pendingAction = PendingAction.NONE;
        }

        bool HasPublishPermission()
        {
            var accessToken = AccessToken.CurrentAccessToken;
            return accessToken != null && accessToken.Permissions.Contains("publish_actions");
        }

        class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
        {
            public Action HandleCancel { get; set; }

            public Action<FacebookException> HandleError { get; set; }

            public Action<TResult> HandleSuccess { get; set; }

            public void OnCancel()
            {
                var c = HandleCancel;
                if (c != null)
                    c();
            }

            public void OnError(FacebookException error)
            {
                var c = HandleError;
                if (c != null)
                    c(error);
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                var c = HandleSuccess;
                if (c != null)
                    c(result.JavaCast<TResult>());
            }
        }

        class CustomProfileTracker : ProfileTracker
        {
            public delegate void CurrentProfileChangedDelegate(Profile oldProfile, Profile currentProfile);

            public CurrentProfileChangedDelegate HandleCurrentProfileChanged { get; set; }

            protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
            {
                var p = HandleCurrentProfileChanged;
                if (p != null)
                    p(oldProfile, currentProfile);
            }
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(this, typeof(LandingPage));
            intent.AddFlags(ActivityFlags.NoAnimation);
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.slide_in_back, Resource.Animation.slide_out_back);
            this.Finish();
        }

        protected override void OnResume()
        {
            base.OnResume();

            AppEventsLogger.ActivateApp(this);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutString(PENDING_ACTION_BUNDLE_KEY, pendingAction.ToString());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);

            UpdateUI();

        }

        protected override void OnPause()
        {
            base.OnPause();

            AppEventsLogger.DeactivateApp(this.ApplicationContext);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            profileTracker.StopTracking();
        }
    }
}

