using System;
using Kunicardus.Core;
using Kunicardus.Core.ViewModels.iOSSpecific;
using UIKit;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Foundation;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.CrossCore;

namespace Kunicardus.Touch
{
	public class ConfirmNewPinViewController : BaseMvxViewController
	{
		#region Private Variables

		private SettingsPinView _currentLayout;
		private UITextField _all;

		#endregion

		#region Properties

		public new iConfirmNewPinViewModel ViewModel {
			get { return (iConfirmNewPinViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		private bool _pinChaged = false;

		public bool PinChanged {
			get{ return _pinChaged; }
			set { 
				if (value) {
					NavigationController.PopToRootViewController (false);
					PinHelper.CorrectPin = ViewModel.NewPin;
					PinHelper.UserId = ViewModel.UserId;
					if (Title == ApplicationStrings.set_pin) {
						NSUserDefaults.StandardUserDefaults.SetInt (1, ViewModel.UserId);
						NSUserDefaults.StandardUserDefaults.Synchronize ();
						AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
						ShowMenu<SettingsViewModel> (app, typeof(SettingsViewModel));
						NavigationController.PopToRootViewController (false);
					}
				}
				_pinChaged = value;
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

		public ConfirmNewPinViewController ()
		{
			HideMenuIcon = true;
		}

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff");
			InitUI ();
			Title = this.ViewModel.HeaderTitle;
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
			this.CreateBinding (_all).To ((iConfirmNewPinViewModel vm) => vm.ConfirmNewPin).Apply ();
			this.CreateBinding (this).For (v => v.PinChanged).To ((iConfirmNewPinViewModel vm) => vm.PinChanged).Apply ();
			_all.BecomeFirstResponder ();
			_currentLayout = new SettingsPinView (this.ViewModel.PageTitle, View);
			_currentLayout.PinProgressFinished += (sender, e) => {
				ViewModel.PinInputFinished = true;
				if (!ViewModel.PinsAreCorrect ())
					_currentLayout.ClearDigits ();
			}; 
			_currentLayout.InitUI (_all, GetStatusBarHeight ());
			View.AddSubview (_all);
		}

		#endregion
	}
}

