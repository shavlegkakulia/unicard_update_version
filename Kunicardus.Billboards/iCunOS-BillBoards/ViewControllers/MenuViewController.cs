using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Billboards.Core;
using Autofac;
using System.Threading.Tasks;
using Facebook.LoginKit;

namespace iCunOS.BillBoards
{
	public class MenuViewController : UIViewController
	{
		#region Variables

		private static MenuViewModel _viewModel;

		#endregion

		#region Ctors

		public MenuViewController ()
		{
			using (var scope = App.Container.BeginLifetimeScope ()) {
				_viewModel = scope.Resolve<MenuViewModel> ();
			}
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.DarkGray);
			InitUI ();
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			// Helper vars
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			nfloat paddingLeft = 10;
			nfloat tmpPaddingTop = 50;

			// Adding logo
			UIImageView logo = new UIImageView (ImageHelper.MaxResizeImage (UIImage.FromBundle ("menu_logo"), 0, 75));
			logo.SizeToFit ();
			logo.Frame = new CGRect (0, tmpPaddingTop, logo.Frame.Width, logo.Frame.Height);
			View.AddSubview (logo);
			tmpPaddingTop = logo.Frame.Bottom + 20;

			// Add Hello Label
			UILabel helloLabel = new UILabel ();
			helloLabel.Text = _viewModel.GetUserName ();
			helloLabel.TextColor = UIColor.White;
			helloLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 17);
			helloLabel.Frame = new CGRect (paddingLeft, tmpPaddingTop, View.Frame.Width, 20);
			View.AddSubview (helloLabel);
			tmpPaddingTop = helloLabel.Frame.Bottom + 20;

			nfloat paddingTopRisesMenu = 38;
			nfloat paddingTopRisesDivider = 5;

			#region Adding menu items

			View.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_home", ApplicationStrings.MainPage, () => {
				if (((UINavigationController)app.SidebarController.ContentAreaController).TopViewController.GetType ()
				    == typeof(MainViewController)) {
					app.SidebarController.ToggleMenu ();
				} else {
					var controller = new UINavigationController ();
					controller.PushViewController (new MainViewController (), false);
					controller.NavigationBar.BarStyle = UIBarStyle.Black;
					controller.NavigationBarHidden = false;
					app.SidebarController.ChangeContentView (controller);
				}
			}));
			tmpPaddingTop += paddingTopRisesMenu;
			View.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			View.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_pin", ApplicationStrings.Map, () => {
				if (((UINavigationController)app.SidebarController.ContentAreaController).TopViewController.GetType ()
				    == typeof(MapViewController)) {
					app.SidebarController.ToggleMenu ();
				} else {
					var controller = new UINavigationController ();
					controller.PushViewController (new MapViewController (), false);
					controller.NavigationBar.BarStyle = UIBarStyle.Black;
					controller.NavigationBarHidden = false;
					app.SidebarController.ChangeContentView (controller);
				}
			}));
			tmpPaddingTop += paddingTopRisesMenu;
			View.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			View.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_billboard", ApplicationStrings.Ads, () => {
				if (((UINavigationController)app.SidebarController.ContentAreaController).TopViewController.GetType ()
				    == typeof(AdsViewController)) {
					app.SidebarController.ToggleMenu ();
				} else {
					var controller = new UINavigationController ();
					controller.PushViewController (new AdsViewController (
						UIPageViewControllerTransitionStyle.Scroll,
						UIPageViewControllerNavigationOrientation.Horizontal,
						UIPageViewControllerSpineLocation.Min), false);
					controller.NavigationBar.BarStyle = UIBarStyle.Black;
					controller.NavigationBarHidden = false;
					app.SidebarController.ChangeContentView (controller);
				}
			}, 23));		
			tmpPaddingTop += paddingTopRisesMenu;
			View.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			View.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_history", ApplicationStrings.History, () => {
				if (((UINavigationController)app.SidebarController.ContentAreaController).TopViewController.GetType ()
				    == typeof(HistoryViewController)) {
					app.SidebarController.ToggleMenu ();
				} else {
					var controller = new UINavigationController ();
					controller.PushViewController (new HistoryViewController (), false);
					controller.NavigationBar.BarStyle = UIBarStyle.Black;
					controller.NavigationBarHidden = false;
					app.SidebarController.ChangeContentView (controller);
				}
			}, 20));
			tmpPaddingTop += paddingTopRisesMenu;
			View.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			View.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_settings", ApplicationStrings.Settings, () => {
				if (((UINavigationController)app.SidebarController.ContentAreaController).TopViewController.GetType ()
				    == typeof(SettingsViewController)) {
					app.SidebarController.ToggleMenu ();
				} else {
					var controller = new UINavigationController ();
					controller.PushViewController (new SettingsViewController (), false);
					controller.NavigationBar.BarStyle = UIBarStyle.Black;
					controller.NavigationBarHidden = false;
					app.SidebarController.ChangeContentView (controller);
				}
			}));
			tmpPaddingTop += paddingTopRisesMenu;
			View.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			View.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_logout", ApplicationStrings.Logout, () => {				
				
				LogOut ();
			}));
			#endregion
		}

		private UIButton MenuView (nfloat left, nfloat top, string icon, string name, Action onClick, nfloat? titleLeft = null)
		{
			UIButton imageAndTextButton = new UIButton (UIButtonType.System);
			imageAndTextButton.BackgroundColor = UIColor.Clear;
			if (icon != "menu_billboard" && icon != "menu_history") {
				imageAndTextButton.SetImage (UIImage.FromBundle (icon), UIControlState.Normal);
			} else {
				imageAndTextButton.SetImage (ImageHelper.MaxResizeImage (UIImage.FromBundle (icon), 0, 23), UIControlState.Normal);
			}
			imageAndTextButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			imageAndTextButton.SetTitle (name, UIControlState.Normal);
			imageAndTextButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			imageAndTextButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			imageAndTextButton.ImageEdgeInsets = new UIEdgeInsets (4.5f, 5f, 4.5f, 0.0f);
			imageAndTextButton.TitleEdgeInsets = new UIEdgeInsets (0.0f, (titleLeft.HasValue ? titleLeft.Value : 16.5f), 0f, 0.0f);

			imageAndTextButton.Frame = new CGRect (left, top, View.Frame.Width - left - 100, 34);
			imageAndTextButton.TintColor = UIColor.White;
			imageAndTextButton.TouchUpInside += delegate {
				if (onClick != null) {
					onClick.Invoke ();
				}
			};
			return imageAndTextButton;

		}

		private UIView MenuDeviderLine (nfloat top)
		{
			UIView view = new UIView (new CGRect (0, top, View.Frame.Width, 1.2f));
			view.BackgroundColor = UIColor.Clear.FromHexString ("#57504a");
			return view;
		}

		public static void LogOut ()
		{
			if (MapViewController._locationManager != null) {
				MapViewController._locationManager.StopMonitoringRegion ();
				MapViewController._locationManager.StopLocationUpdates ();
			}
			Navigation.Active = false;

			DialogPlugin.ShowProgressDialog (ApplicationStrings.Loading);
			Task.Run (() => {
				var success = _viewModel.Logout ();
				UIApplication.SharedApplication.InvokeOnMainThread (() => {
					DialogPlugin.DismissProgressDialog ();
					if (success) {
						AppDelegate.Instance.SidebarController.NavigationController.SetViewControllers (new UIViewController[] { new LandingViewController () }, true);
					}
				});
			});
		}

		#endregion
	}
}

