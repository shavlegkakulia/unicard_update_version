using System;
using UIKit;
using System.Collections.Generic;

namespace Kunicardus.Touch
{
	public class ImagesPageDataSource : UIPageViewControllerDataSource
	{
		List<ImageItemViewController> _pages;

		public ImageItemViewController CurrentPage { get; set; }

		public ImagesPageDataSource (List<ImageItemViewController> pages)
		{
			_pages = pages;
		}

		public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			CurrentPage = referenceViewController as ImageItemViewController;
			if (CurrentPage != null) {
				if (CurrentPage.Index == _pages.Count - 1) {
					return null;
				}
				return _pages [(CurrentPage.Index + 1)];
			} else
				return null;
		}

		public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			CurrentPage = referenceViewController as ImageItemViewController;
			if (CurrentPage != null) {
				if (CurrentPage.Index == 0) {
					return null;
				} else {
					return _pages [CurrentPage.Index - 1];
				}
			} else
				return null;
		}

		public override nint GetPresentationCount (UIPageViewController pageViewController)
		{
			return _pages.Count;
		}
	}
}

