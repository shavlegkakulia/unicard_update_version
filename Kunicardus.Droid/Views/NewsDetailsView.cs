using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Kuni.Core.ViewModels;
using Kuni.Core.Models.DB;
using Android.Util;
using MvvmCross;
using MvvmCross.ViewModels;
using MvvmCross.Binding.BindingContext;
using Android.Views.InputMethods;
using Android.Content;
using Android.Views;
using MvvmCross.Platforms.Android.Views;

namespace Kunicardus.Droid.Views
{
	[Activity (Label = "UNICARD",
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
		LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
	[MvxViewFor (typeof(NewsDetailsViewModel))]
	public class NewsDetailsView : MvxActivity
	{
		NewsDetailsViewModel _ViewModel;
		NewsInfo _news;
		BindableWebView webView;
		int _newsId;

		TextView header, description, date;

		public string Description {
			get { return string.Empty; }
			set {
				if (!string.IsNullOrEmpty (value)) {
					webView.LoadDataWithBaseURL ("file:///android_asset/", value, "text/html", "UTF-8", null);
				}
			}
		}

		public NewsDetailsView ()
		{
			ViewModel = Mvx.IoCConstruct<NewsDetailsViewModel> ();
			_ViewModel = (NewsDetailsViewModel)ViewModel;
			//_news = _ViewModel.GetNewsById(newsId);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.NewsDetailsView);

			_newsId = Intent.GetIntExtra ("newsId", 0);

			FindViewById<TextView> (Resource.Id.pageTitle).Text = Resources.GetString (Resource.String.news);

			_ViewModel.GetNewsById (_newsId);

			webView = FindViewById<BindableWebView> (Resource.Id.webView1);
			webView.Settings.SetLayoutAlgorithm (Android.Webkit.WebSettings.LayoutAlgorithm.SingleColumn);

			DisplayMetrics metrics = new DisplayMetrics ();
			WindowManager.DefaultDisplay.GetMetrics (metrics);


			webView.SetInitialScale (GetScale ());
			webView.SetPadding (0, 0, 0, 0);
            //todo
			this.CreateBinding (this).For (v => v.Description).To ((NewsDetailsViewModel vm) => vm.Description).Apply ();

			var backButton = FindViewById<ImageView> (Resource.Id.partner_details_back);
			backButton.Click += delegate {
				this.Finish ();
			};
		}

		bool _appWasInBackground;

		DateTime _lastDateTime;

		public bool _pinIsOpened;

		protected override void OnPause ()
		{
			_appWasInBackground = true;
			_lastDateTime = DateTime.Now;
			base.OnPause ();
		}

		protected override void OnResume ()
		{
			if (_appWasInBackground) {
				_appWasInBackground = false;

				var secondsDifference = DateTime.Now.Ticks / TimeSpan.TicksPerSecond - _lastDateTime.Ticks / TimeSpan.TicksPerSecond;
				if (secondsDifference > 60 && !_pinIsOpened) {
					if (_ViewModel.UserSettings != null && _ViewModel.UserSettings.Pin != null) {
						OpenPinInputDialog (_ViewModel.UserSettings.Pin);
						HideKeyboard ();
					}
				}
			}
			base.OnResume ();
		}

		public  void OpenPinInputDialog (string pinFromDB)
		{
			_pinIsOpened = true;
			PinInputDialogFragment d = new PinInputDialogFragment (pinFromDB, false, true);
			d.Show (FragmentManager, "");
			d.SetStyle (DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
			d.Cancelable = false;
		}

		private void HideKeyboard ()
		{
			View view = base.CurrentFocus;
			if (view != null) {
				InputMethodManager inputManager = (InputMethodManager)this.GetSystemService (Context.InputMethodService);
				inputManager.HideSoftInputFromWindow (view.WindowToken, HideSoftInputFlags.None);
			}
//			searchText.ClearFocus ();
		}


		int PIC_WIDTH = 500;

		private int GetScale ()
		{
			int width = WindowManager.DefaultDisplay.Width;
			Double val = (Double)width / (Double)PIC_WIDTH;
			val = val * 100d;
			return (int)val;
		}
	}
}