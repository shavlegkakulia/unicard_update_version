using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Kuni.Core.ViewModels;
using Kunicardus.Droid.Adapters;
//using MvvmCross.Binding.Droid.Views;
using Android.Graphics;
using Kunicardus.Droid.Views;
using Android.Support.V4.Widget;
//using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross;

namespace Kunicardus.Droid.Fragments
{
    public class NewsListFragment : BaseMvxFragment, AdapterView.IOnItemClickListener
    {
        private MainView _mainView;
        private ImageButton _menu;
        private NewsListViewModel _ViewModel;

        SwipeRefreshLayout _refresher;

        public NewsListFragment()
        {
            ViewModel = Mvx.IoCProvider.IoCConstruct<NewsListViewModel>();
            _ViewModel = (NewsListViewModel)ViewModel;
        }

        public bool DataPopulated
        {
            set
            {
                if (value)
                {
                    OnRefreshFinished();
                }
            }
            get { return false; }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var View = this.BindingInflate(Resource.Layout.NewsListView, null);

            _menu = View.FindViewById<ImageButton>(Resource.Id.menuImg);
            _menu.Click += (o, e) => _mainView.ShowMenu();

            var title = View.FindViewById<TextView>(Resource.Id.pageTitle);
            title.Text = Resources.GetString(Resource.String.news);
            View.FindViewById<ImageView>(Resource.Id.alert).Visibility = ViewStates.Invisible;
            View.FindViewById<ImageView>(Resource.Id.merchants).Visibility = ViewStates.Invisible;

            _refresher = View.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            _refresher.SetBackgroundColor(Color.ParseColor("#e2e3e3"));
            _refresher.Refresh += OnRefresh;
            _refresher.Refreshing = true;

            var list = View.FindViewById<MvxListView>(Resource.Id.newsListView);
            list.OnItemClickListener = this;
            list.Adapter = new NewsListViewAdapter(this.Activity, (IMvxAndroidBindingContext)BindingContext);
            var set = this.CreateBindingSet<NewsListFragment, NewsListViewModel>();
            set.Bind(this).For(v => v.DataPopulated).To(vmod => vmod.DataPopulated);
            set.Apply();

            return View;
        }

        public override void OnViewCreated(View View, Bundle savedInstanceState)
        {
            base.OnViewCreated(View, savedInstanceState);
        }

        void OnRefresh(object sender, EventArgs e)
        {
            _ViewModel.RefreshNewsList();
        }

        private void OnRefreshFinished()
        {
            _refresher.Refreshing = false;
        }

        public override void OnAttach(Activity activity)
        {
            if (activity != null)
            {
                _mainView = ((MainView)activity);
            }
            base.OnAttach(activity);
        }

        public override void OnActivate()
        {
            _refresher.Post(delegate
            {
                _refresher.Refreshing = true;
            });
            _ViewModel.GetNewsList();
            _ViewModel.RefreshNewsList();
        }

        public void OnItemClick(AdapterView parent, View View, int position, long id)
        {
            try
            {
                var newsId = _ViewModel.GetNewsId(position);
                _ViewModel.MarkAsRead(newsId);
                Intent intent = new Intent(this.Activity, typeof(NewsDetailsView));
                intent.PutExtra("newsId", newsId);
                intent.AddFlags(ActivityFlags.NoAnimation);
                _mainView._isInnerActivity = true;
                StartActivity(intent);
                Activity.OverridePendingTransition(Resource.Animation.slide_in, Resource.Animation.slide_out);
            }
            catch
            {
                Toast.MakeText(Activity, Resource.String.error_occured, ToastLength.Long).Show();
            }
        }
    }
}