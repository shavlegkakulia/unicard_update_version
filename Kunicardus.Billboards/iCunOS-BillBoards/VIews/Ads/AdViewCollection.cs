using System;
using Kunicardus.Billboards.Core.ViewModels;
using UIKit;
using CoreGraphics;
using HomeKit;
using System.Collections.Generic;
using Foundation;

namespace iCunOS.BillBoards
{
	public class AdViewCollection : UIView
	{
		//		AdsListViewModel _viewModel;
		//		AdView _oldAdView;
		//		UIScrollView _scrollView;
		//		CGRect _frame;
		//		List<AdView> _adViews;

		public AdViewCollection (CGRect frame, AdsListViewModel viewModel, nfloat statusBarHeight) : base (frame)
		{
//			_viewModel = viewModel;
//			_frame = frame;
//			_adViews = new List<AdView> ();
//			_scrollView = new UIScrollView (new CGRect (0, 0, _frame.Width, _frame.Height));
//			_scrollView.DecelerationRate = UIScrollView.DecelerationRateFast;
//			nfloat contentWidth = 0f;
//			for (int i = 0; i < _viewModel.Advertisments.Count; i++) {
//				
//				//adding advertisements
//				AdViewWrapper wrapper = new AdViewWrapper (new CGRect ((i * _frame.Width), 0, _frame.Width, _frame.Height), _viewModel.Advertisments [i]);
//				wrapper.Adview = new AdView (new CGRect (new CGPoint ((_frame.Width - AdView.DefaultSize.Width) / 2.0f, 
//					(Frame.Height - AdView.DefaultSize.Height - statusBarHeight) / 2.0f
//				), AdView.DefaultSize));
//
//				_adViews.Add (wrapper.Adview);
//				wrapper.AddSubview (wrapper.Adview);
//
//				wrapper.Adview.IsActive |= i == 0;
//				
//				_scrollView.AddSubview (wrapper);
//				if (i == _viewModel.Advertisments.Count - 1)
//					contentWidth = wrapper.Frame.Right;
//			}
//
//			_scrollView.ContentSize = new CGSize (contentWidth, frame.Height);
//			AddSubview (_scrollView);
		}

	}
}

