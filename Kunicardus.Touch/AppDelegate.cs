using Foundation;
using UIKit;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.ViewModels;
using SidebarNavigation;
using Kunicardus.Touch.Helpers.UI;
using Facebook.CoreKit;
using Google.Maps;
using GoogleAnalytics.iOS;
using Kunicardus.Core;
using AppSearch;
using System;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using System.Linq;
using Kunicardus.Touch.Plugins.UIDialogPlugin;
using System.Security.Principal;

namespace Kunicardus.Touch
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : MvxApplicationDelegate
	{

		public static readonly string GoogleMapsAPIKey = NSBundle.MainBundle.ObjectForInfoDictionary ("GoogleMapsAPIKey").ToString ();

		public Toast Toast {
			get;
			set;
		}

		public IGAITracker Tracker;
		//test Tracking Id
		//	public static readonly string TrackingId = "UA-67609429-1";
		//live Tracking Id
		public static readonly string TrackingId = "UA-41502131-4";

		public override UIWindow Window {
			get;
			set;
		}

		public SidebarController SidebarController;

		public UIActivityIndicatorView ActivityIndicator;

		public event EventHandler OpenCard;
		// Quick actions
		public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }

		public bool HandleShortcutItem (UIApplicationShortcutItem shortcutItem)
		{
			var handled = false;

			// Anything to process?
			if (shortcutItem == null)
				return false;

			// Take action based on the shortcut type
			switch (shortcutItem.Type) {
			case ShortcutIdentifier.First:
				
				Console.WriteLine ("First shortcut selected");
				using (var db = Mvx.Resolve<ILocalDbProvider> ()) {
					var user = db.Get<UserInfo> ().FirstOrDefault ();
					if (user == null) {
						LaunchedShortcutItem = null;
					}
				}
				handled = true;
				break;			
			}

			// Return results
			return handled;
		}

		public override void OnActivated (UIApplication application)
		{
			// Handle any shortcut item being selected
			HandleShortcutItem (LaunchedShortcutItem);

			// Clear shortcut after it's been handled
			//LaunchedShortcutItem = null;
		}

		public override void PerformActionForShortcutItem (UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
		{
			// Perform action
			completionHandler (HandleShortcutItem (shortcutItem));
		}
		// --------------



		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			MapServices.ProvideAPIKey (GoogleMapsAPIKey);
			// ----

			// Get possible shortcut item
			if (options != null) {
				LaunchedShortcutItem = options [UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;
			}

			Window = new UIWindow (UIScreen.MainScreen.Bounds);
			Window.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			// Init activity Indicator --------------------------------
		
			GAI.SharedInstance.DispatchInterval = 20;
			GAI.SharedInstance.TrackUncaughtExceptions = true;
			Tracker = GAI.SharedInstance.GetTracker (TrackingId);
			GAService gaService = GAService.GetGAServiceInstance ();
			if (gaService != null)
				gaService.TrackEvent (GAServiceHelper.From.Application, GAServiceHelper.Events.ApplicationStart);

			var presenter = new TouchCustomPresenter (this, Window);
			var setup = new Setup (this, Window, presenter);
			setup.Initialize ();

			var startup = Mvx.Resolve<IMvxAppStart> ();
			startup.Start ();

			Window.MakeKeyAndVisible ();



			// Styling navigation
			UINavigationBar.Appearance.BarTintColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			UINavigationBar.Appearance.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
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
			
			//mySliders.TintColor = UIColor.Red;

			//UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment (new UIOffset (0, -60), UIBarMetrics.Default);

			return true;
		}


		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// We need to handle URLs by passing them to FBSession in order for SSO authentication
			// to work.
			return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
		}

		public override void DidEnterBackground (UIApplication application)
		{
			base.DidEnterBackground (application);
			_currentTime = DateTime.Now;
			_fromBackground = true;
		}

		public override void WillEnterForeground (UIApplication application)
		{
			base.WillEnterForeground (application);
			if (PinHelper.IsUserLoggedIn) {
				var secondsDifference = DateTime.Now.Ticks / TimeSpan.TicksPerSecond - _currentTime.Ticks / TimeSpan.TicksPerSecond;
				if (_fromBackground && secondsDifference > 60 && !_pinIsOpened) {
					if (!string.IsNullOrWhiteSpace (PinHelper.UserId)) {
						var status = PinHelper.GetPinStatus (PinHelper.UserId);
						if (status == PinStatus.ShouldEnterPin) {
							InitPinUI ();
							_pinIsOpened = true;
						}
					}
				}
			}
		}

		#region Pin Logic

		private bool _pinIsOpened;
		private bool _fromBackground;
		private DateTime _currentTime;
		private float _pinPosition = 80f;
		private PinWrapper _pinWrapper;
		private EnterPin _enterPin;

		private void InitPinUI ()
		{
			nfloat width = Window.Frame.Width - 60f;
			nfloat height = 160f;

			if (!Screen.IsTall) {
				height = 150;
				width = Window.Frame.Width - 60f;
			}

			_pinWrapper = new PinWrapper (Window.Frame);

			_enterPin = new EnterPin (
				new CoreGraphics.CGRect (
					(Window.Frame.Width - width) / 2.0f,
					(Window.Frame.Height - height) / 2.0f - _pinPosition,
					width,
					height), PinStatus.ShouldEnterPin);

			Window.AddSubview (_pinWrapper);
			Window.AddSubview (_enterPin);

			_enterPin.SetPinFinished += (pinSender, pinE) => {
				var pinIsCorrect = PinHelper.CorrectPin == pinE;
				if (pinIsCorrect) {
					_pinWrapper.RemoveFromSuperview ();
					_enterPin.RemoveFromSuperview ();
					_pinIsOpened = false;
					_enterPin.ClearDigits ();
				} else {
					_enterPin.ClearDigits ();
					TouchUIDialogPlugin dialog = new TouchUIDialogPlugin ();
					dialog.ShowToast (ApplicationStrings.IncorrectPin);
				}
			};
		}

		#endregion

	}
}
