using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Support.V4.Widget;
using Kunicardus.Billboards.Adapters;
using Kunicardus.Billboards.Helpers;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Graphics.Drawables;
using Android.Graphics;
using Kunicardus.Billboards.Core.ViewModels;
using Android.Content.PM;
using Xamarin.Facebook.Login;
using Xamarin.Facebook;
using Autofac;
using Kunicardus.Billboards.Fragments;

namespace Kunicardus.Billboards.Activities
{
    [Activity(Label = "UNICARD BILLBOARDS", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FragmentActivity
    {
        private MenuPagerAdapter adapter;
        private CustomViewPager pager;
        private MainViewModel _viewModel;

        TextView _alertCount;
        TextView _title;

        DrawerLayout _drawerLayout;
        ActionBarDrawerToggle _drawerToggle;

        public override void OnBackPressed()
        {
            if (_drawerLayout!=null && _drawerLayout.IsDrawerVisible(GravityCompat.Start))
            {
                _drawerLayout.CloseDrawer(GravityCompat.Start);
                return;
            }
            if (pager!=null && pager.CurrentItem != 0)
            {
                pager.SetCurrentItem(0, false);
                return;
            }
            if (pager.CurrentItem == 0)
            {
                AlertDialog alertDialog;
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage(Resources.GetString(Resource.String.closeMessage));
                builder.SetPositiveButton(("დიახ"), delegate
                {
                    this.MoveTaskToBack(true);
                });
                builder.SetNegativeButton("არა", delegate
                {
                });
                alertDialog = builder.Create();
                alertDialog.SetCanceledOnTouchOutside(true);
                alertDialog.Show();

                TextView message = alertDialog.FindViewById<TextView>(Android.Resource.Id.Message);
                message.Gravity = GravityFlags.Center;
                return;
            }  
        }

		protected override void OnCreate (Bundle bundle)
        {
			base.OnCreate (bundle);
			FacebookSdk.SdkInitialize (this);
			SetContentView (Resource.Layout.Main);
            
            using (var scope = App.Container.BeginLifetimeScope())
            {
                _viewModel = scope.Resolve<MainViewModel>();
            }

            var alert = FindViewById<ImageButton>(Resource.Id.alert);
            _alertCount = FindViewById<TextView>(Resource.Id.alertCount);
            _alertCount.Click += AlertCountClick;
            alert.Click += AlertCountClick;

            #region DisplayMetrics
            //DisplayMetrics metrics = new DisplayMetrics();
            //WindowManager.DefaultDisplay.GetMetrics(metrics);
            //switch (metrics.DensityDpi)
            //{
            //    case DisplayMetricsDensity.D280:
            //        break;
            //    case DisplayMetricsDensity.D400:
            //        break;
            //    case DisplayMetricsDensity.D560:
            //        break;
            //    case DisplayMetricsDensity.High:
            //        break;
            //    case DisplayMetricsDensity.Low:
            //        break;
            //    case DisplayMetricsDensity.Medium:
            //        break;
            //    case DisplayMetricsDensity.Tv:
            //        break;
            //    case DisplayMetricsDensity.Xhigh:
            //        break;
            //    case DisplayMetricsDensity.Xxhigh:
            //        break;
            //    case DisplayMetricsDensity.Xxxhigh:
            //        break;
            //    default:
            //        break;
            //}
            #endregion

            InitAdapter ();
			DrawerSetup ();

            //var view = ActionBar.CustomView;
            _title = FindViewById<TextView>(Resource.Id.pageTitle);
        }

        private void AlertCountClick(object sender, EventArgs e)
        {
            pager.SetCurrentItem(2, false);
        }

		public void ChangePageTitle (int position)
        {            
			switch (position) {
                case 0:
				_title.SetText (Resource.String.homePage);
                    break;
                case 1:
				_title.SetText (Resource.String.map);
                    break;
                case 2:
				_title.SetText (Resource.String.ads);
                    break;
                case 3:
				_title.SetText (Resource.String.history);
                    break;
                case 4:
				_title.SetText (Resource.String.info);
                    break;
            }
        }

		public bool CheckPreviewMode ()
        {
			return adapter.CheckPreviewMode ();
        }

        public bool OpenMap(bool value)
        {
            pager.SetCurrentItem(1, true);
            return adapter.TooglePreviewMode(value);
        }

		public void OpenAds ()
        {
			pager.SetCurrentItem (2, false);
        }

        public void OpenAds(object parameter)
        {
            adapter.SetParameter(parameter);
            pager.SetCurrentItem(2, false);
        }
                        
        private void InitAdapter()
        {

			pager = FindViewById<CustomViewPager> (Resource.Id.pager);
			pager.EnableTouchEvents (false);
			adapter = new MenuPagerAdapter (SupportFragmentManager, this, pager);
			var pageMargin = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, 4, Resources.DisplayMetrics);
            pager.OffscreenPageLimit = 4;
            pager.PageMargin = pageMargin;
			pager.PageSelected += (o, e) => {
				adapter.ActivateFragment (e.Position);
            };
            pager.Adapter = adapter;
            pager.CurrentItem = 0;
        }

        private void DrawerSetup()
        {
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout, Android.Resource.Color.Transparent, Resource.String.drawer_open, Resource.String.drawer_close);
            _drawerLayout.SetDrawerListener(_drawerToggle);
            _drawerLayout.DrawerOpened += (o, e) =>
            {
                InvalidateOptionsMenu();
            };
            _drawerLayout.DrawerClosed += (o, e) =>
            {
                InvalidateOptionsMenu();
            };

            var btnDrawer = FindViewById<ImageButton>(Resource.Id.menuImg);
            btnDrawer.Click += (o, e) =>
            {
                if (_drawerToggle.DrawerIndicatorEnabled)
                {
                    if (_drawerLayout.IsDrawerVisible(GravityCompat.Start))
                    {
                        _drawerLayout.CloseDrawer(GravityCompat.Start);
                    }
                    else
                    {
                        _drawerLayout.OpenDrawer(GravityCompat.Start);
                    }
                }
            };

            #region Load Menu List
            ListView _menuList = FindViewById<ListView>(Resource.Id.menuList);
            if (_menuList != null)
            {
                MenuAdapter _adapter = new MenuAdapter(this, _viewModel.MenuItems);
                _menuList.Adapter = _adapter;
                _menuList.ItemClick += (o, e) =>
                {
                    if (e.Position == _menuList.Count - 1)
                    {
                        LogoutClicked(this, null);
                    }
                    else
                    {
                        _drawerLayout.CloseDrawer(GravityCompat.Start);
                        pager.SetCurrentItem(e.Position, false);
                    }
                };
            }
            #endregion
        }

        public void SetDrawerName(string name)
        {
            var textView = FindViewById<TextView>(Resource.Id.username);
            textView.Text = name;
        }

        public void LogoutClicked(object sender, EventArgs e)
        {
            var success = _viewModel.Logout();
            if (success)
            {
                if (LoginManager.Instance != null)
                {
                    LoginManager.Instance.LogOut();
                }
                Intent intent = new Intent(this, typeof(LoginView));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                this.Finish();
                StartActivity(intent);
            }
        }

        public void ChangeAlertCount(int count)
        {
            if (count>0)
            {
                _alertCount.Visibility = ViewStates.Visible;
                _alertCount.Text = count.ToString();
            }
            else
            {
                _alertCount.Visibility = ViewStates.Gone;
            }
        }

        public void IncreaseAlertCount()
        {
            RunOnUiThread(() =>
            {
                int count = Convert.ToInt32(_alertCount.Text);
                _alertCount.Visibility = ViewStates.Visible;
                _alertCount.Text = (++count).ToString();
            });
        }

        public void DecreaseAlertCount()
        {
            RunOnUiThread(() =>
            {
                int count = Convert.ToInt32(_alertCount.Text);
                if (--count > 0)
                {
                    _alertCount.Visibility = ViewStates.Visible;
                    _alertCount.Text = count.ToString();
                }
                else
                {
                    _alertCount.Visibility = ViewStates.Gone;
                }
            });
        }

        public void UpdateDistance(string distance)
        {
            adapter.UpdateDistance(distance);
        }
    }
}

