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
using Kunicardus.Billboards.Core;
using Kunicardus.Billboards.Core.DbModels;
using Splunk.Mint;

namespace Kunicardus.Billboards.Activities
{
    [Activity(Label = "UNIBOARD", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true,
        LaunchMode = LaunchMode.SingleTask, Theme = "@style/Theme.Splash")]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Mint.EnableDebug();
            Mint.InitAndStartXamarinSession(Application.Context, "171d0ceb");

            var db = new BillboardsDb(BillboardsDb.path);
            if (db.Table<UserInfo>().FirstOrDefault() != null)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(this, typeof(LandingPage));
                StartActivity(intent);                
            }

        }
    }
}