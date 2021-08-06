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
using Kunicardus.Billboards.Core.Services;
using System.Threading.Tasks;
using Kunicardus.Billboards.Adapters;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.Services.Concrete;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Plugins;
using Autofac;
using Android.Views.Animations;
using Android.Media;
using System.IO;
using Android.Support.V4.Content;

namespace Kunicardus.Billboards.Fragments
{
    public class HomePageFragment : BaseFragment, Android.Views.Animations.Animation.IAnimationListener
    {
        MainActivity _activity;
        HomePageViewModel _viewModel;
        ListView _adsListView;
        LoadedAdsAdapter _adapter;
        Button btnStartTrip;
        ImageView imgStartTrip, profilePic;
        RelativeLayout ltStartTrip;
        ImageView imgGreenBack;

        TextView txtName, txtPoints, txtNearestBillboard;

        TranslateAnimation leftAnimation, bounceAnimation;
        RotateAnimation cycleAnimation;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.HomeLayout, null);

            txtName = view.FindViewById<TextView>(Resource.Id.txtName);
            txtPoints = view.FindViewById<TextView>(Resource.Id.txtPoints);
            imgGreenBack = view.FindViewById<ImageView>(Resource.Id.navigationOverlay);

            profilePic = view.FindViewById<ImageView>(Resource.Id.profilePic);
            txtNearestBillboard = view.FindViewById < TextView>(Resource.Id.txtNearestBillboard);


            #region Animations
            leftAnimation = GetAnimationPoint();
            leftAnimation.Duration = 15000;
            leftAnimation.RepeatCount = 0;
            leftAnimation.FillAfter = false;
            //leftAnimation.RepeatMode = RepeatMode.Reverse;
            leftAnimation.SetInterpolator(Activity, Android.Resource.Animation.LinearInterpolator);
            leftAnimation.SetAnimationListener(this);

            cycleAnimation = new RotateAnimation(0, 359.99f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            cycleAnimation.Duration = 1100;
            cycleAnimation.FillAfter = false;
            cycleAnimation.RepeatCount = RotateAnimation.Infinite;
            cycleAnimation.SetAnimationListener(this);
            cycleAnimation.SetInterpolator(Activity, Android.Resource.Animation.LinearInterpolator);

            bounceAnimation = new TranslateAnimation(0, 0, -700, 40);
            bounceAnimation.Duration = 800;
            bounceAnimation.RepeatCount = 0;
            bounceAnimation.FillAfter = true;
            bounceAnimation.SetAnimationListener(this);
            bounceAnimation.SetInterpolator(Activity, Android.Resource.Animation.BounceInterpolator);
           
            _playerWheel = MediaPlayer.Create(Activity, Resource.Raw.wheels_audio);
            _playerBounce = MediaPlayer.Create(Activity, Resource.Raw.bounce_audio);
            #endregion 

            profilePic.LongClick += (sender, e) =>
            {
                AnimationSet set = new AnimationSet(false);
                set.AddAnimation(cycleAnimation);
                set.AddAnimation(leftAnimation);
                profilePic.StartAnimation(set);
            };

            _activity = (MainActivity)Activity;
            var connectivityPlugin = new ConnectivityPlugin();
            using (var scope = App.Container.BeginLifetimeScope())
            {
                _viewModel = scope.Resolve<HomePageViewModel>();
                _viewModel.SessionTimedOut += _activity.LogoutClicked;
            }

            _adsListView = view.FindViewById<ListView>(Resource.Id.adsListView);

            _adapter = new LoadedAdsAdapter(this.Activity, new List<AdsModel>());
            _adsListView.Adapter = _adapter;
            _adsListView.ItemClick += ListItemClicked;

            btnStartTrip = view.FindViewById<BaseButton>(Resource.Id.btnStartTrip);
            imgStartTrip = view.FindViewById<ImageButton>(Resource.Id.imgStartTrip);
            ltStartTrip = view.FindViewById<RelativeLayout>(Resource.Id.navigationLayout2);
            ltStartTrip.Click += StartTripClick;
            btnStartTrip.Click += StartTripClick;
            imgStartTrip.Click += StartTripClick;

            view.FindViewById<BaseTextView>(Resource.Id.btnAllads1).Click += (o, e) =>
            {
                _activity.OpenAds();
            };
            view.FindViewById<ImageButton>(Resource.Id.btnAllAds).Click += (o, e) =>
            {
                _activity.OpenAds();
            };

            var mapBack = view.FindViewById<ImageView>(Resource.Id.background);
            mapBack.SetImageDrawable(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.mapBackground));

