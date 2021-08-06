using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using System.Runtime.Remoting.Messaging;
using Kunicardus.Billboards.Core.ViewModels;

namespace iCunOS.BillBoards
{
	public class PageDataSource : UIPageViewControllerDataSource
	{
		readonly List<AdsCarouselItemViewController> _pages;
		readonly List<AdvertismentViewModel> _models;

		public AdsCarouselItemViewController OldPage { get; set; }

		public AdsCarouselItemViewController CurrentPage { get; set; }

		public PageDataSource (List<AdsCarouselItemViewController> pages, List<AdvertismentViewModel> models)
		{
			_pages = pages;
			_models = models;
		}

		override public UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			CurrentPage = referenceViewController as AdsCarouselItemViewController;
			if (CurrentPage != null) {
				if (CurrentPage.Wrapper.Adview.ViewModel == null)
					CurrentPage.Wrapper.Adview.ViewModel = _models [CurrentPage.Index];
				
				if (OldPage != null)
					OldPage.Wrapper.Adview.IsActive = false;
				CurrentPage.Wrapper.Adview.IsActive = true;

				OldPage = CurrentPage;

				if (CurrentPage.Index == 0) {
					return null;
				} else {
					return _pages [CurrentPage.Index - 1];
				}
			} else
				return null;
		}

		override public UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			CurrentPage = referenceViewController as AdsCarouselItemViewController;
			if (CurrentPage != null) {
				if (CurrentPage.Wrapper.Adview.ViewModel == null)
					CurrentPage.Wrapper.Adview.ViewModel = _models [CurrentPage.Index];
			
				if (CurrentPage.Index != 0) {
					if (OldPage != null)
						OldPage.Wrapper.Adview.IsActive = false;
					CurrentPage.Wrapper.Adview.IsActive = true;
					OldPage = CurrentPage;
				}

			
				if (CurrentPage.Index == _pages.Count - 1) {
					return null;
				}
				return _pages [(CurrentPage.Index + 1)];
			} else
				return null;
		}

		public override nint GetPresentationCount (UIPageViewController pageViewController)
		{
			return _pages.Count;
		}

	}
}

