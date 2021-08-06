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
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using Android.Util;
using System.Timers;
using Android.Graphics;
using System.Diagnostics;
using Android.Views.Animations;
using Kunicardus.Billboards.Core.Enums;
using Kunicardus.Billboards.Plugins.RecyclerView;
using Kunicardus.Billboards.Activities;
using Kunicardus.Billboards.Fragments;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.ViewHolders;
using Kunicardus.Billboards.Plugins;
using Kunicardus.Billboards.Core.ViewModels;
using Kunicardus.Billboards.Helpers;
using Kunicardus.Billboards.Core.Helpers;
using Kunicardus.Billboards.Core.Plugins;
using Autofac;

namespace Kunicardus.Billboards.Adapters
{

    public class RecyclerAdapter : RecyclerView.Adapter
    {
        private const int VIEW_TYPE_PADDING = 1;
        private const int VIEW_TYPE_ITEM = 2;

        //private RecyclerViewList<Advertisment> _ads;
        private CustomRecyclerView mRecyclerView;
        AdsFragment _fragment;
        Timer _timer;
        Stopwatch _stopWatch;
        TranslateAnimation _anim;
        IConnectivityPlugin _connectivityPlugin;

        private List<AdvertismentViewModel> Advertisments { get; set; }

        public event EventHandler<int> TimeElapsed;

        int flingCounter;
        ProgressDialog _progressDialog;

        public RecyclerAdapter(CustomRecyclerView recyclerView, AdsFragment fragment, List<AdvertismentViewModel> advertisments)
        {
            using (var scope = App.Container.BeginLifetimeScope())
            {
                _connectivityPlugin = scope.Resolve<IConnectivityPlugin>();
            }

            Advertisments = advertisments;
            _fragment = fragment;
            flingCounter = 0;
            mRecyclerView = recyclerView;
            mRecyclerView.CurrentItem = 0;
            mRecyclerView.OnFling += OnRecyclerViewFling;
            mRecyclerView.BeforeFling += BeforeRecyclerViewFling;


            _anim = GetAnimationPoint();
            _anim.Duration = 500;
            _anim.RepeatCount = Android.Views.Animations.Animation.Infinite;
            _anim.FillAfter = true;
            _anim.RepeatMode = RepeatMode.Reverse;
            _anim.SetInterpolator(_fragment.Activity, Android.Resource.Animation.LinearInterpolator);
        }

        #region RecyclerView Controls

        private void BeforeRecyclerViewFling(object sender, FlingEventArgs e)
        {
            try
            {
                if (mRecyclerView.CurrentItem == Advertisments.Count)
                {
                    return;
                }
                if (Advertisments[mRecyclerView.CurrentItem].Advertisment.Status == AdvertismentStatus.Loaded)
                {
                    ResetTimer(e.Direction);
                }

            }
            catch (ArgumentOutOfRangeException ex)
            {
                Log.Debug("OutOfRangeException", "Ads Recycler Adapter: {0}", ex.ToString());
            }
            catch (Exception ex)
            {
                Log.Debug("Exception", "Ads Recycler Adapter: {0}", ex.ToString());
            }
        }

        private void OnRecyclerViewFling(object sender, FlingEventArgs e)
        {
            LinearLayoutManager manager = (LinearLayoutManager)mRecyclerView.GetLayoutManager();

            //if (Advertisments[mRecyclerView.CurrentItem].Downloaded && Advertisments[mRecyclerView.CurrentItem].Advertisment.Status == Core.Enums.AdvertismentStatus.Loaded)
            //{
            //    InitTimer();
            //}
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == VIEW_TYPE_ITEM)
            {
                View raw = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.AdDetailsLayout, parent, false);

