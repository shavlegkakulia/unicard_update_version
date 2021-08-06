using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Kunicardus.Billboards.Core.ViewModels;
using System.Threading.Tasks;
using Kunicardus.Billboards.Adapters;
using Android.Support.V4.View;
using Kunicardus.Billboards.Plugins;
using Kunicardus.Billboards.Helpers;
using Kunicardus.Billboards.Core.Models;
using Android.Support.V7.Widget;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Plugins.RecyclerView;
using Autofac;
using Kunicardus.Billboards.Core.Enums;

namespace Kunicardus.Billboards.Fragments
{
    public class AdsFragment : BaseFragment//, Android.Views.ViewTreeObserver.IOnGlobalLayoutListener
    {
        View _view;
        ViewPager imageViewer;
        AdsListViewModel _adsViewModel;

        private CustomRecyclerView mRecyclerView;
        private SnappyLinearLayoutManager mLayoutManager;
        private RecyclerAdapter mAdapter;

        private const int NUM_ITEMS = 5;
        private const string BUNDLE_LIST_PIXELS = "allPixels";

        private float itemWidth;
        private float padding;
        private float firstItemWidth;
        public float allPixels;

        LinearLayout _adsCountLayout;
        ProgressDialog _progressDialog;
        RelativeLayout _downloadLayout;
        ImageButton _btnDownload;

        int index = 0;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            _view = inflater.Inflate(Resource.Layout.AdsLayout, null);

            _adsCountLayout = _view.FindViewById<LinearLayout>(Resource.Id.adCountRelativeLayout);

            mRecyclerView = _view.FindViewById<CustomRecyclerView>(Resource.Id.recyclerView);
            //mRecyclerView.OnFling += OnRecyclerViewFling;
            //mRecyclerView.BeforeFling += BeforeRecyclerViewFling;


            Display display = Activity.WindowManager.DefaultDisplay;
            Point size = new Point();
            display.GetSize(size);
            int width = size.X;
            int height = size.Y;
            firstItemWidth = Activity.Resources.GetDimension(Resource.Dimension.padding_item_width);
            itemWidth = Activity.Resources.GetDimension(Resource.Dimension.item_width);
            padding = (size.X - itemWidth) / 2;
            allPixels = 0;

            mRecyclerView.SetScreenWidth(width);
            //mRecyclerView.OnFling += OnRecyclerViewFling;

            mLayoutManager = new SnappyLinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
            mRecyclerView.HorizontalFadingEdgeEnabled = true;
            mRecyclerView.SetFadingEdgeLength(60);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            //mRecyclerView.AddOnScrollListener(new OnScrollListener(this));

            using (var scope = App.Container.BeginLifetimeScope())
            {
                _adsViewModel = scope.Resolve<AdsListViewModel>();
            }

            return _view;
        }

        private void OnRecyclerViewFling(object sender, FlingEventArgs e)
        {
            LinearLayoutManager linearLayoutManager = (LinearLayoutManager)mRecyclerView.GetLayoutManager();
            int lastVisibleView = linearLayoutManager.FindLastVisibleItemPosition();
            int firstVisibleView = linearLayoutManager.FindFirstVisibleItemPosition();

            if (!(e.CurrentItem == 0 && e.Direction == FlingDirection.Left) || !(e.CurrentItem == _adsViewModel.Advertisments.Count - 1 && e.Direction == FlingDirection.Right) && _adsViewModel.Advertisments.Count > 1)
            {
                if (e.Direction == FlingDirection.Right)
                {
                    var inactive = (ImageView)_adsCountLayout.GetChildAt(firstVisibleView-1);
                    var active = (ImageView)_adsCountLayout.GetChildAt(lastVisibleView-1);
                    if (inactive != null && active != null)
                    {
                        inactive.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.round_dot));
                        active.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.round_dot_active));
                    }
                }
                else
                {
                    var inactive = (ImageView)_adsCountLayout.GetChildAt(lastVisibleView-1);
                    var active = (ImageView)_adsCountLayout.GetChildAt(firstVisibleView-1);
                    if (inactive != null && active != null)
                    {
                        inactive.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.round_dot));
                        active.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.round_dot_active));
                    }
                }
            }
        }

        public override void OnActivate(object o = null)
        {

            _progressDialog = ProgressDialog.Show(Activity, null, "Loading...");
            _progressDialog.SetCancelable(true);
            Task.Run(() =>
            {
                var adsLoaded = _adsViewModel.GetBillboards();
                if (adsLoaded)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        _progressDialog.Dismiss();
                        if (mAdapter == null || (_adsViewModel.Advertisments.Count > mAdapter.ItemCount - 2))
                        {
                            mAdapter = new RecyclerAdapter(mRecyclerView, this, _adsViewModel.Advertisments);
                            mRecyclerView.SetAdapter(mAdapter);

                            #region Pager Indicator
                            //_adsCountLayout.RemoveAllViews();
                            //for (int i = 0; i < _adsViewModel.Advertisments.Count; i++)
                            //{
                            //    ImageView img = new ImageView(Activity);
                            //    if (i == 0)
                            //    {
                            //        img.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.round_dot_active));
                            //    }
                            //    else
                            //    {
                            //        img.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.round_dot));
                            //    }
                            //
                            //    LinearLayout.LayoutParams parameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                            //    parameters.SetMargins(5, 0, 5, 0);
                            //    img.LayoutParameters = parameters;
                            //    img.Id = i;
                            //    _adsCountLayout.AddView(img, i);
                            //}
                            #endregion
                        }
                        mRecyclerView.ItemCount = _adsViewModel.Advertisments.Count;
                        if (o != null && o is int)
                        {
                            mRecyclerView.CurrentItem = (int)o;
                            mRecyclerView.ScrollToPosition((int)o + 1);
                        }
                    });
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        _progressDialog.Dismiss();
                        Toast toast = Toast.MakeText(Activity, Resource.String.no_ads, ToastLength.Short);
                        TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                        if (v != null)
                            v.Gravity = GravityFlags.Center;
                        toast.Show();
                    });
                }
            });
        }

        private void ShowDownload()
        {
            _downloadLayout.Visibility = ViewStates.Visible;
        }
        
        public override bool UserVisibleHint
        {
            get
            {
                return base.UserVisibleHint;
            }
            set
            {
                base.UserVisibleHint = value;
                if (!value)
                {
                    if (mAdapter != null)
                    {
                        mAdapter.ResetTimer();
                    }
                }
            }
        }

        public override void OnPause()
        {
            if (mAdapter != null)
            {
                mAdapter.ResetTimer();
            }

            base.OnPause();
        }

        public override void OnResume()
        {
            //if (mAdapter != null)
            //{
            //    mAdapter.InitTimer();
            //}
            base.OnResume();
        }
    }
}