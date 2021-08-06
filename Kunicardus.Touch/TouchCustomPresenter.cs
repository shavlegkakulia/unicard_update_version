using System;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Exceptions;
using Cirrious.MvvmCross.Views;


namespace Kunicardus.Touch
{
	public class TouchCustomPresenter : MvxTouchViewPresenter
	{
		//private UIWindow _window;
		private IMvxTouchViewCreator _viewCreator;

		public TouchCustomPresenter (UIApplicationDelegate applicationDelegate, UIWindow window) : base (applicationDelegate, window)
		{
			// specialized construction logic goes here
			//_window = window;
		}

		public bool IsStuck { get; set; }

		protected IMvxTouchViewCreator ViewCreator {
			get { return _viewCreator ?? (_viewCreator = Mvx.Resolve<IMvxTouchViewCreator> ()); }
		}

		public override void Show (MvxViewModelRequest request)
		{
			if (IsStuck) {
				MvxTrace.Trace (MvxTraceLevel.Warning, "Can not show - View is set to stuck");
				return;
			}

		
			if (request.PresentationValues != null) {
				if (request.PresentationValues.ContainsKey ("CLEAR_STACK")) {
					var nextViewController = (UIViewController)ViewCreator.CreateView (request);

					if (MasterNavigationController.TopViewController.GetType () != nextViewController.GetType ()) {
						MasterNavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
						MasterNavigationController.NavigationBar.TintColor = UIColor.White;

//						MasterNavigationController.PopToRootViewController (false);
						MasterNavigationController.SetViewControllers (new UIViewController[]{ nextViewController }, true);
						//MasterNavigationController.PushViewController (nextViewController, true);
					}

					return;
				}
			}

			base.Show (request);
		}

		protected override UINavigationController CreateNavigationController (UIViewController viewController)
		{
			var navBar = base.CreateNavigationController (viewController);
			navBar.NavigationBarHidden = true;		
			navBar.NavigationBar.BarStyle = UIBarStyle.Black;
			navBar.NavigationBar.TintColor = UIColor.White;
			return navBar;
		}

		public override void Show (IMvxTouchView view)
		{
			if (IsStuck) {
				MvxTrace.Trace (MvxTraceLevel.Warning, "Can not show - View is set to stuck");
				return;
			}

			var viewController = view as UIViewController;
			if (null == viewController) {
				throw new MvxException ("Passed in IMvxTouchView is not a UIViewController");
			}

			if (null == MasterNavigationController) {
				ShowFirstView (viewController);
			} else {

				bool animated = true;
//				var presentableView = view as IPresentableView;
//				if (null != presentableView) {
//					animated = presentableView.AnimatedOnShow;
//				}

				MasterNavigationController.PushViewController (viewController, animated);
			}
		}

		public override void Close (Cirrious.MvvmCross.ViewModels.IMvxViewModel toClose)
		{
			if (IsStuck) {
				MvxTrace.Trace (MvxTraceLevel.Warning, "Can not close - View is set to stuck");
				return;
			}

			var topViewController = MasterNavigationController.TopViewController;

			if (topViewController == null) {
				MvxTrace.Trace (MvxTraceLevel.Warning, "Don't know how to close this viewmodel - no topmost");
				return;
			}

			var topView = topViewController as IMvxTouchView;
			if (topView == null) {
				MvxTrace.Trace (MvxTraceLevel.Warning, "Don't know how to close this viewmodel - topmost is not a touchview");
				return;
			}

			bool animated = true;
//			var presentableView = topView as IPresentableView;
//			if (null != presentableView) {
//				animated = presentableView.AnimatedOnClose;
//			}

			var viewModel = topView.ReflectionGetViewModel ();
			if (viewModel != toClose) {
				MvxTrace.Trace (MvxTraceLevel.Warning, "Don't know how to close this viewmodel - topmost view does not present this viewmodel");
				return;
			}

			MasterNavigationController.PopViewController (animated);
		}
	}
}