                Button btnRetry = raw.FindViewById<Button>(Resource.Id.btnRetry);
                ProgressBar progressBar = raw.FindViewById<ProgressBar>(Resource.Id.progressBar1);
                TextView txtSkip = raw.FindViewById<TextView>(Resource.Id.txtSkip);
                TextView txtInfo = raw.FindViewById<TextView>(Resource.Id.txtInfo);
                TextView txtInfo2 = raw.FindViewById<TextView>(Resource.Id.txtInfo2);
                TextView txtPoints = raw.FindViewById<TextView>(Resource.Id.txtPoints);
                TextView txtSeconds = raw.FindViewById<TextView>(Resource.Id.seconds);
                TextView txtIgnore = raw.FindViewById<TextView>(Resource.Id.txtIgnore);
                TextView txtPassTime = raw.FindViewById<TextView>(Resource.Id.txtPassTime);
                TextView txtPassDate = raw.FindViewById<TextView>(Resource.Id.txtPassDate);
                TextView txtOrganization = raw.FindViewById<TextView>(Resource.Id.txtOrganization);
                ImageView imgOrganization = raw.FindViewById<ImageView>(Resource.Id.imgOrganizationLogo);
                ImageView imgAd = raw.FindViewById<ImageView>(Resource.Id.imgAd);
                ImageView imgSuccess = raw.FindViewById<ImageView>(Resource.Id.imgSuccess);
                ImageView imgDownload = raw.FindViewById<ImageView>(Resource.Id.imgDownloadCube);
                ImageView imgArrow = raw.FindViewById<ImageView>(Resource.Id.arrow);
                RelativeLayout overlay = raw.FindViewById<RelativeLayout>(Resource.Id.overlayRelativeLayout);
                RelativeLayout timer = raw.FindViewById<RelativeLayout>(Resource.Id.progressBarLayout);
                RelativeLayout points = raw.FindViewById<RelativeLayout>(Resource.Id.pointsLayout);
                RelativeLayout download = raw.FindViewById<RelativeLayout>(Resource.Id.downloadRelativeLayout);
                //ProgressBar adProgressBar = raw.FindViewById<ProgressBar>(Resource.Id.adProgressBar);
                //btnRetry.Gravity = GravityFlags.Center;
                //btnRetry.TextAlignment = TextAlignment.Center;
                overlay.Visibility = ViewStates.Gone;
                imgDownload.Click += DownloadCLick;
                imgArrow.Click += DownloadCLick;
                txtSkip.Click += SkipClick;
                txtIgnore.Click += SkipClick;
                btnRetry.Click += RetryClick;

