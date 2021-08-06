using Foundation;
using UIKit;
using SidebarNavigation;
using iCunOS.BillBoards;
using Facebook.CoreKit;
using Kunicardus.Billboards.Core;
using Kunicardus.Billboards.Core.DbModels;
using System.Linq;
using System;
using System.IO;

namespace iCunOS.BillBoards
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{

		public static AppDelegate Instance {
			get {
				return UIApplication.SharedApplication.Delegate as AppDelegate;
			}
		}

		public static readonly string GoogleMapsAPIKey = NSBundle.MainBundle.ObjectForInfoDictionary ("GoogleMapsAPIKey").ToString ();

		public Toast Toast {
			get;
			set;
		}

		public override UIWindow Window {
			get;
			set;
		}

		public SidebarController SidebarController;

		public UIActivityIndicatorView ActivityIndicator;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{			
			Google.Maps.MapServices.ProvideAPIKey (GoogleMapsAPIKey);

			Window = new UIWindow (UIScreen.MainScreen.Bounds);
			AdView.DefaultSize = new CoreGraphics.CGSize (Window.Frame.Width - 30, Window.Frame.Height - 100);

			var controller = new UINavigationController ();
			controller.NavigationBar.BarStyle = UIBarStyle.Black;
			controller.NavigationBar.TintColor = UIColor.White;

			App.Initialize ();

			using (var db = new BillboardsDb (BillboardsDb.path)) {				
				if (db.Table<UserInfo> ().FirstOrDefault () != null) {
					controller.PushViewController (new RootViewController (), false);
				} else {
					controller.PushViewController (new LandingViewController (), false);  
				}
			}	

			Window.RootViewController = controller;

			Window.MakeKeyAndVisible ();




			// Styling navigation
			UINavigationBar.Appearance.BarTintColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
			UINavigationBar.Appearance.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
			UINavigationBar.Appearance.SetTitleTextAttributes (new UITextAttributes {
				TextColor = UIColor.White,
				Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 1)
			});
			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.LightContent, false);
			UIApplication.SharedApplication.SetStatusBarHidden (false, false);

			UIBarButtonItem.AppearanceWhenContainedIn (typeof(UISearchBar)).SetTitleTextAttributes (
				new UITextAttributes () {
					TextColor = UIColor.White
				}, UIControlState.Normal);

			UIApplication.Notifications.ObserveDidBecomeActive ((sender, args) => {
				Console.WriteLine ("ObserveDid BecomeActive");
			});
			UIApplication.Notifications.ObserveDidEnterBackground ((sender, args) => {
				Console.WriteLine ("ObserveDid EnterBackground");
			});


			// Register for notifications
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes (
					                           UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
				                           );

				application.RegisterUserNotificationSettings (notificationSettings);
			} 


			if (launchOptions != null) {
				// check for a local notification
				if (launchOptions.ContainsKey (UIApplication.LaunchOptionsLocalNotificationKey)) {
					var localNotification = launchOptions [UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
					if (localNotification != null) {
						
						GoToMap ();

						// reset our badge
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					}
				}
			}


			return true;
		}

		private void GoToMap ()
		{
			if (((UINavigationController)this.SidebarController.ContentAreaController).TopViewController.GetType ()
			    == typeof(MapViewController)) {
				if (this.SidebarController.IsOpen) {
					this.SidebarController.ToggleMenu ();
				}
			} else {
				var controller = new UINavigationController ();
				controller.PushViewController (new MapViewController (), false);
				controller.NavigationBar.BarStyle = UIBarStyle.Black;
				controller.NavigationBarHidden = false;
				this.SidebarController.ChangeContentView (controller);
			}
		}

		public override void ReceivedLocalNotification (UIApplication app, UILocalNotification notification)
		{
			GoToMap ();
			// reset our badge
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			Console.WriteLine ("App entering background state.");
		}

		public override void WillEnterForeground (UIApplication application)
		{
			Console.WriteLine ("App will enter foreground");
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate (UIApplication application)
		{			
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
			Navigation.Active = false;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// We need to handle URLs by passing them to FBSession in order for SSO authentication
			// to work.
			return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
		}
	}
}


