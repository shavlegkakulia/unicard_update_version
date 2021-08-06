using System;
using Android.App;
using Kuni.Core;
using Xamarin.Facebook;
using Xamarin.Facebook.Share.Widget;
using Xamarin.Facebook.Share;
using Java.Interop;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.AppEvents;
using Android.OS;
using Android.Content;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views;
using Android;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;

namespace Kunicardus.Droid
{
    [Activity(Label = "Unicard", NoHistory = false,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTask,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginView : MvxActivity, GraphRequest.IGraphJSONObjectCallback
    {
        public void OnCompleted(Org.Json.JSONObject p0, GraphResponse p1)
        {
            string email = string.Empty, name = string.Empty, lastName = string.Empty;
            var objContent = p1.JSONObject.ToString();
            if (objContent.Contains("email"))
            {
                email = p1.JSONObject.GetString("email");
            }
            if (objContent.Contains("first_name"))
            {
                name = p1.JSONObject.GetString("first_name");
            }
            if (objContent.Contains("last_name"))
            {
                lastName = p1.JSONObject.GetString("last_name");
            }

            if (!ViewModel.Completed)
            {
                ViewModel.FacebookConnect(name, lastName, email, AccessToken.CurrentAccessToken.Token);
                ViewModel.Completed = true;
            }
        }

        const String PENDING_ACTION_BUNDLE_KEY = "com.facebook.samples.hellofacebook:PendingAction";
        PendingAction pendingAction = PendingAction.NONE;

        private readonly string[] EXTENDED_PERMISSIONS = { "public_profile", "email" };
        ICallbackManager callbackManager;
        ProfileTracker profileTracker;
        ShareDialog shareDialog;
        FacebookCallback<SharerResult> shareCallback;
        Profile profile;

        public static int LOCATION_Permission_REQUEST_CODE = 1;

        enum PendingAction
        {
            NONE,
            POST_PHOTO,
            POST_STATUS_UPDATE
        }

        #region Location Permission Request
        protected readonly string[] PermissionsLocation =
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

        static readonly int RequestLocationId = 98;

        void RequestLocationPermissions()
        {
            const string permission = Manifest.Permission.AccessFineLocation;
            if (ContextCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted)
            {
                //_progressDialog = ProgressDialog.Show(this, null, "მიმდინარეობს ადგილმდებარეობის მიღება...");
                return;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
            {
                ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
                return;
            }

            ActivityCompat.RequestPermissions(this, PermissionsLocation, LOCATION_Permission_REQUEST_CODE);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [Android.Runtime.GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        #endregion

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            SetContentView(Resource.Layout.LoginView);

            ////GAService.GetGASInstance().Track_App_Page(8);
#if DEBUG
            FacebookSdk.IsDebugEnabled = true;
            FacebookSdk.AddLoggingBehavior(LoggingBehavior.IncludeAccessTokens);
#endif

            (ViewModel as LoginViewModel).Completed = false;
            callbackManager = CallbackManagerFactory.Create();

            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult =>
                {
                    HandlePendingAction();
                },
                HandleCancel = () =>
                {
                    if (pendingAction != PendingAction.NONE)
                    {
                        pendingAction = PendingAction.NONE;
                    }
                },
                HandleError = loginError =>
                {
                    if (pendingAction != PendingAction.NONE
                        && loginError is FacebookAuthorizationException)
                    {
                        pendingAction = PendingAction.NONE;
                    }
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

            var authButton = FindViewById<TextView>(Resource.Id.button1);
            authButton.Click += (o, e) =>
            {
                this.Finish();
            };

            var loginButton = FindViewById<TextView>(Resource.Id.login_button);
            loginButton.Click += (o, e) =>
            {
                if ((this.ViewModel as LoginViewModel).IfNewtork())
                    LoginManager.Instance.LogInWithReadPermissions(this, EXTENDED_PERMISSIONS);
                else
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.no_network), ToastLength.Short).Show();
                }
            };

            profileTracker = new CustomProfileTracker
            {
                HandleCurrentProfileChanged = (oldProfile, currentProfile) =>
                {
                    UpdateUI();
                    HandlePendingAction();
                }
            };

            RequestLocationPermissions();
        }

        protected override void OnStop()
        {
            (this.ViewModel as LoginViewModel).DismissDialog();
            base.OnStop();
        }

        private void UpdateUI()
        {
            var ifLoggedIn = AccessToken.CurrentAccessToken != null;
            profile = Profile.CurrentProfile;
            if (ifLoggedIn)
                if (profile != null)
                {
                    //todo
                    var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                    Bundle parameters = new Bundle();
                    parameters.PutString("fields", "id,first_name,last_name,email");
                    request.Parameters = parameters;
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
                HandleCancel?.Invoke();
            }

            public void OnError(FacebookException error)
            {
                HandleError?.Invoke(error);
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                HandleSuccess?.Invoke(result.JavaCast<TResult>());
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

        }

        protected override void OnPause()
        {
            base.OnPause();
            AppEventsLogger.DeactivateApp(this.ApplicationContext);
            if ((ViewModel as LoginViewModel).Completed)
            {
                (ViewModel as LoginViewModel).DismisDialog();
                (ViewModel as LoginViewModel).Completed = false;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            profileTracker.StopTracking();
        }

        public new LoginViewModel ViewModel
        {
            get { return (LoginViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }
    }
}

