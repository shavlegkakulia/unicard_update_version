using System;
using Kunicardus.Core;
using System.Globalization;
using UIKit;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Accelerate;
using System.Runtime.InteropServices;
using Foundation;
using Kunicardus.Core.ViewModels.iOSSpecific;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;

namespace Kunicardus.Touch
{
	public class OldPinViewController : BaseMvxViewController
	{
		#region Private Variables

		private SettingsPinView _currentLayout;
		private UITextField _all;

		#endregion

		#region Constructor Implementation

		public OldPinViewController ()
		{
			HideMenuIcon = true;
		}

		#endregion

		#region Properties

		public new iOldPinViewModel ViewModel {
			get { return (iOldPinViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		private bool _pinRemoved;

		public bool PinRemoved {
			get{ return _pinRemoved; }
			set {
				if (value) {
					NSUserDefaults.StandardUserDefaults.SetInt (2, ViewModel.UserId);
					NSUserDefaults.StandardUserDefaults.Synchronize ();
					AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
					ShowMenu<SettingsViewModel> (app, typeof(SettingsViewModel));
					NavigationController.PopToRootViewController (false);
				} else
					_currentLayout.ClearDigits ();
				_pinRemoved = value;
			}
		}

		private UIViewController CreateViewFor (IMvxViewModel viewModel, bool navBarHidden)
		{
			var controller = new UINavigationController ();
			var screen = this.CreateViewControllerFor (viewModel) as UIViewController;
			controller.PushViewController (screen, false);
			controller.NavigationBar.BarStyle = UIBarStyle.Black;
			controller.NavigationBarHidden = navBarHidden;
			return controller;
		}

		private void ShowMenu<T> (AppDelegate app, Type viewModelType) where T: IMvxViewModel
		{							
			var controller = ((UINavigationController)app.SidebarController.ContentAreaController).TopViewController;
			MvxViewController topMvxViewController = null;
			topMvxViewController = (MvxViewController)((UINavigationController)app.SidebarController.ContentAreaController).TopViewController;
			if (topMvxViewController != null) {
				app.SidebarController.ChangeContentView (CreateViewFor (Mvx.IocConstruct<T> (), false));
			} 
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff");
			InitUI ();
			Title = this.ViewModel.HeaderTitle;
			var set = this.CreateBindingSet<OldPinViewController,iOldPinViewModel> ();
			set.Bind (this).For (v => v.PinRemoved).To (vm => vm.PinRemoved).Apply ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
			NavigationController.NavigationBar.Translucent = false;
			_all.BecomeFirstResponder ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationController.NavigationBarHidden = true;
			base.ViewWillDisappear (animated);
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			_all = new UITextField (new CGRect (0, 0, 1, 1));
			this.CreateBinding (_all).To ((iOldPinViewModel vm) => vm.OldPin).Apply ();
			_all.BecomeFirstResponder ();
			_currentLayout = new SettingsPinView (this.ViewModel.PageTitle, View);
			_currentLayout.PinProgressFinished += (sender, e) => {
				if (Title == ApplicationStrings.RemovePin) {
					ViewModel.RemovePin ();
				} else if (Title == ApplicationStrings.ChangePin) {
					ViewModel.PinInputFinished = true;
					if (!ViewModel.PinIsCorrect ())
						_currentLayout.ClearDigits ();
				}
			}; 
			_currentLayout.InitUI (_all, GetStatusBarHeight ());
			View.AddSubview (_all);
		}

		#endregion
	}
}

