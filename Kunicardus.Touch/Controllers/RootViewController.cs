using System;
using UIKit;
using SidebarNavigation;
using Cirrious.MvvmCross.Touch.Views;
using Kunicardus.Core.ViewModels.iOSSpecific;
using Cirrious.MvvmCross.ViewModels;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core;
using Cirrious.CrossCore;

namespace Kunicardus.Touch
{
	public class RootViewController : MvxViewController
	{
		public SidebarController SidebarController { get; private set; }

		public RootViewController ()
		{
			
		}

		public RootViewModel RootViewModel
		{ get { return base.ViewModel as RootViewModel; } }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//NavigationController.NavigationBar.Translucent = false;
			NavigationController.NavigationBarHidden = true;
			//NavigationController.NavigationBar.AccessibilityNavigationStyle = UIAccessibilityNavigationStyle.Automatic;
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			// Perform any additional setup after loading the view, typically from a nib.

			if (ViewModel == null)
				return;

			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;

			// create a slideout navigation controller with the top navigation controller and the menu view controller
			MainViewModel main = Mvx.IocConstruct<MainViewModel> ();
			main.UserAuthed = (this.ViewModel as RootViewModel).UserAuthed;
			MenuViewModel menu = Mvx.IocConstruct<MenuViewModel> ();
			app.SidebarController = new SidebarController (this, CreateViewFor (main, false), CreateViewFor (menu, true));

			app.SidebarController.MenuWidth = 270;
			app.SidebarController.ReopenOnRotate = false;
			app.SidebarController.MenuLocation = SidebarController.MenuLocations.Left;

		}

		public override void ViewDidAppear (bool animated)
		{
			NavigationController.NavigationBarHidden = true;
			base.ViewDidAppear (animated);
		}

		public override void ViewWillAppear (bool animated)
		{						
			base.ViewWillAppear (animated);
		}

		// from Stuart Lodge N+1-25
		private UIViewController CreateViewFor (IMvxViewModel viewModel, bool navBarHidden)
		{
			var controller = new UINavigationController ();
			controller.NavigationBar.BarStyle = UIBarStyle.Black;
			controller.NavigationBar.TintColor = UIColor.White;
			var screen = this.CreateViewControllerFor (viewModel) as UIViewController;
			controller.PushViewController (screen, false);
			controller.NavigationBarHidden = navBarHidden;
			return controller;
		}
	}
}

