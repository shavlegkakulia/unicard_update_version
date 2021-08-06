using System;
using UIKit;
using CoreGraphics;
using RadialProgress;
using Kunicardus.Billboards.Core.ViewModels;
using Autofac;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Kunicardus.Billboards.Core.Models.DTOs.Response;
using NotificationCenter;
using System.Deployment.Internal;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace iCunOS.BillBoards
{
	public class AdsViewController : UIPageViewController
	{
		#region Variables

		private AdsListViewModel _viewModel;
		private PageDataSource _pageDataSource;
		private int? _historyId;
		private UIBarButtonItem _newsBarButton;
		private KuniBadgeBarButtonItem _newsCounter;

		#endregion

		#region Constructor

		public AdsViewController (UIPageViewControllerTransitionStyle style, 
		                          UIPageViewControllerNavigationOrientation orient, 
		                          UIPageViewControllerSpineLocation spine, int? historyId = null)
			: base (style, orient, spine)
		{
			using (var scope = App.Container.BeginLifetimeScope ()) {
				_viewModel = scope.Resolve<AdsListViewModel> ();
			}
			_historyId = historyId;
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.DarkGray);
			Title = ApplicationStrings.Ads;
			NavigationController.NavigationBar.TintColor = UIColor.White;
			NavigationController.NavigationBar.Translucent = false;

			ShowMenuIcon ();

			_newsCounter = new KuniBadgeBarButtonItem ("adalert", null);
			_newsBarButton = new UIBarButtonItem (ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIBarButtonItemStyle.Plain, null);	

			NavigationItem.RightBarButtonItems = new UIBarButtonItem[] {	
				_newsBarButton
			};

			InitUI ();
		}

		private UIImage ImageFromView (UIView view)
		{
			UIGraphics.BeginImageContextWithOptions (view.Frame.Size, view.Opaque, 0.0f);
			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return img;
		}

		#endregion

		#region Methods

		private List<AdsCarouselItemViewController> pages;

		private void InitUI ()
		{
			var response = _viewModel.GetBillboards ();
			if (response && _viewModel.Advertisments != null) {
				//Title Bar ads icon
				_newsCounter.BadgeCount = _viewModel.Advertisments.Count;
				_newsBarButton.Image = ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
		
				pages = new List<AdsCarouselItemViewController> ();
				for (int i = 0; i < _viewModel.Advertisments.Count; i++) {
					pages.Add (new AdsCarouselItemViewController (i, _viewModel.Advertisments [i]));
				}

				//Specify the data source for the pages
				_pageDataSource = new PageDataSource (pages, _viewModel.Advertisments);
				DataSource = _pageDataSource;

				//setting first page active
				if (pages.Count > 0) {
					var index = 0;
					AdsCarouselItemViewController page = pages [0];
					if (_historyId != null) {
						page = pages.Where (x => x.ViewModel?.Advertisment?.HistoryId == _historyId).FirstOrDefault ();
						index = page.Index;
					} 
					//Set the initial content (first pages)
					SetViewControllers (new UIViewController[] { page }, UIPageViewControllerNavigationDirection.Forward, false, s => {
					});
					pages [index].Wrapper.Adview.ViewModel = _viewModel.Advertisments [index];
					pages [index].Wrapper.Adview.IsActive = true;
					_pageDataSource.OldPage = pages [index];
				}
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			if (_pageDataSource?.CurrentPage?.Wrapper?.Adview?._progress != null) {
				_pageDataSource.CurrentPage.Wrapper.Adview._progress.Reset ();
			}
			if (pages?.Count > 0)
			if (pages [0]?.Wrapper?.Adview != null && pages [0].Wrapper.Adview.IsActive)
			if (pages [0].Wrapper.Adview._progress != null)
				pages [0].Wrapper.Adview._progress.Reset ();
		}

		protected void ShowMenuIcon ()
		{
			var app = AppDelegate.Instance;
			NavigationItem.SetLeftBarButtonItem (
				new UIBarButtonItem (UIImage.FromBundle ("threelines")
						, UIBarButtonItemStyle.Plain
						, (sender, args) => app.SidebarController.ToggleMenu ()), true);

			UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment (new UIOffset (0, 0), UIBarMetrics.Default);

		}

		public void SkipTheAd (int index)
		{	
			_newsCounter.BadgeCount--;
			_newsBarButton.Image = ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);

			AdsCarouselItemViewController controller = null;
			int nextIndex;
			if (pages.Count > 1) {
				if (index < pages.Count - 1) {
					controller = pages [index + 1];
				} else {
					controller = pages [index - 1];
				}
			}

			pages.RemoveAt (index);
			_viewModel.Advertisments.RemoveAt (index);

			for (int i = 0; i < pages.Count; i++)
				pages [i].Index = i;

			if (pages.Count == 0) {
				SetViewControllers (new UIViewController[] { new UIViewController () }, UIPageViewControllerNavigationDirection.Forward, true, s => {
				});
				return;
			}
			SetViewControllers (new UIViewController[] { controller }, UIPageViewControllerNavigationDirection.Forward, true, s => {
			});
			controller.Wrapper.Adview.ViewModel = _viewModel.Advertisments [controller.Index];
			controller.Wrapper.Adview.IsActive = true;
		}

		#endregion
	}
}

