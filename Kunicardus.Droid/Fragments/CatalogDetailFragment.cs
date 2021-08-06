using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Kuni.Core.ViewModels;
using Kuni.Core.Models;
using MvvmCross;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Support.V4.View;
using Kuni.Core;

using MvvmCross.Binding.BindingContext;

namespace Kunicardus.Droid
{
	public class CatalogDetailFragment : MvxFragment
	{

		#region Private Variables

		private BaseExpandableListView _userDiscountsList;
		private LinearLayout _oldPriceLayout, _discountPercentLayout, _discountedPointsLayout;
		public Dialog dialog;
		private Android.Support.V4.App.FragmentManager supportFragmentManager;
		private MainView _currentActivity;
		CatalogDetailViewModel _currentViewModel;

		#endregion

		#region Constructor Implementation



		public CatalogDetailFragment (int productId)
		{
			this.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCConstruct<CatalogDetailViewModel>();
			_currentViewModel = (this.ViewModel as CatalogDetailViewModel);
			((CatalogDetailViewModel)ViewModel).InitialiseData (productId);
		}

		#endregion

		#region Fragment Native Methods

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			supportFragmentManager = this.Activity.SupportFragmentManager;
		}

		BaseButton _btnBuy;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var view = this.BindingInflate (Resource.Layout.CatalogDetailView, null);
			_currentActivity = this.Activity as MainView;
			_userDiscountsList = view.FindViewById<BaseExpandableListView> (Resource.Id.user_discount_list);
			_userDiscountsList.setExpanded (true);

			var webview = view.FindViewById<BindableWebView> (Resource.Id.product_description);
			webview.Settings.SetLayoutAlgorithm (Android.Webkit.WebSettings.LayoutAlgorithm.SingleColumn);

			_btnBuy = view.FindViewById<BaseButton> (Resource.Id.btnBuy);
			_btnBuy.Click += (o, e) => OnPaymentClick ();
            
			ImageView img = view.FindViewById<ImageView> (Resource.Id.imgMainc);
			img.Click += (o, e) => ImageClicked ();

			var backButton = view.FindViewById<ImageButton> (Resource.Id.catalog_detail_back);
			backButton.Click += (sender, e) => {
				ChildFragmentManager.PopBackStack ();
				((MainView)Activity).SupportFragmentManager.PopBackStack ();
			};

			_discountPercentLayout = view.FindViewById<LinearLayout> (Resource.Id.product_discount_percent_layout);
			_oldPriceLayout = view.FindViewById<LinearLayout> (Resource.Id.oldprice_layout);
			_discountedPointsLayout = view.FindViewById<LinearLayout> (Resource.Id.product_discount_percent);

			var set = this.CreateBindingSet<CatalogDetailFragment, CatalogDetailViewModel> ();
			set.Bind (this).For (x => x.DataPopulated).To (vm => vm.DataPopulated);
			set.Apply (); 

