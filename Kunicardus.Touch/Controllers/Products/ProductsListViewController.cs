using System;
using Cirrious.MvvmCross.Touch.Views;
using UIKit;
using Kunicardus.Core.ViewModels;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using Foundation;
using ObjCRuntime;
using Kunicardus.Core.ViewModels.iOSSpecific;
using Kunicardus.Touch.Helpers.UI;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Xml.Schema;
using System.Collections.ObjectModel;

namespace Kunicardus.Touch
{
	public class ProductsListViewController : MvxCollectionViewController
	{
		#region Props

		public new iCatalogListViewModel ViewModel {
			get { return (iCatalogListViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public bool DataPopulated {
			get{ return true; }
			set {
				if (value) {
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					RefreshControl.EndRefreshing ();
					CollectionView.ReloadData ();
					if (_filtered) {
						CollectionView.ScrollRectToVisible (new CGRect (0, 0, 1, 1), false);
						_filtered = false;
					}
					if (_removeFilter == null) {
						InitRemoveFilterButton ();
					}
					if (ViewModel.FilterInfo.IsFiltered) {
						View.AddSubview (_removeFilter);
						_filterNavigationButton.Image = _filteredIcon;
					} else {
						_removeFilter.RemoveFromSuperview ();
					}
					if (_indicator != null && _indicator.IsAnimating)
						_indicator.StopAnimating ();
					ViewModel.IsLoading = false;
				}
			}
		}

		public UIRefreshControl RefreshControl {
			get;
			set;
		}

		#endregion

		#region UI

		private UIImage _filteredIcon, _notFilteredIcon;
		private CatalogFilterView _filter;
		private UIButton _removeFilter;
		private UIBarButtonItem _filterNavigationButton;

		#endregion

		#region Ctors

		public ProductsListViewController () : base (new UICollectionViewFlowLayout () {
				ItemSize = 
				new CGSize (
					((BaseMvxViewController.APP.Window.Frame.Width / 2.0f) - 2), 
					((BaseMvxViewController.APP.Window.Frame.Width / 2.0f) + 40)),
				ScrollDirection = UICollectionViewScrollDirection.Vertical,
				SectionInset = new UIEdgeInsets (0, 2, 0, 2),
				MinimumLineSpacing = 4,
				MinimumInteritemSpacing = 0,
			})
		{
			RefreshControl = new UIRefreshControl ();
		}

		#endregion

		#region Overrides

		UIActivityIndicatorView _indicator;

		public  override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBar.Translucent = false;
			View.BackgroundColor = UIColor.Clear.FromHexString ("#f3f3f3");
			Title = ApplicationStrings.WhereToSpend;
			NavigationController.NavigationBar.TintColor = UIColor.White;
			ShowMenuIcon ();
			InitList ();
			InitRefreshControl ();
			InitFilterControl ();
			InitIndicator ();

			CollectionView.Delegate = new ProductDelegate (ViewModel, _indicator);
			View.AddSubview (_indicator);
		}

		#endregion

		#region Methods

		private void InitIndicator ()
		{
			var indicatorSize = 30f;
			_indicator = new UIActivityIndicatorView (new CGRect (
				100f,
				100f,
				indicatorSize, 
				indicatorSize));
			_indicator.Transform = CGAffineTransform.MakeScale (1.25f, 1.25f);
			_indicator.Center = new CGPoint (View.Center.X, View.Frame.Height - indicatorSize * 2.5f);
			_indicator.BringSubviewToFront (View);
			_indicator.Color = UIColor.Black;
		}

		private void InitRemoveFilterButton ()
		{
			nfloat removeFilterWidth = 180f;
			nfloat removeFilterHeight = 35f;
			_removeFilter = new UIButton (UIButtonType.RoundedRect);
			_removeFilter.Frame = 
				new CGRect (
				(View.Frame.Width - removeFilterWidth) / 2.0f, 
				View.Frame.Height - removeFilterHeight - 60, 
				removeFilterWidth, 
				removeFilterHeight);
			
//			UIView transBG = new UIView (new CGRect (0, 0, _removeFilter.Frame.Width, _removeFilter.Frame.Height));
//			transBG.BackgroundColor = UIColor.Clear.FromHexString ("#484b4c");
//			transBG.Layer.CornerRadius = transBG.Frame.Height / 2.0f;
//			transBG.Alpha = 0.8f;

			_removeFilter.BackgroundColor = UIColor.Clear.FromHexString ("#484b4c");
			_removeFilter.Alpha = 0.9f;
			_removeFilter.Layer.CornerRadius = _removeFilter.Frame.Height / 2.0f;
			_removeFilter.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			_removeFilter.SetTitleColor (UIColor.White, UIControlState.Normal);
			_removeFilter.TintColor = UIColor.White;
			_removeFilter.SetTitle (ApplicationStrings.RemoveFilter, UIControlState.Normal);
			_removeFilter.SetImage (ImageHelper.MaxResizeImage (UIImage.FromBundle ("cross"), 0, 25), UIControlState.Normal);
			_removeFilter.TitleEdgeInsets = 
				new UIEdgeInsets (0, 
				-_removeFilter.ImageView.Frame.Size.Width, 0, _removeFilter.ImageView.Frame.Size.Width);
			_removeFilter.ImageEdgeInsets = 
				new UIEdgeInsets (0, _removeFilter.TitleLabel.Frame.Size.Width + 5, 0, -_removeFilter.TitleLabel.Frame.Size.Width - 5);
			_removeFilter.TouchUpInside += delegate {
				_filterNavigationButton.Image = _notFilteredIcon;
				_filter.Clear ();
				ViewModel.FilterInfo = _filter.FilterInfo;
				_filtered = false;
				CollectionView.ScrollRectToVisible (new CGRect (0, 0, 1, 1), false);
			};
		}

		private UIImage ImageFromView (UIView view)
		{
			UIGraphics.BeginImageContextWithOptions (view.Frame.Size, view.Opaque, 0.0f);
			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return img;
		}

		private bool _filtered;

		private void InitFilterControl ()
		{			
			_filteredIcon = ImageHelper.MaxResizeImage (UIImage.FromBundle ("filter_checked"), 20, 0);
			_notFilteredIcon = ImageHelper.MaxResizeImage (UIImage.FromBundle ("filter"), 20, 0);
			_filter = new CatalogFilterView (ViewModel.FilterInfo);
			BaseMvxViewController.APP.Window.AddSubview (_filter);

			_filter.Filtered += delegate(object sender, ProductFilterInfo e) {
				ViewModel.FilterInfo = e;
				_filtered = true;
			};

			_filterNavigationButton = new UIBarButtonItem (_notFilteredIcon, UIBarButtonItemStyle.Plain, delegate {
				_filter.Open ();
			});
			NavigationItem.RightBarButtonItem = _filterNavigationButton;
		


			Task.Run (() => {
				ViewModel.GetFilterFields ();
				_filter.ProductCategories = ViewModel.ProductCategories;
				_filter.UserTypes = ViewModel.UserTypes;
				_filter.PriceRanges = ViewModel.PriceRanges;
				UIApplication.SharedApplication.InvokeOnMainThread (() => {
					_filter.InitUI ();
				});
			});
		}

		private void InitRefreshControl ()
		{
			RefreshControl.ValueChanged += delegate {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				ViewModel.GetOrFilterProductList (false);
			};
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		private void InitList ()
		{
			CollectionView.Frame = new CGRect (0, CollectionView.Frame.Y + 2, CollectionView.Frame.Width, CollectionView.Frame.Height);
			CollectionView.AddSubview (RefreshControl);
			CollectionView.AlwaysBounceVertical = true;
			CollectionView.BackgroundColor = UIColor.Clear.FromHexString ("#f3f3f3");
			CollectionView.RegisterClassForCell (typeof(ProductCell), ProductCell.Key);
			CollectionView.AllowsSelection = true;
			var source = new MvxCollectionViewSource (CollectionView, ProductCell.Key);
			CollectionView.Source = source;
			var set = this.CreateBindingSet<ProductsListViewController, iCatalogListViewModel> ();
			set.Bind (source).To (vm => vm.Products);
			set.Bind (this).For (x => x.DataPopulated).To (vm => vm.DataPopulated);
			set.Apply ();
			
			CollectionView.ReloadData ();
		}



		private void ShowMenuIcon ()
		{
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			NavigationItem.SetLeftBarButtonItem (
				new UIBarButtonItem (UIImage.FromBundle ("threelines")
					, UIBarButtonItemStyle.Plain
					, (sender, args) => app.SidebarController.ToggleMenu ()), true);

		}

		#endregion
	}
}

