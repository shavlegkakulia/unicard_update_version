using System;
using UIKit;
using Foundation;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Kunicardus.Core.ViewModels.iOSSpecific;
using System.Collections.Generic;
using Kunicardus.Core.Models.DB;
using System.Net.NetworkInformation;
using System.ComponentModel.Design;
using Kunicardus.Core;

namespace Kunicardus.Touch
{
	public class ProductDelegate : NSObject,IUICollectionViewDelegate,IDisposable
	{
		public new IntPtr Handle {
			get {
				return new IntPtr ();
			}
		}

		public new void Dispose ()
		{
			this.Dispose ();
		}

		iCatalogListViewModel _viewmodel;
		UIActivityIndicatorView _indicator;

		public ProductDelegate (iCatalogListViewModel viewmodel, UIActivityIndicatorView indicator)
		{
			_viewmodel = viewmodel;
			_indicator = indicator;
		}

		[Export ("scrollViewDidScroll:")]
		public void Scrolled (UIKit.UIScrollView scrollView)
		{
			if (scrollView.ContentOffset.Y > scrollView.ContentSize.Height - scrollView.Frame.Height - 100f && !_viewmodel.IsLoading) {
				_indicator.StartAnimating ();
				_viewmodel.PageIndex += 1;
				_viewmodel.LoadMore ();
				_viewmodel.IsLoading = true;
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			}
		}


		[Foundation.Export ("collectionView:didSelectItemAtIndexPath:")]
		public void ItemSelected (UIKit.UICollectionView collectionView, Foundation.NSIndexPath indexPath)
		{
			var data = ((MvxCollectionViewSource)collectionView.DataSource).ItemsSource;
			var item = ((List<ProductsInfo>)data) [(int)indexPath.Item];
			_viewmodel.ProductClick (item);
		}
	}
}

