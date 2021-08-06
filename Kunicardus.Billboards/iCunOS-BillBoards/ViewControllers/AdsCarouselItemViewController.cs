using System;
using UIKit;
using Kunicardus.Billboards.Core.ViewModels;
using CoreGraphics;
using CoreText;

namespace iCunOS.BillBoards
{
	public class AdsCarouselItemViewController : UIViewController
	{
		public int Index {
			get;
			set;
		}

		public AdvertismentViewModel ViewModel {
			get;
			set;
		}

		public AdsCarouselItemViewController (int index, AdvertismentViewModel viewModel)
		{
			this.Index = index;
			this.ViewModel = viewModel;
		}

		public AdViewWrapper Wrapper {
			get;
			set;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Wrapper = new AdViewWrapper (new CGRect (0, 0, this.View.Frame.Width, View.Frame.Height));
			Wrapper.Adview = new AdView (new CGRect (new CGPoint ((this.View.Frame.Width - AdView.DefaultSize.Width) / 2.0f,
				0f
			), AdView.DefaultSize));
			Wrapper.AddSubview (Wrapper.Adview);

			View.AddSubview (Wrapper);
			Wrapper.Adview.OnSkip += SkipAd;
		}

		void SkipAd (object sender, bool e)
		{
			if (e) {
				var uiPageViewController = (this.ParentViewController as AdsViewController);
				if (uiPageViewController != null)
					uiPageViewController.SkipTheAd (this.Index);
			}
		}
	}
}

