using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Kunicardus.Billboards.Activities;
using Kunicardus.Billboards.Core.ViewModels;
using Autofac;

namespace Kunicardus.Billboards.Fragments
{
    public class InfoFragment : BaseFragment
    {
        SettingsViewModel _viewModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var _view = inflater.Inflate(Resource.Layout.InfoLayout, null);

            using (var scope = App.Container.BeginLifetimeScope())
            {
                _viewModel = scope.Resolve<SettingsViewModel>();
            }

            var packageInfo = Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0);
            _view.FindViewById<BaseTextView>(Resource.Id.version).Text = string.Format("ვერსია: V{0}.{1}", packageInfo.VersionName, packageInfo.VersionCode);
            
            var fullName = _view.FindViewById<BaseTextView>(Resource.Id.settings_fullname);
            var userName = _view.FindViewById<BaseTextView>(Resource.Id.settings_username);
            var fb_icon = _view.FindViewById<ImageView>(Resource.Id.fb_icon);
            var logout = _view.FindViewById<BaseButton>(Resource.Id.settings_logout);
            var byWandio = _view.FindViewById<Button>(Resource.Id.by_wandio);

            byWandio.Click += (sender, e) =>
            {
                OpenWebPage("http://www.wandio.com");
            };
            if (_viewModel.UserInfoFromDB != null)
            {
                fullName.Text = String.Format("{0} {1}", _viewModel.UserInfoFromDB.FirstName, _viewModel.UserInfoFromDB.LastName);
                userName.Text = _viewModel.UserInfoFromDB.Username;
                if (!_viewModel.UserInfoFromDB.IsFacebookUser)
                    fb_icon.Visibility = ViewStates.Invisible;
                else
                    fb_icon.Visibility = ViewStates.Visible;
                logout.Click += (sender, e) =>
                {
                    ((MainActivity)Activity).LogoutClicked(null, null);
                };
            }

            return _view;
        }

        public override void OnActivate(object o = null)
        {
        }

        private void OpenWebPage(string address)
        {
            try
            {
                var uri = Android.Net.Uri.Parse(address);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
            catch
            {
                Toast.MakeText(Activity, "საიტზე გადასვლა ვერ მოხერხდა", ToastLength.Long).Show();
            }
        }
    }
}