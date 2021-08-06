using System;
using UIKit;
using Kunicardus.Core;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.ViewModels;
using Kunicardus.Core.ViewModels;
using Cirrious.CrossCore;
using CoreAnimation;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core.ViewModels.iOSSpecific;
using GameController;
using System.Threading.Tasks;
using GoogleAnalytics.iOS;

namespace Kunicardus.Touch
{
	public class MenuController : BaseMvxViewController
	{
		#region Variables

		private CAGradientLayer gradient;
		private GAService _gaService;

		#endregion

		#region Properties

		public new MenuViewModel ViewModel {
			get { return (MenuViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		#endregion

		#region Constructors

		public MenuController ()
		{
			HideMenuIcon = true;
			_gaService = GAService.GetGAServiceInstance ();
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();		
			InitUI ();
			if (APP.LaunchedShortcutItem != null) {
				ShowMenu<MainViewModel> (APP, typeof(MainViewModel));
			}
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();
			gradient.Frame = View.Bounds;
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			// Helper vars
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			nfloat paddingLeft = 10;
			nfloat tmpPaddingTop = 40;
			nfloat buttonfontSize = 16;

			// Create gradient background for view
			gradient = new CAGradientLayer ();
			gradient.Frame = View.Bounds;
			gradient.NeedsDisplayOnBoundsChange = true;
			gradient.MasksToBounds = true;
			gradient.Colors = new CGColor[] {
				UIColor.Clear.FromHexString ("#f3922b").CGColor,
				UIColor.Clear.FromHexString ("#f6ae2b").CGColor,
				UIColor.Clear.FromHexString ("#f5a02b").CGColor
			};
			View.Layer.InsertSublayer (gradient, 0);

			// Adding logo
			UIButton logo = new UIButton (UIButtonType.System);
			logo.SetBackgroundImage (UIImage.FromBundle ("menu_unicard_logo"), UIControlState.Normal);
			logo.SizeToFit ();
			logo.Frame = new CGRect (0, tmpPaddingTop, logo.Frame.Width, logo.Frame.Height);

			logo.TouchUpInside += (o, e) => {
				ShowMenu<MainViewModel> (app, typeof(MainViewModel));
			};

			View.AddSubview (logo);
			tmpPaddingTop = logo.Frame.Bottom + 20;

			// Add Hello Label
			UILabel helloLabel = new UILabel ();
			helloLabel.TextColor = UIColor.White;
			helloLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 18);
			helloLabel.Frame = new CGRect (paddingLeft, tmpPaddingTop, View.Frame.Width, 20);
			View.AddSubview (helloLabel);
			tmpPaddingTop = helloLabel.Frame.Bottom + 13;

			// Add card button
			UIButton cardImageButton = new UIButton (UIButtonType.System);
			cardImageButton.Layer.CornerRadius = 5;
			cardImageButton.BackgroundColor = UIColor.Clear.FromHexString ("#fcbc19");
			cardImageButton.SetImage (UIImage.FromBundle ("menu_card"), UIControlState.Normal);
			cardImageButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			cardImageButton.SetTitle (ApplicationStrings.Card, UIControlState.Normal);
			cardImageButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			cardImageButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, buttonfontSize);
			cardImageButton.ImageEdgeInsets = new UIEdgeInsets (10.0f, 10.0f, 10.0f, 0.0f);
			cardImageButton.TitleEdgeInsets = new UIEdgeInsets (0.0f, 20.0f, 0f, 0.0f);
			cardImageButton.Frame = new CGRect (paddingLeft, tmpPaddingTop, 250, 43);
			cardImageButton.TintColor = UIColor.White;
			cardImageButton.TouchUpInside += OpenCard;
			View.AddSubview (cardImageButton);

			// Changing padding sizes
			tmpPaddingTop = cardImageButton.Frame.Bottom + 10;
			paddingLeft += 6;
			nfloat paddingTopRisesDivider = 1;
			nfloat paddingTopRisesMenu = 34;

			CustomScrollView scroll = new CustomScrollView (new CGRect (0, tmpPaddingTop, View.Frame.Width, View.Frame.Height - tmpPaddingTop));
			scroll.DelaysContentTouches = false;
			scroll.ScrollEnabled = true;
			scroll.AlwaysBounceVertical = true;
			scroll.ShowsVerticalScrollIndicator = false;
			scroll.ShowsHorizontalScrollIndicator = false;


			tmpPaddingTop = 5;
			// Adding menu items


			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_home", ApplicationStrings.Main, () => {
				ShowMenu<MainViewModel> (app, typeof(MainViewModel));
				_gaService.TrackScreen (GAServiceHelper.Pagenames.HomePage);
			}));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_mypage", ApplicationStrings.MyPage, () => {
				ShowMenu<iMyPageViewModel> (app, typeof(iMyPageViewModel));
				_gaService.TrackScreen (GAServiceHelper.Pagenames.MyPage);
			}));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_catalog", ApplicationStrings.WhereToSpend, () => {
				ShowMenu<iCatalogListViewModel> (app, typeof(iCatalogListViewModel));
				_gaService.TrackScreen (GAServiceHelper.Pagenames.Catalog);
			}));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_merchants", ApplicationStrings.AroundMe, () => {
				ShowMenu<iMerchantsAroundMeViewModel> (app, typeof(iMerchantsAroundMeViewModel));
				_gaService.TrackScreen (GAServiceHelper.Pagenames.AroundMe);
			}));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_partners", ApplicationStrings.Partners, 
				() => {
					ShowMenu<OrganisationListViewModel> (app, typeof(OrganisationListViewModel));
					_gaService.TrackScreen (GAServiceHelper.Pagenames.Partners);
				}
			));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_news", ApplicationStrings.News, () => {
				ShowMenu<NewsListViewModel> (app, typeof(NewsListViewModel));
				_gaService.TrackScreen (GAServiceHelper.Pagenames.News);
			}));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_aboutus", ApplicationStrings.AboutUs, () => {
				ShowMenu<AboutViewModel> (app, typeof(AboutViewModel));
				_gaService.TrackScreen (GAServiceHelper.Pagenames.AboutUs);
			}));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_settings", ApplicationStrings.Settings, () => {
				ShowMenu<SettingsViewModel> (app, typeof(SettingsViewModel));
				_gaService.TrackScreen (GAServiceHelper.Pagenames.Settings);
			}));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.AddSubview (MenuDeviderLine (tmpPaddingTop));
			tmpPaddingTop += paddingTopRisesDivider;

			scroll.AddSubview (MenuView (paddingLeft, tmpPaddingTop, "menu_logout", ApplicationStrings.Logout, () => {
				((MenuViewModel)ViewModel).Logout ();
				_gaService.TrackScreen (GAServiceHelper.Pagenames.LogInPage);
				MainViewController.DialogShowed = false;
			}));
			tmpPaddingTop += paddingTopRisesMenu;

			scroll.ContentSize = new CGSize (View.Frame.Width, tmpPaddingTop);
			View.AddSubview (scroll);

			// Set Bingings
			this.CreateBinding (helloLabel).To ((MenuViewModel vm) => vm.WelcomeMessage).Apply ();
		}


		private UIButton MenuView (nfloat left, nfloat top, string icon, string name, Action onClick, nfloat? titleLeft = null)
		{
			UIButton imageAndTextButton = new UIButton (UIButtonType.System);
			imageAndTextButton.BackgroundColor = UIColor.Clear;
			imageAndTextButton.SetImage (UIImage.FromBundle (icon), UIControlState.Normal);
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
			UIView view = new UIView (new CGRect (0, top, View.Frame.Width, 1.5f));
			view.BackgroundColor = UIColor.Clear.FromHexString ("#fab33d");
			return view;
		}

		private UIViewController CreateViewForCollection (IMvxViewModel viewModel, bool navBarHidden)
		{
			var controller = new UINavigationController ();
			var screen = this.CreateViewControllerFor (viewModel) as MvxCollectionViewController;
			controller.PushViewController (screen, false);
			controller.NavigationBar.BarStyle = UIBarStyle.Black;
			controller.NavigationBarHidden = navBarHidden;
			return controller;
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
			MvxTableViewController topMvxTableViewController = null;
			MvxCollectionViewController tppMvxCollectionViewController = null;
			if (controller.GetType () == typeof(ProductsListViewController)) {
				tppMvxCollectionViewController = (MvxCollectionViewController)((UINavigationController)app.SidebarController.ContentAreaController).TopViewController;
				
			} else if (controller.GetType () == typeof(NewsListViewController) || controller.GetType () == typeof(OrganisationListViewController)) {
				topMvxTableViewController = (MvxTableViewController)((UINavigationController)app.SidebarController.ContentAreaController).TopViewController;
			} else {
				topMvxViewController = (MvxViewController)((UINavigationController)app.SidebarController.ContentAreaController).TopViewController;
			}
			if (tppMvxCollectionViewController != null) {
				if (tppMvxCollectionViewController.ViewModel.GetType () == viewModelType) {
					//UIApplication.SharedApplication.InvokeOnMainThread (() => {
					app.SidebarController.ToggleMenu ();	
					//});
				} else {
					//UIApplication.SharedApplication.InvokeOnMainThread (() => {
					app.SidebarController.ChangeContentView (CreateViewFor (Mvx.IocConstruct<T> (), false));
					//});
				}
				
			} else if (topMvxViewController != null) {
				if (topMvxViewController.ViewModel.GetType () == viewModelType) {
					//UIApplication.SharedApplication.InvokeOnMainThread (() => {

					app.SidebarController.CloseMenu ();	
					//});
				} else {
					//UIApplication.SharedApplication.InvokeOnMainThread (() => {
					app.SidebarController.ChangeContentView (CreateViewFor (Mvx.IocConstruct<T> (), false));
					//});
				}
			} else if (topMvxTableViewController != null) {
				if (topMvxTableViewController.ViewModel.GetType () == viewModelType) {
					//UIApplication.SharedApplication.InvokeOnMainThread (() => {
					app.SidebarController.CloseMenu ();	
					//});
				} else {
					//UIApplication.SharedApplication.InvokeOnMainThread (() => {
					app.SidebarController.ChangeContentView (CreateViewFor (Mvx.IocConstruct<T> (), false));
					//});
				}
			} 				
		}

		#endregion

		#region Events

		void OpenCard (object sender, EventArgs e)
		{			
			var vc = new CardViewController (((MenuViewModel)ViewModel).CardNumber);
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			app.Window.RootViewController.PresentModalViewController (vc, true);
			_gaService.TrackEvent (GAServiceHelper.From.FromMenu, GAServiceHelper.Events.CardClicked);
		}

		#endregion
	}
}

