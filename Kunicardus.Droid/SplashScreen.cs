using Android.App;
using Android.Content.PM;
using Xamarin.Facebook;
using Splunk.Mint;
//using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.OS;
using Plugin.CurrentActivity;

namespace Kunicardus.Droid
{
	[Activity (
		Label = "UNICARD"
		, MainLauncher = true
		, Icon = "@drawable/app_icon"
        , Theme = "@style/SplashTheme"
        , NoHistory = true
		, ScreenOrientation = ScreenOrientation.Portrait)]
	public class SplashScreen : MvxSplashScreenActivity
	{
		public SplashScreen ()
			: base (Resource.Layout.SplashScreen)
		{
		}

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
        }

        protected override void OnViewModelSet ()
		{
			base.OnViewModelSet ();
			if (!FacebookSdk.IsInitialized)
				FacebookSdk.SdkInitialize (this);

			////GAService.GetGASInstance ().Initialize (this);

			//TODO: uncomment and build special version for spac phones 
//			Mint.EnableDebug ();
//			Mint.InitAndStartXamarinSession (Application.Context, "c0755731");
		}
	}
}