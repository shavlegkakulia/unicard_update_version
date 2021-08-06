using System;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Kuni.Core;
using Kunicardus.Droid.Adapters;
using MvvmCross;
using Android.Graphics;
using Android.Support.V4.Widget;
//using BindingInflate =  MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Kunicardus.Droid
{
    public class MyPageFragment : BaseMvxFragment
    {
        ImageButton _menu;

        //View uniCard;
        MyPageViewModel _viewmodel;

        SwipeRefreshLayout _refresher;
        ListView listView;


        public bool IsCardActive { get; set; }

        public bool IsAnimInProgress { get; set; }

        public bool IsReplaced { get; set; }

        public bool DataPopulated
        {
            set
            {
                if (value)
                {
                    _refresher.Refreshing = false;
                    InitListView();
                }
            }
            get { return false; }
        }

        private void InitListView()
        {
            if (_viewmodel.Transactions != null && _viewmodel.Transactions.Count >= 0 && _viewmodel.TransactionsUpdated)
                listView.Adapter = new TransfersListViewAdapter(this.Activity, _viewmodel.Transactions, (IMvxAndroidBindingContext)BindingContext);
        }

        public MyPageFragment()
        {
            ViewModel = Mvx.IoCProvider.IoCConstruct<MyPageViewModel>();
            _viewmodel = (MyPageViewModel)ViewModel;
        }

        public override void OnActivate()
        {
            _refresher.Post(delegate
            {
                _refresher.Refreshing = true;
            });
            _viewmodel.GetData();
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var View = this.BindingInflate(Resource.Layout.MyPageView, null);

            var alert = View.FindViewById<ImageButton>(Resource.Id.alert);
            alert.Visibility = ViewStates.Gone;
            View.FindViewById<ImageView>(Resource.Id.merchants).Visibility = ViewStates.Gone;

            _refresher = View.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            _refresher.SetBackgroundColor(Color.ParseColor("#e2e3e3"));
            _refresher.Refresh += OnRefresh;

            _menu = View.FindViewById<ImageButton>(Resource.Id.menuImg);
            _menu.Click += (o, e) => ((MainView)base.Activity).ShowMenu();
            
            var set = this.CreateBindingSet<MyPageFragment, MyPageViewModel>();
            set.Bind(this).For(v => v.DataPopulated).To(vmod => vmod.DataPopulated);
            set.Apply();

            listView = View.FindViewById<ListView>(Resource.Id.transfersListView);
            var header = this.BindingInflate(Resource.Layout.MyPageHeader, null);
            listView.AddHeaderView(header);

            InitListView();

            return View;
        }

        void OnRefresh(object sender, EventArgs e)
        {
            _viewmodel.GetData();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnPause()
        {
            base.OnPause();
        }
    }
}