            this.OnActivate();
            return view;
        }

        public void UpdateDistance(string distance)
        {
            if (txtNearestBillboard!=null)
            {
                txtNearestBillboard.Visibility = ViewStates.Visible;
                txtNearestBillboard.Text = string.Format("მანძილი უახლოეს ბილბორდამდე: {0}", distance);
            }
        }

        private void ListItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            //var itemId = _adapter.GetItemId(e.Position);
            _activity.OpenAds(e.Position);
        }

        private void StartTripClick(object sender, EventArgs e)
        {
            SetButtonBackground();
            if (!_activity.CheckPreviewMode())
            {
                _activity.OpenMap(true);
            }
            else
            {
                _activity.OpenMap(false);
            }
        }

        public void SetButtonBackground()
        {
            if (_activity.CheckPreviewMode())
            {
                btnStartTrip.Text = Resources.GetString(Resource.String.endTrip);
                ltStartTrip.SetBackgroundResource(Resource.Drawable.rounded_btn_background2);
                imgStartTrip.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.endTripNew));
                imgGreenBack.Visibility = ViewStates.Visible;
            }
            else
            {
                btnStartTrip.Text = Resources.GetString(Resource.String.startTrip);
                ltStartTrip.SetBackgroundResource(Resource.Drawable.rounded_btn_background);
                imgStartTrip.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.trip));
                imgGreenBack.Visibility = ViewStates.Invisible; 
                txtNearestBillboard.Visibility = ViewStates.Invisible;
            }
        }

        public override void OnActivate(object o = null)
        {
            //_activity.ActionBar.SetTitle(Resource.String.homePage);
            SetButtonBackground();

            Task.Run(() =>
            {
                var result = _viewModel.GetUserInfo();
                if (!string.IsNullOrEmpty(_viewModel.DisplayMessage))
                {
                    Toast.MakeText(Activity, _viewModel.DisplayMessage, ToastLength.Short).Show();
                }
                if (result)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        txtName.Text = _viewModel.User.FirstName + " " + _viewModel.User.LastName;
                        txtPoints.Text = _viewModel.User.Balance_AvailablePoints.ToString();
                        _activity.SetDrawerName(txtName.Text);
                    });
                }

                var adsLoaded = _viewModel.GetLoadedAds();
                if (adsLoaded)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        _adapter.UpdateList(_viewModel.Advertisments);
                        _activity.ChangeAlertCount(_viewModel.Advertisments.Count);
                        _adapter.NotifyDataSetChanged();
                    });
                }   
            });
        }

        #region Animation
        MediaPlayer _playerWheel;
        MediaPlayer _playerBounce;

        public TranslateAnimation GetAnimationPoint()
        {
            DisplayMetrics metrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
            switch (metrics.DensityDpi)
            {
                case DisplayMetricsDensity.High:
                    return new TranslateAnimation(0, 500, 0, 0);
                case DisplayMetricsDensity.Medium:
                    return new TranslateAnimation(0, 420, 0, 0);
                case DisplayMetricsDensity.Xhigh:
                    return new TranslateAnimation(0, 735, 0, 0);
                case DisplayMetricsDensity.Xxhigh:
                    return new TranslateAnimation(0, 1080, 0, 0);
                case DisplayMetricsDensity.Xxxhigh:
                    return new TranslateAnimation(0, 1370, 0, 0);
                default:
                    return new TranslateAnimation(0, 0, 0, 20);
            }
        }

        public void OnAnimationEnd(Animation animation)
        {
            if (animation == leftAnimation)
            {
                _playerWheel.Pause();
                //profilePic.ClearAnimation();
                profilePic.StartAnimation(bounceAnimation);
            }
            else if (animation == bounceAnimation)
            {
                _playerBounce.Pause();
                profilePic.ClearAnimation(); 
            }
        }

        public void OnAnimationRepeat(Animation animation)
        {
            ///throw new NotImplementedException();
        }

        public void OnAnimationStart(Animation animation)
        {
            try
            {
                if (animation == leftAnimation)
                {
                    _playerWheel.Start();
                }
                if (animation == bounceAnimation)
                {
                    _playerBounce.Start();
                }
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
        }
        #endregion
    }
}