using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Kunicardus.Billboards.Activities
{
    [Activity(Label = "UNIBOARD", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true, LaunchMode = LaunchMode.SingleTop, Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class LandingPage : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LandingPageLayout);

            var enter = FindViewById<Button>(Resource.Id.enter);
            enter.Click += EnterClick;
        }

        private void EnterClick(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(LoginView));
            intent.AddFlags(ActivityFlags.NoAnimation);
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.slide_in, Resource.Animation.slide_out);
        }
    }
}