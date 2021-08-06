using UIKit;
using SidebarNavigation;

namespace iCunOS.BillBoards
{
	public class RootViewController : UIViewController
	{
		public RootViewController ()
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBar.Translucent = false;
			NavigationController.NavigationBarHidden = true;
			//NavigationController.NavigationBar.AccessibilityNavigationStyle = UIAccessibilityNavigationStyle.Automatic;
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			// Perform any additional setup after loading the view, typically from a nib.
					
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;

			// create a slideout navigation controller with the top navigation controller and the menu view controller
			app.SidebarController = new SidebarController (this, CreateViewFor (new MainViewController (), false), 
				CreateViewFor (new MenuViewController (), true));

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
		private UIViewController CreateViewFor (UIViewController c, bool navBarHidden)
		{
			var controller = new UINavigationController ();
			controller.NavigationBar.BarStyle = UIBarStyle.Black;
			controller.NavigationBar.TintColor = UIColor.White;

			controller.PushViewController (c, false);
			controller.NavigationBarHidden = navBarHidden;
			return controller;
		}
	}
}