                AdsViewHolder view = new AdsViewHolder(raw)
                {
                    BtnRetry = btnRetry,
                    BtnSkip = txtSkip,
                    ProgressBar = progressBar,
                    Seconds = txtSeconds,
                    TxtOrganization = txtOrganization,
                    TxtInfo = txtInfo,
                    TxtInfo2 = txtInfo2,
                    TxtPassTime = txtPassTime,
                    TxtPassDate = txtPassDate,
                    OverlayLayout = overlay,
                    Image = imgAd,
                    ImgSuccess = imgSuccess,
                    ImgArrow = imgArrow,
                    ImgDownload = imgDownload,
                    ImgOrganizationLogo = imgOrganization,
                    Points = txtPoints, 
                    DownloadLayout = download,
                    PointsLayout = points,
                    TimerLayout = timer
                };
                return view;
            }
            else
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_padding, parent, false);
                return new PaddingViewHolder(row);
            }

        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (GetItemViewType(position) == VIEW_TYPE_ITEM)
            {
                int indexPosition = position - 1;

                if (Advertisments[indexPosition] != null)
                {
                    if (Advertisments[indexPosition].Downloaded || (Advertisments[indexPosition].Advertisment.Status != AdvertismentStatus.NotLoaded
                        && Advertisments[indexPosition].Advertisment.Status != AdvertismentStatus.Skipped))
                    {
                        BindViewHolder(holder, position, indexPosition);
                    }
                    else
                    {
                        var myHolder = holder as AdsViewHolder;
                        var date = Advertisments[indexPosition].Advertisment.PassDate;
                        myHolder.OverlayLayout.Visibility = ViewStates.Gone;
                        myHolder.TimerLayout.Visibility = ViewStates.Gone;
                        myHolder.PointsLayout.Visibility = ViewStates.Gone;
                        myHolder.DownloadLayout.Visibility = ViewStates.Visible;
                        myHolder.TxtOrganization.Text = Advertisments[indexPosition].Advertisment.OrganizationName;
                        myHolder.TxtPassDate.Text = date.ToGeoString();
                        myHolder.TxtPassTime.Text = date.ToShortTimeString();

                        if (Advertisments[indexPosition].Advertisment.OrganizationLogo != null)
                        {
                            var byteArray = Convert.FromBase64String(Advertisments[indexPosition].Advertisment.OrganizationLogo);
                            using (Bitmap bmp = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length), 100, 60, false))
                            {
                                myHolder.ImgOrganizationLogo.SetImageBitmap(bmp);
                            }
                        }
                    }
                }
                //else
                //{
                //    Task.Run(() =>
                //    {
                //        var adLoaded = Advertisments[indexPosition].Load();
                //        _fragment.Activity.RunOnUiThread(() =>
                //        {
                //            if (adLoaded)
                //            {
                //                BindViewHolder(holder, position, indexPosition);
                //            }
                //        });
                //    });
                //}
            }
        }

        private void BindViewHolder(RecyclerView.ViewHolder holder, int position, int indexPosition)
        {
            var myHolder = holder as AdsViewHolder;
            myHolder.DownloadLayout.Visibility = ViewStates.Gone;
            myHolder.Tag = Advertisments[indexPosition].Advertisment.AdvertismentId.ToString();
            myHolder.PointsLayout.Visibility = ViewStates.Visible;
            myHolder.TimerLayout.Visibility = ViewStates.Visible;
            myHolder.ProgressBar.Max = Advertisments[indexPosition].Advertisment.TimeOut * 10;

            #region Status Check
            switch (Advertisments[indexPosition].Advertisment.Status)
            {

                case Kunicardus.Billboards.Core.Enums.AdvertismentStatus.Loaded:
                    myHolder.OverlayLayout.Visibility = ViewStates.Visible;
                    myHolder.TimerLayout.Visibility = ViewStates.Gone;
                    myHolder.BtnRetry.Text = "რეკლამის ნახვა";
                    myHolder.TxtInfo2.Visibility = ViewStates.Visible;
                    myHolder.TxtInfo2.Text = string.Format("აღნიშნული რეკლამის ყურების შემდეგ თქვენ დაგერიცხებათ {0} ქულა", Advertisments[indexPosition].Advertisment.Points);
                    myHolder.TxtInfo.Visibility = ViewStates.Invisible;
                    //myHolder.BtnSkip.Visibility = ViewStates.Invisible;

                    break;
                case Kunicardus.Billboards.Core.Enums.AdvertismentStatus.Seen:

                    myHolder.Seconds.Text = "";

                    myHolder.BtnRetry.Text = "ქულების დარიცხვა";

                    myHolder.ImgSuccess.SetImageDrawable(_fragment.Resources.GetDrawable(Resource.Drawable.error));
                    myHolder.ImgSuccess.Visibility = ViewStates.Visible;

                    myHolder.ProgressBar.Progress = Advertisments[indexPosition].Advertisment.TimeOut * 10;
                    myHolder.ProgressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#C8C528"), PorterDuff.Mode.SrcIn);
                    myHolder.OverlayLayout.Visibility = ViewStates.Visible;
                    ShowOverlay(myHolder, false, indexPosition);
                    break;
                case Kunicardus.Billboards.Core.Enums.AdvertismentStatus.PointsAcumulated:
                    myHolder.TimerLayout.Visibility = ViewStates.Visible;
                    myHolder.PointsLayout.Visibility = ViewStates.Visible;

                    myHolder.BtnRetry.Text = "მეტი ქულა";
                    myHolder.BtnSkip.Visibility = ViewStates.Visible;

                    myHolder.Seconds.Text = "";

                    myHolder.ImgSuccess.SetImageDrawable(_fragment.Resources.GetDrawable(Resource.Drawable.pointsAcumulated));
                    myHolder.ImgSuccess.Visibility = ViewStates.Visible;

                    myHolder.BtnSkip.Visibility = ViewStates.Visible;

                    myHolder.ProgressBar.Progress = Advertisments[indexPosition].Advertisment.TimeOut * 10;
                    myHolder.ProgressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#C8C528"), PorterDuff.Mode.SrcIn);
                    myHolder.OverlayLayout.Visibility = ViewStates.Visible;
                    ShowOverlay(myHolder, true, indexPosition);
                    break;
                case AdvertismentStatus.NotLoaded:
                    myHolder.OverlayLayout.Visibility = ViewStates.Gone;
                    myHolder.DownloadLayout.Visibility = ViewStates.Visible;
                    break;
                default:
                    break;
            }
            #endregion

            myHolder.Points.Text = Advertisments[indexPosition].Advertisment.Points.ToString();

            //if (position == 1 &&  Advertisments[indexPosition].Downloaded && Advertisments[indexPosition].Advertisment.Status == AdvertismentStatus.Loaded)
            //{
            //    InitTimer();
            //}

            if (Advertisments[indexPosition].Advertisment.Image != null)
            {
                var byteArray = Convert.FromBase64String(Advertisments[indexPosition].Advertisment.Image);
                using (Bitmap bmp = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length), 300, 437, false))
                {
                    myHolder.Image.SetImageBitmap(bmp);
                }
            }
        }

        public override int ItemCount
        {
            get { return Advertisments.Count + 2; }
        }

        public override int GetItemViewType(int position)
        {
            if (position == 0 || position == ItemCount - 1)
            {
                return VIEW_TYPE_PADDING;
            }
            return VIEW_TYPE_ITEM;
        }

        #endregion

        #region Click Events

        private void DownloadCLick(object sender, EventArgs e)
        {
            if (!_connectivityPlugin.IsNetworkReachable)
            {
                var toast = Toast.MakeText(_fragment.Activity, "არ გაქვთ კავშირი ინტერნეტთან", ToastLength.Short);
                TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                if (v != null)
                    v.Gravity = GravityFlags.Center;
                toast.Show();
                return;
            }

            var view = mRecyclerView.GetLayoutManager().GetChildAt(1);
            if (view == null)
            {
                return;
            }
            var viewHolder = mRecyclerView.GetChildViewHolder(view);
            if (!(viewHolder is AdsViewHolder))
            {
                return;
            }
            var item = (AdsViewHolder)viewHolder;

            //Animation rotationAnime = AnimationUtils.LoadAnimation(_fragment.Activity, Resource.Animation.indeterminate_rotation);
            item.ImgArrow.StartAnimation(_anim);

            //item.AdProgressBar.Visibility = ViewStates.Visible;
            //item.AdProgressBar.Enabled = true;
            item.ImgArrow.Enabled = false;
            item.ImgDownload.Enabled = false;
            Task.Run(() =>
                {
                    var adLoaded = Advertisments[mRecyclerView.CurrentItem].Load();
                    _fragment.Activity.RunOnUiThread(() =>
                        {
                            if (adLoaded.Successful)
                            {
                                BindViewHolder(viewHolder, 1, mRecyclerView.CurrentItem);
                            }
                            else
                            {
                                var toast = Toast.MakeText(_fragment.Activity, adLoaded.DisplayMessage ?? "რეკლამის ჩამოტვირთვა ვერ მოხერხდა", ToastLength.Short);
                                TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                                if (v != null)
                                    v.Gravity = GravityFlags.Center;
                                toast.Show();
                            }
                            item.ImgArrow.ClearAnimation();
                            item.ImgArrow.Enabled = true;
                            item.ImgDownload.Enabled = true;
                            //item.AdProgressBar.Visibility = ViewStates.Gone;
                            //item.AdProgressBar.Enabled = false;
                            //item.BtnDownload.Enabled = true;
                        });
                });           
        }

        private void SkipClick(object sender, EventArgs e)
        {
            int textId = 0;
            switch (Advertisments[mRecyclerView.CurrentItem].Advertisment.Status)
            {
                case AdvertismentStatus.Loaded:
                    textId = Resource.String.loadedIgnoreText;
                    break;
                case AdvertismentStatus.Seen:
                    textId = Resource.String.loadedIgnoreText;
                    break;
                case AdvertismentStatus.PointsAcumulated:
                    textId = Resource.String.extraIgnoreText;
                    break;
                case AdvertismentStatus.NotLoaded:
                    textId = Resource.String.ignoreText;
                    break;
                default:
                    break;
            }
                   
            _fragment.Activity.RunOnUiThread(() =>
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(_fragment.Activity);
                    builder.SetMessage(textId)
                       .SetNegativeButton("არა", delegate
                        {
                        })
                       .SetPositiveButton("დიახ", SkipAd);   
                    AlertDialog _alertDialog = builder.Create();
                    _alertDialog.SetCanceledOnTouchOutside(false);
                    _alertDialog.Show();
                });
        }

        private void SkipAd(object sender, DialogClickEventArgs e)
        {
            try
            {
                Advertisments[mRecyclerView.CurrentItem].SkipTheAd();
                if (mRecyclerView.CurrentItem == 0)
                {
                    Advertisments.RemoveAt(mRecyclerView.CurrentItem);
                    NotifyItemRemoved(1);
                    //ResetTimer();
                }
                else
                {
                    Advertisments.RemoveAt(mRecyclerView.CurrentItem);
                    NotifyItemRemoved(mRecyclerView.CurrentItem + 1);
                    if (Advertisments.Count <= mRecyclerView.CurrentItem)
                    {
                        mRecyclerView.CurrentItem = Advertisments.Count - 1;
                    }
                }
                mRecyclerView.ItemCount--;
                ((MainActivity)_fragment.Activity).DecreaseAlertCount();
                //InitTimer();
            }
            catch (Exception ex)
            {
                Log.Debug("Refresh Ex: ", ex.ToString());
            }
        }

        private void RetryClick(object sender, EventArgs e)
        {
            if (Advertisments[mRecyclerView.CurrentItem].Advertisment.Status == AdvertismentStatus.Loaded)
            {
                StartWatching();
                return;
            }
            var item = (Button)sender;
            if (item.Text == _fragment.Activity.Resources.GetString(Resource.String.more))
            {
                Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Advertisments[mRecyclerView.CurrentItem].Advertisment.ExternalLink));
                _fragment.Activity.StartActivity(browserIntent);
            }
            else
            {
                if (!_connectivityPlugin.IsNetworkReachable)
                {
                    var toast = Toast.MakeText(_fragment.Activity, "არ გაქვთ კავშირი ინტერნეტთან", ToastLength.Short);
                    TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                    if (v != null)
                        v.Gravity = GravityFlags.Center;
                    toast.Show();
                    return;
                }
                TimerElapsed(null, null);
            }
        }

        #endregion

        public void StartWatching()
        {
            var view = mRecyclerView.GetLayoutManager().GetChildAt(1);
            if (view == null)
            {
                return;
            }
            var viewHolder = mRecyclerView.GetChildViewHolder(view);
            if (!(viewHolder is AdsViewHolder))
            {
                return;
            }
            var myHolder = (AdsViewHolder)viewHolder;

            myHolder.TimerLayout.Visibility = ViewStates.Visible;
            myHolder.PointsLayout.Visibility = ViewStates.Visible;

            myHolder.OverlayLayout.Visibility = ViewStates.Invisible;
            myHolder.ImgSuccess.Visibility = ViewStates.Invisible;

            myHolder.Seconds.Text = Advertisments[mRecyclerView.CurrentItem].Advertisment.TimeOut.ToString();
            //myHolder.Seconds.Visibility = ViewStates.Visible;

            myHolder.Seconds.SetTextColor(Color.ParseColor("#F44F1A"));
            myHolder.ProgressBar.Progress = 0;
            myHolder.ProgressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#F44F1A"), PorterDuff.Mode.SrcIn);

            InitTimer();
        }

        public TranslateAnimation GetAnimationPoint()
        {
            DisplayMetrics metrics = new DisplayMetrics();
            _fragment.Activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
            switch (metrics.DensityDpi)
            {
                case DisplayMetricsDensity.High:
                    return new TranslateAnimation(0, 0, 0, 15);
                case DisplayMetricsDensity.Medium:
                    return new TranslateAnimation(0, 0, 0, 20);
                case DisplayMetricsDensity.Xhigh:
                    return new TranslateAnimation(0, 0, 0, 25);
                case DisplayMetricsDensity.Xxhigh:
                    return new TranslateAnimation(0, 0, 0, 45);
                case DisplayMetricsDensity.Xxxhigh:
                    return new TranslateAnimation(0, 0, 0, 50);
                default:
                    return new TranslateAnimation(0, 0, 0, 20);
            }
        }

        private void SavePoints(AdsViewHolder item)
        {
            if (!_connectivityPlugin.IsNetworkReachable)
            {
                var toast = Toast.MakeText(_fragment.Activity, "არ გაქვთ კავშირი ინტერნეტთან", ToastLength.Short);
                TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                if (v != null)
                    v.Gravity = GravityFlags.Center;
                toast.Show();

                _fragment.Activity.RunOnUiThread(() =>
                    {
                        item.Seconds.Text = "";
                        item.ImgSuccess.Visibility = ViewStates.Visible;
                        ShowOverlay(item, false, mRecyclerView.CurrentItem);
                        Advertisments[mRecyclerView.CurrentItem].Advertisment.Status = AdvertismentStatus.Seen;
                        NotifyItemChanged(mRecyclerView.CurrentItem);
                    });
                return;
            }

            _progressDialog = ProgressDialog.Show(_fragment.Activity, null, "Loading...");
            _progressDialog.SetCancelable(true);
            Task.Run(() =>
                {
                    var result = Advertisments[mRecyclerView.CurrentItem].SavePoints();
                    if (result != null)
                    {
                        _fragment.Activity.RunOnUiThread(() =>
                            {
                                //if (TimeElapsed != null)
                                //{
                                //    TimeElapsed(this, mRecyclerView.CurrentItem);
                                //}
                                item.Seconds.Text = "";
                                item.ImgSuccess.Visibility = ViewStates.Visible;

                                ShowOverlay(item, result.Successful && result.Status == Core.Enums.AdvertismentStatus.PointsAcumulated, mRecyclerView.CurrentItem);

                                Advertisments[mRecyclerView.CurrentItem].Advertisment.Status = result.Status;
                                NotifyItemChanged(mRecyclerView.CurrentItem);
                                _progressDialog.Dismiss();
                            });
                    }
                });
        }

        void ShowOverlay(AdsViewHolder item, bool show, int indexPosition)
        {
            if (show)
            {
                item.ProgressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#C9C62B"), PorterDuff.Mode.SrcIn);
                item.ImgSuccess.SetImageDrawable(_fragment.Resources.GetDrawable(Resource.Drawable.pointsAcumulated));

                item.BtnRetry.Enabled = true;
                item.BtnRetry.Text = _fragment.Activity.Resources.GetString(Resource.String.more);
                item.BtnSkip.Enabled = true;
                item.BtnSkip.Visibility = ViewStates.Visible;
                if (!string.IsNullOrEmpty(Advertisments[indexPosition].Advertisment.ExternalLink))
                {
                    item.TxtInfo.Visibility = ViewStates.Visible;
                    item.BtnRetry.Visibility = ViewStates.Visible;
                    // item.BtnSkip.Text = _fragment.Activity.Resources.GetString(Resource.String.ignore);
                }
                else
                {
                    var message = _fragment.Activity.Resources.GetString(Resource.String.congrats);
                    _fragment.Activity.RunOnUiThread(() =>
                        {
                            AlertDialog.Builder builder = new AlertDialog.Builder(_fragment.Activity);
                            builder.SetMessage(string.Format(message, Advertisments[indexPosition].Advertisment.Points))
                                    .SetPositiveButton("გასაგებია", SkipAd);
                            AlertDialog _alertDialog = builder.Create();
                            _alertDialog.SetCanceledOnTouchOutside(false);
                            _alertDialog.Show();
                        });
                }
                item.TxtInfo2.Visibility = ViewStates.Visible;
                item.TxtInfo2.Text = string.Format("თქვენ დაგერიცხათ {0} ქულა", Advertisments[indexPosition].Advertisment.Points);
                //_header.Visibility = ViewStates.Invisible;
            }
            else
            {
                item.ProgressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#E95936"), PorterDuff.Mode.SrcIn);
                item.ImgSuccess.SetImageDrawable(_fragment.Resources.GetDrawable(Resource.Drawable.error));

                item.BtnRetry.Enabled = true;
                item.BtnRetry.Text = "ქულების დარიცხვა";// _fragment.Activity.Resources.GetString(Resource.String.retry);
                item.BtnSkip.Enabled = true;
                //item.BtnSkip.Text = _fragment.Activity.Resources.GetString(Resource.String.ignore);

                item.TxtInfo2.Visibility = ViewStates.Visible;
                item.TxtInfo2.Text = "სამწუხაროდ, ქულების დარიცხვა ვერ მოხერხდა. გთხოვთ, სცადოთ ხელახლა ქვედა ღილაკზე დაჭერით";
                //item.BtnSkip.Visibility = ViewStates.Invisible;
                item.TxtInfo.Visibility = ViewStates.Invisible;
            }
        }

        #region Timer Control

        public void InitTimer()
        {
            _stopWatch = new Stopwatch();
            
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Stop();
                _timer.Elapsed -= TimerElapsed;
            }
            _timer = new Timer();
            _timer.Interval = 100;
            _timer.Elapsed += TimerElapsed;
            _timer.Enabled = true;
            _timer.Start();
            _stopWatch.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // _timer.Enabled = false;
            _fragment.Activity.RunOnUiThread(() =>
                {
                    if (GetItemViewType(1) == VIEW_TYPE_ITEM)
                    {

                        var view = mRecyclerView.GetLayoutManager().GetChildAt(1);
                        if (view == null)
                        {
                            return;
                        }
                        var viewHolder = mRecyclerView.GetChildViewHolder(view);
                        if (!(viewHolder is AdsViewHolder))
                        {
                            return;
                        }
                        var item = (AdsViewHolder)viewHolder;

                        if (item.AdapterPosition == mRecyclerView.CurrentItem + 1)
                        {
                            item.ProgressBar.Progress++;
                            try
                            {
                                item.Seconds.Text = (Advertisments[mRecyclerView.CurrentItem].Advertisment.TimeOut - (item.ProgressBar.Progress / 10)).ToString();

                                if (Advertisments[mRecyclerView.CurrentItem].Advertisment.Status == AdvertismentStatus.Loaded && _stopWatch != null
                                    && _stopWatch.ElapsedMilliseconds / 1000 == Advertisments[mRecyclerView.CurrentItem].Advertisment.TimeOut
                                    && Advertisments[mRecyclerView.CurrentItem].Advertisment.Status != Core.Enums.AdvertismentStatus.PointsAcumulated)
                                {
                                    _stopWatch.Stop();
                                    _stopWatch.Reset();
                                    _timer.Elapsed -= TimerElapsed;
                                    item.Seconds.SetTextColor(Color.ParseColor("#C9C62B"));
                                    //item.ProgressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#C9C62B"), PorterDuff.Mode.SrcIn);
                                    item.OverlayLayout.Visibility = ViewStates.Visible;
                                    SavePoints(item);

                                    _timer.Enabled = false;

                                }
                                else if (Advertisments[mRecyclerView.CurrentItem].Advertisment.Status == AdvertismentStatus.Seen)
                                {
                                    SavePoints(item);
                                }

                            }
                            catch (Exception ex)
                            {

                                Log.Debug("RecyclerView Message: ", ex.ToString());
                            }
                        }
                    }
                });
            //_timer.Enabled = true;
        }


        public void ResetTimer(FlingDirection direction)
        {
            if (_stopWatch != null)
            {
                try
                {
                    _stopWatch.Stop();
                    _stopWatch.Reset();
                    AdsViewHolder item;
                    if (direction == FlingDirection.Left)
                    {
                        item = (AdsViewHolder)mRecyclerView.GetChildViewHolder(mRecyclerView.GetChildAt(1));
                    }
                    else
                    {
                        item = (AdsViewHolder)mRecyclerView.GetChildViewHolder(mRecyclerView.GetChildAt(0));
                    }

                    int seconds = 0;

                    if (item != null && item.ProgressBar != null && item.Seconds != null && int.TryParse(item.Seconds.Text, out seconds) && seconds != 0)
                    {
                        item.OverlayLayout.Visibility = ViewStates.Visible;
                        item.TimerLayout.Visibility = ViewStates.Invisible;
                        item.BtnRetry.Text = "რეკლამის ნახვა";

                        item.ProgressBar.Progress = 0;
                        item.Seconds.Text = Advertisments[mRecyclerView.CurrentItem].Advertisment.TimeOut.ToString();
                        _timer.Enabled = false;
                        _timer.Elapsed -= TimerElapsed;
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("AdsFragment Message: ", ex.ToString());
                    return;
                }
            }
        }

        internal void ResetTimer()
        {
            if (_stopWatch != null)
            {
                try
                {
                    _stopWatch.Stop();
                    _stopWatch.Reset();
                    var item = (AdsViewHolder)mRecyclerView.GetChildViewHolder(mRecyclerView.GetChildAt(1));

                    int seconds = 0;

                    if (item != null && item.ProgressBar != null && item.Seconds != null && int.TryParse(item.Seconds.Text, out seconds) && seconds != 0)
                    {
                        item.OverlayLayout.Visibility = ViewStates.Visible;
                        item.TimerLayout.Visibility = ViewStates.Invisible;
                        item.BtnRetry.Text = "რეკლამის ნახვა";

                        item.ProgressBar.Progress = 0;
                        item.Seconds.Text = Advertisments[mRecyclerView.CurrentItem].Advertisment.TimeOut.ToString();
                        _timer.Enabled = false;
                        _timer.Elapsed -= TimerElapsed;
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("AdsFragment Message: ", ex.ToString());
                    return;
                }
            }
        }

        #endregion

    }
}