			HideAllLayouts ();
			return view;
		}

		public bool DataPopulated {
			get { return true; }
			set { 
				if (value) {
					HideOldPriceMessage ();
					if (_currentActivity._dialog != null) {
						_currentActivity._dialog.Dismiss ();
					}
				}
			}
		}

		#endregion

		#region Methods

		bool dialogWasOpened = false;

		private void ImageClicked ()
		{
			if (!dialogWasOpened) {
				dialogWasOpened = true;
				if (_currentViewModel.ImageUrls == null) {
					return;
				}

				dialog = new Dialog (_currentActivity,
					Android.Resource.Style.ThemeBlackNoTitleBarFullScreen);

				dialog.SetCancelable (true);
				dialog.SetContentView (Resource.Layout.ImageSliderView);
				dialog.Show ();
				_currentActivity._dialog.Show ();
				var imageViewer = dialog.FindViewById<ViewPager> (Resource.Id.pager);
				var dialogView = dialog.FindViewById<RelativeLayout> (Resource.Id.dialog_main_relative_layout);
				var closeButton = dialogView.FindViewById<ImageButton> (Resource.Id.close_image_slider);
				closeButton.Click += (sender, e) => {
					dialog.Dismiss ();
					imageViewer.Adapter.Dispose ();
				};
				dialog.DismissEvent += (dismissSender, dismissE) => {
					dialogWasOpened = false;
				};
				imageViewer.Adapter = new ImageSliderAdapter (_currentActivity, _currentViewModel.ImageUrls);
				imageViewer.OffscreenPageLimit = _currentViewModel.ImageUrls.Count - 1;
				var dialogProductName = dialog.FindViewById<BaseTextView> (Resource.Id.catalog_dialog_product_name);
				dialogProductName.Text = _currentViewModel.ProductName;
			}
		}

		private void OnPaymentClick ()
		{
			if (_currentViewModel == null || _currentViewModel.DeliveryMethods == null) {
				return;
			}
		
			if (_currentViewModel.DeliveryMethods.Count == 1) {
				_currentActivity._dialog.Show ();
				Task.Run (async () => {
					var bpvm = Mvx.IoCConstruct<BuyProductViewModel> ();
					bpvm.InitViewModel	(_currentViewModel.CurrentImageUrl, _currentViewModel.ProductName, _currentViewModel.ProductId, 
						_currentViewModel.ProductPrice, _currentViewModel.DeliveryMethods.FirstOrDefault ().DeliveryMethodId,
						_currentViewModel.ProductTypeID, _currentViewModel.DeliveryMethods.FirstOrDefault ().Note, _currentViewModel.UserDiscounts);

					BuyProductFragment fragment = new BuyProductFragment (bpvm);

					var fragmentTransaction = supportFragmentManager.BeginTransaction ();
					fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
					fragmentTransaction.Replace (Resource.Id.main_fragment, fragment).Commit ();
				});

				return;
			}

			dialog = new Dialog (_currentActivity,
				Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
			dialog.SetContentView (Resource.Layout.DeliveryTypeDialogView);
			dialog.SetCancelable (true);
			dialog.Show ();
			Task.Run (async () => {
				LinearLayout ly = new LinearLayout (dialog.Context);
				float scale = Resources.DisplayMetrics.Density;
				int dpAsPixels = (int)(15 * scale + 0.5f);
				ly.SetPadding (dpAsPixels, 0, dpAsPixels, 0);

				int btnMarginBt = (int)(4 * scale + 0.5f);
				int btnpadding = (int)(10 * scale + 0.5f);
				if (_currentViewModel.DeliveryMethods != null) {
					foreach (var item in  _currentViewModel.DeliveryMethods) {
						var bt = new BaseButton (_currentActivity);
						var btparamas = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent,
							                ViewGroup.LayoutParams.WrapContent);
						btparamas.SetMargins (0, btnMarginBt, 0, btnMarginBt);
						bt.SetPadding (0, btnpadding, 0, btnpadding);
						bt.SetMinimumHeight (100);
						bt.Text = item.Name;
						bt.SetHintTextColor (Color.White);
						bt.SetTextSize (ComplexUnitType.Sp, 15);
						bt.Gravity = GravityFlags.Center;
						bt.SetBackgroundResource (Resource.Drawable.delivery_type_btn_bg);
						bt.LayoutParameters = btparamas;
						InitButton (bt, _currentViewModel.CurrentImageUrl, _currentViewModel.ProductName, _currentViewModel.ProductId, 
							_currentViewModel.ProductPrice, item.DeliveryMethodId, _currentViewModel.ProductTypeID, item.Note, _currentViewModel.UserDiscounts);
						ly.AddView (bt);
					}

					var param = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent,
						            ViewGroup.LayoutParams.WrapContent);
					ly.Orientation = Orientation.Vertical;
					ly.LayoutParameters = param;

					_currentActivity.RunOnUiThread (() => {
						dialog.FindViewById<ProgressBar> (Resource.Id.progress).Visibility = ViewStates.Gone;
						dialog.FindViewById<LinearLayout> (Resource.Id.lycontainer).AddView (ly);
					});

				}
			});
//			dialog.Show ();
		}

		private void HideAllLayouts ()
		{
			_discountedPointsLayout.Visibility = ViewStates.Visible;
			_discountPercentLayout.Visibility = ViewStates.Invisible;
			_discountedPointsLayout.Visibility = ViewStates.Invisible;
			_discountPercentLayout.Visibility = ViewStates.Invisible;
		}

		private void InitButton (Button bt, string url, string name, int pID, int price,
		                         int methodId, int ptypeID, string note, List<DiscountModel> discounts)
		{
			bt.Click += (o, e) => {
				var bpvm = Mvx.IoCConstruct<BuyProductViewModel> ();
				bpvm.InitViewModel	(url,
					name, pID, price, 
					methodId, ptypeID, note, discounts);
				BuyProductFragment fragment = new BuyProductFragment (bpvm);

				var fragmentTransaction = supportFragmentManager.BeginTransaction ();

				fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
//				fragmentTransaction.AddToBackStack (null);
				fragmentTransaction.Replace (Resource.Id.detailsContent, fragment).Commit ();
				dialog.Dismiss ();
			};
		}

		private void HideOldPriceMessage ()
		{
			_discountedPointsLayout.Visibility = ViewStates.Visible;
			if (_currentViewModel.HideOldPrice) {
				_oldPriceLayout.Visibility = ViewStates.Invisible;
			} else
				_oldPriceLayout.Visibility = ViewStates.Visible;
			if (_currentViewModel.HideDiscountPercent) {
				_discountPercentLayout.Visibility = ViewStates.Invisible;
			} else
				_discountPercentLayout.Visibility = ViewStates.Visible;

			if (_currentViewModel.HideDiscountedPoints) {
				_discountedPointsLayout.Visibility = ViewStates.Invisible;
				_discountPercentLayout.Visibility = ViewStates.Invisible;
			} else if (!_currentViewModel.HideDiscountPercent) {
				_discountedPointsLayout.Visibility = ViewStates.Visible;
				_discountPercentLayout.Visibility = ViewStates.Visible;
			}
				
			#region Retreiving User Discounts Data
			var discounts = _currentViewModel.UserDiscounts;
			var currentPrice = (this.ViewModel as CatalogDetailViewModel).ProductPrice;
			if (discounts != null && discounts.Count != 0) {
				_userDiscountsList.SetAdapter (new UserDiscountsExpanadableListAdapter (this.Activity, discounts, Convert.ToInt32 (currentPrice)));
				_userDiscountsList.Visibility = ViewStates.Visible;
			} else {
				_userDiscountsList.Visibility = ViewStates.Invisible;
				_userDiscountsList.Visibility = ViewStates.Gone;
			}
			#endregion

		}

		#endregion

	}
}

