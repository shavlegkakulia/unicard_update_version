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
using Kunicardus.Billboards.Core.ViewModels;
using Kunicardus.Billboards.Core.Services;
using Kunicardus.Billboards.Adapters;
using System.Threading.Tasks;
using Kunicardus.Billboards.Plugins;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Activities;
using Autofac;
using Android.Support.V4.Widget;
using Android.Graphics;

namespace Kunicardus.Billboards.Fragments
{
    public class HistoryFragment : BaseFragment
    {
        ListView _adsList;
        HistoryViewModel _viewModel;
        AdsHistoryAdapter _adapter;
        SwipeRefreshLayout _refresher;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.HistoryLayout, null);
            using (var scope = App.Container.BeginLifetimeScope())
            {
                _viewModel = scope.Resolve<HistoryViewModel>();
            }


            _refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            _refresher.SetBackgroundColor(Color.ParseColor("#e2e3e3"));
            _refresher.Refresh += OnRefresh;
            _refresher.Refreshing = true;

            _adsList = view.FindViewById<ListView>(Resource.Id.adsListView);
            return view;
        }

        void OnRefresh(object sender, EventArgs e)
        {
            OnActivate();
        }

        public override void OnActivate(object o = null)
        {
            Task.Run(() =>
                {
                    var success = _viewModel.GetAdvertisments();
                    if (success)
                    {
                        _adapter = new AdsHistoryAdapter(Activity, _viewModel.Advertisments);
                        Activity.RunOnUiThread(() =>
                            {
                                _adsList.Adapter = _adapter;
                                _refresher.Refreshing = false;
                            });
                    }
                });
        }
    }
}