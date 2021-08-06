using System;
using Android.OS;
using Android.Views;
using Android.Widget;

using MvvmCross.Platforms.Android.Binding.BindingContext;
using Kuni.Core.ViewModels;
using Android.Util;
using MvvmCross.Binding.BindingContext;
using MvvmCross;
//using MvvmCross.Droid.Views.Fragments;

namespace Kunicardus.Droid.Fragments
{
    public class NewsDetailsFragment : BaseMvxFragment
    {
        NewsDetailsViewModel _ViewModel;
        BindableWebView webView;
        int _newsId;

        TextView description, date;

        public NewsDetailsFragment(int newsId)
        {
            ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCProvider.IoCConstruct<NewsDetailsViewModel>();
            _ViewModel = (NewsDetailsViewModel)ViewModel;
            _newsId = newsId;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var View = this.BindingInflate(Resource.Layout.NewsDetailsView, null);

            View.FindViewById<TextView>(Resource.Id.pageTitle).Text = Resources.GetString(Resource.String.news);

            _ViewModel.GetNewsById(_newsId);

            webView = View.FindViewById<BindableWebView>(Resource.Id.webView1);
            webView.Settings.SetLayoutAlgorithm(Android.Webkit.WebSettings.LayoutAlgorithm.SingleColumn);

            DisplayMetrics metrics = new DisplayMetrics();
            this.Activity.WindowManager.DefaultDisplay.GetMetrics(metrics);


            webView.SetInitialScale(GetScale());
            webView.SetPadding(0, 0, 0, 0);
            this.CreateBinding(this).For(v => v.Description).To((NewsDetailsViewModel vm) => vm.Description).Apply();

            var backButton = View.FindViewById<ImageView>(Resource.Id.partner_details_back);
            backButton.Click += delegate
            {
                ((MainView)Activity).ClearBackStack();
            };

            return View;
        }

        int PIC_WIDTH = 500;

        private int GetScale()
        {
            int width = this.Activity.WindowManager.DefaultDisplay.Width;
            Double val = (Double)width / (Double)PIC_WIDTH;
            val = val * 100d;
            return (int)val;
        }

        public string Description
        {
            get { return string.Empty; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    webView.LoadDataWithBaseURL("file:///android_asset/", value, "text/html", "UTF-8", null);
                }
            }
        }

    }
}