using System;
using UIKit;
using CoreGraphics;
using Facebook.CoreKit;
using System.Runtime.InteropServices;
using Kunicardus.Billboards.Core.ViewModels;
using Autofac;
using System.ComponentModel.Design;
using CoreAnimation;
using Foundation;
using System.IO;
using AVFoundation;

namespace iCunOS.BillBoards
{
	public class MainViewController : BaseViewController
	{
		#region Constructor

		public MainViewController ()
		{
			using (var scope = App.Container.BeginLifetimeScope ()) {
				_viewModel = scope.Resolve<HomePageViewModel> ();
				_viewModel.GetLocalUserInfo ();
			}
		}

		#endregion

		#region Variables

		private HomePageViewModel _viewModel;
		private nfloat _topViewHeight = 100f, _middleViewHeight = 100f;
		private UITableView _adsTableView;
		private UIRefreshControl _refreshController;
		private UILongPressGestureRecognizer _longPressGesture;

		#endregion

		#region UI

		private UIImageView _userImageView;
		private UIBarButtonItem _newsBarButton;
		private KuniBadgeBarButtonItem _newsCounter;
		private UIButton _startNavigation, _stopNavigation;
		private UIView _topView, _middleView, _bottomView;
		private UILabel _points, _pointsIndicator, _userName;

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			Title = ApplicationStrings.MainPage;
			InitUI ();
			if (MapViewController._locationManager != null)
				MapViewController._locationManager.BillboardPassed += BillboardPassed;
			
			// Screen subscribes to the location changed event
			UIApplication.Notifications.ObserveDidBecomeActive ((sender, args) => {	
				if (MapViewController._locationManager != null) {
					MapViewController._locationManager.BillboardPassed -= BillboardPassed;
					MapViewController._locationManager.BillboardPassed += BillboardPassed;
					RefreshView ();
				}
			});

			// Whenever the app enters the background state, we unsubscribe from the event 
			// so we no longer perform foreground updates
			UIApplication.Notifications.ObserveDidEnterBackground ((sender, args) => {
				if (MapViewController._locationManager != null) {
					MapViewController._locationManager.BillboardPassed -= BillboardPassed;
				}
			});

			System.Threading.Tasks.Task.Run (() => {
				_viewModel.GetUserInfo ();
				UIApplication.SharedApplication.InvokeOnMainThread (() => {
					_points.Text = Math.Round (_viewModel.User.Balance_AvailablePoints, 1).ToString ().Replace (",", ".");
					_points.SizeToFit ();

					var pIFrame = _pointsIndicator.Frame;
					pIFrame.X = _points.Frame.Right + 2f;
					pIFrame.Y = _userName.Frame.Bottom + 11f;
					_pointsIndicator.Frame = pIFrame;
				});
			});

			_newsCounter = new KuniBadgeBarButtonItem ("adalert", null);
			_newsBarButton = new UIBarButtonItem (ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIBarButtonItemStyle.Plain, delegate {					
				var controller = new UINavigationController ();
				controller.PushViewController (new AdsViewController (
					UIPageViewControllerTransitionStyle.Scroll,
					UIPageViewControllerNavigationOrientation.Horizontal,
					UIPageViewControllerSpineLocation.Min), false);
				controller.NavigationBar.BarStyle = UIBarStyle.Black;
				controller.NavigationBarHidden = false;
				APP.SidebarController.ChangeContentView (controller);
			});				


			NavigationItem.RightBarButtonItems = new UIBarButtonItem[] {	
				_newsBarButton,
				new UIBarButtonItem (UIImage.FromBundle ("location_pin2"), UIBarButtonItemStyle.Plain, delegate {
				})
			};
		}

		#endregion

		#region Methods

		private void RotateUserImage ()
		{
			var userImageFrame = _userImageView.Frame;
			//wheel sound
			var file = Path.Combine ("Sounds", "wheels_audio.mp3");
			var Soundurl = NSUrl.FromFilename (file);
			var player = AVAudioPlayer.FromUrl (Soundurl);
			player.NumberOfLoops = 5;
			player.Volume = 1.0f;
			player.PrepareToPlay ();

			//rotate animation
			CABasicAnimation rotationAnimation;
			rotationAnimation = CABasicAnimation.FromKeyPath ("transform.rotation.z");
			rotationAnimation.To = new NSNumber (Math.PI * 2);
			rotationAnimation.Duration = 2;
			rotationAnimation.Cumulative = true;
			rotationAnimation.RepeatCount = 3;
			_userImageView.Layer.AddAnimation (rotationAnimation, "rotationAnimation");

			//transform animation
			UIView.Animate (5.5, 0, UIViewAnimationOptions.CurveEaseInOut,
				() => {
					player.Play ();
					_userImageView.Center =
						new CGPoint (UIScreen.MainScreen.Bounds.Right + _userImageView.Frame.Width / 2f, _userImageView.Center.Y);
				},
				() => {
					player.Stop ();
					_userImageView.Layer.RemoveAnimation ("rotationAnimation");

					//bounce animation variables
					_userImageView.Frame = new CGRect (15f, -30f, _userImageView.Frame.Width, _userImageView.Frame.Height);
					float springDampingRatio = 0.25f;
					float initialSpringVelocity = 2.0f;

					//bounce sound
					var bounceFile = Path.Combine ("Sounds", "spring_up.mp3");
					var bounceUrl = NSUrl.FromFilename (bounceFile);
					var bouncePlayer = AVAudioPlayer.FromUrl (bounceUrl);
					bouncePlayer.NumberOfLoops = 0;
					bouncePlayer.Volume = 0.7f;
					bouncePlayer.PrepareToPlay ();

					//bounce animation
					UIView.AnimateNotify (2.0, 0.0, springDampingRatio, initialSpringVelocity, 0, () => {
						bouncePlayer.Play ();
						_userImageView.Frame = new CGRect (15f, 15f, _userImageView.Frame.Width, _userImageView.Frame.Height); 
					}, (r) => {
						bouncePlayer.Stop ();
					});
				}
			);
		}

		private UIImage ImageFromView (UIView view)
		{
			UIGraphics.BeginImageContextWithOptions (view.Frame.Size, view.Opaque, 0.0f);
			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return img;
		}

		private void InitTopView ()
		{
			nfloat buttonWidth = View.Frame.Width - 40f, buttonHeight = 45f;
			if (!Screen.IsTall) {
				_topViewHeight = 80f;
				buttonWidth -= 20f;
				buttonHeight -= 5f;
			}
			_topView = new UIView (new CGRect (0, 0, View.Frame.Width, _topViewHeight));
			var imageView = new UIImageView (_topView.Frame);
			imageView.Image = UIImage.FromBundle ("map_fragment");
			var blackView = new UIView (_topView.Frame);
			blackView.BackgroundColor = UIColor.Clear.FromHexString ("#000000", 0.5f);

			_topView.AddSubviews (
				imageView,
				blackView);

			if (Navigation.Active) {
				
				_stopNavigation = new UIButton (UIButtonType.RoundedRect);
				_stopNavigation.Frame = new CGRect ((View.Frame.Width - buttonWidth) / 2, 
					(_topView.Frame.Height - buttonHeight) / 2,
					buttonWidth, buttonHeight);
				_stopNavigation.BackgroundColor = UIColor.Clear.FromHexString ("#e95936");
				_stopNavigation.TintColor = UIColor.White;
				_stopNavigation.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15f);
				_stopNavigation.SetImage (
					ImageHelper.MaxResizeImage (
						UIImage.FromBundle ("stopnavigation"),
						0,
						30f).
					ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal),
					UIControlState.Normal);
				_stopNavigation.SetTitle (ApplicationStrings.StopJourney, UIControlState.Normal);
				_stopNavigation.ImageEdgeInsets = new UIEdgeInsets (0, -5, 0, 5);
				_stopNavigation.TitleEdgeInsets = new UIEdgeInsets (0, 5, 0, -5);
				_stopNavigation.Layer.CornerRadius = 10f;
				_stopNavigation.Layer.BorderColor = UIColor.Clear.FromHexString ("#9a503e").CGColor;
				_stopNavigation.Layer.BorderWidth = 2.5f;
				_stopNavigation.TouchUpInside += StopNavigation;

				_topView.AddSubview (_stopNavigation);
			} else {
				
				_startNavigation = new UIButton (UIButtonType.System);
				_startNavigation.Frame = new CGRect ((View.Frame.Width - buttonWidth) / 2, 
					(_topView.Frame.Height - buttonHeight) / 2,
					buttonWidth, buttonHeight);
				_startNavigation.BackgroundColor = UIColor.Clear.FromHexString ("#c9c62b");
				_startNavigation.TintColor = UIColor.White;
				_startNavigation.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15f);
				_startNavigation.SetImage (
					ImageHelper.MaxResizeImage (
						UIImage.FromBundle ("startnavigation"),
						0,
						30f).
					ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal),
					UIControlState.Normal);
				_startNavigation.SetTitle (ApplicationStrings.StartJourney, UIControlState.Normal);
				_startNavigation.ImageEdgeInsets = new UIEdgeInsets (0, -5, 0, 5);
				_startNavigation.TitleEdgeInsets = new UIEdgeInsets (0, 5, 0, -5);
				_startNavigation.Layer.CornerRadius = 10f;
				_startNavigation.Layer.BorderColor = UIColor.Clear.FromHexString ("#969544").CGColor;
				_startNavigation.Layer.BorderWidth = 2.5f;
				_startNavigation.TouchUpInside += StartNavigation;


				_topView.AddSubview (_startNavigation);
			}
		}

		private void InitMiddleView ()
		{
			if (!Screen.IsTall)
				_middleViewHeight = 80f;
			_middleView = new UIView (new CGRect (0, _topView.Frame.Bottom, View.Frame.Width, _middleViewHeight));
			_middleView.BackgroundColor = UIColor.Clear.FromHexString ("f6f8f8");

			_userImageView = new UIImageView (new CGRect (15f, 15f, _middleViewHeight - 30f, _middleViewHeight - 30f));
			_userImageView.Image = UIImage.FromBundle ("mainpage_user");
			_userImageView.UserInteractionEnabled = true;
			_longPressGesture = new UILongPressGestureRecognizer (() => {	
				if (_longPressGesture.State == UIGestureRecognizerState.Began) {
					_longPressGesture.State = UIGestureRecognizerState.Ended;
					RotateUserImage ();
				}
			}){ MinimumPressDuration = 1 };
			_userImageView.AddGestureRecognizer (_longPressGesture);

			_userName = new UILabel ();
			_userName.Text = String.Format ("{0} {1}", _viewModel.User.FirstName, _viewModel.User.LastName);
			_userName.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 3f);
			_userName.TextColor = UIColor.Black;
			_userName.SizeToFit ();
			var uFrame = _userName.Frame;
			uFrame.X = _userImageView.Frame.Right + 5f;
			uFrame.Y = 20f;
			_userName.Frame = uFrame;

			_points = new UILabel ();
			_points.Text = Math.Round (_viewModel.User.Balance_AvailablePoints, 1).ToString ().Replace (",", ".");
			_points.Font = UIFont.BoldSystemFontOfSize (UIFont.LabelFontSize);
			_points.TextColor = UIColor.Clear.FromHexString ("#c9c62b");
			_points.SizeToFit ();
			var pFrame = _points.Frame;
			pFrame.X = _userImageView.Frame.Right + 5f;
			pFrame.Y = _userName.Frame.Bottom + 5f;
			_points.Frame = pFrame;

			_pointsIndicator = new UILabel ();
			_pointsIndicator.Text = ApplicationStrings.Point;
			_pointsIndicator.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
			_pointsIndicator.TextColor = UIColor.Clear.FromHexString ("#c9c62b");
			_pointsIndicator.SizeToFit ();
			var pIFrame = _pointsIndicator.Frame;
			pIFrame.X = _points.Frame.Right + 2f;
			pIFrame.Y = _userName.Frame.Bottom + 11f;
			_pointsIndicator.Frame = pIFrame;


			_middleView.AddSubviews (_userImageView, _userName, _points, _pointsIndicator);

			UIView line = new UIView (new CGRect (0, _middleView.Frame.Top, View.Frame.Width, 2f));
			line.BackgroundColor = UIColor.Clear.FromHexString ("#f3f3f3");
			_middleView.AddSubview (line);
		}

		private void InitBottomView ()
		{
			
			nfloat padding = 15f;
			_bottomView = new UIView (new CGRect (0, 0, View.Frame.Width, View.Frame.Height));

			var ads = new UILabel ();
			ads.Text = ApplicationStrings.Advertisements;
			ads.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 4f);
			ads.TextColor = UIColor.Black;
			ads.SizeToFit ();
			var aFrame = ads.Frame;
			aFrame.X = padding;
			aFrame.Y = _middleView.Frame.Bottom + padding + 2f;
			ads.Frame = aFrame;

			var seeAll = new UIButton (UIButtonType.System);
			seeAll.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
			seeAll.SetTitleColor (UIColor.Clear.FromHexString ("#c9c62b"), UIControlState.Normal);
			seeAll.SetTitle (ApplicationStrings.SeeAll, UIControlState.Normal);
			seeAll.BackgroundColor = UIColor.Clear;
			seeAll.SetImage (UIImage.FromBundle ("right_arrow"), UIControlState.Normal);
			seeAll.TintColor = UIColor.Clear.FromHexString ("#c9c62b");
			seeAll.SizeToFit ();
			var sFrame = seeAll.Frame;
			sFrame.Y = _middleView.Frame.Bottom + padding;
			sFrame.X = View.Frame.Width - sFrame.Width - 15f;
			seeAll.Frame = sFrame;
			seeAll.ImageEdgeInsets = new UIEdgeInsets (6f, seeAll.Frame.Width, 3f, 9f);
			seeAll.TitleEdgeInsets = new UIEdgeInsets (0f, -25f, 0f, 0f);
			seeAll.TouchUpInside += SeeAllAds;


			_adsTableView = new UITableView ();
			_adsTableView.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height - GetStatusBarHeight ());
			_adsTableView.RowHeight = 75f;
			DialogPlugin.ShowProgressDialog ("");
			System.Threading.Tasks.Task.Run (() => {
				var success = _viewModel.GetLoadedAds ();
				InvokeOnMainThread (() => {
					if (success && _viewModel.Advertisments?.Count > 0) {
						_adsTableView.Source = new MainPageAdsTableSource (_viewModel.Advertisments);
						_bottomView.AddSubview (_adsTableView);
						_adsTableView.ReloadData ();
						_newsCounter.BadgeCount = _viewModel.Advertisments.Count;

						UIApplication.SharedApplication.InvokeOnMainThread (() => {
							_newsBarButton.Image = ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
						});
					}
					DialogPlugin.DismissProgressDialog ();
				});
			});

			_refreshController = new UIRefreshControl ();
			_adsTableView.AddSubview (_refreshController);
			_refreshController.ValueChanged += Refresh;

			UIView headerView = new UIView (new CGRect (0, 0, View.Frame.Width, ads.Frame.Bottom + padding));
			headerView.AddSubviews (_topView, _middleView, ads, seeAll);
			_adsTableView.TableHeaderView = headerView;

			_adsTableView.TableFooterView = new UIView ();

			_bottomView.AddSubviews (_adsTableView);
		}

		private void InitUI ()
		{
			InitTopView ();
			InitMiddleView ();
			InitBottomView ();

			View.AddSubviews (_bottomView);
		}

		private void RefreshView ()
		{
			System.Threading.Tasks.Task.Run (() => {
				var success = _viewModel.GetLoadedAds ();
				InvokeOnMainThread (() => {
					if (success && _viewModel.Advertisments?.Count > 0) {
						_adsTableView.Source = new MainPageAdsTableSource (_viewModel.Advertisments);
						_bottomView.AddSubview (_adsTableView);
						_adsTableView.ReloadData ();
						_newsCounter.BadgeCount = _viewModel.Advertisments.Count;

						UIApplication.SharedApplication.InvokeOnMainThread (() => {
							_newsBarButton.Image = ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
						});
					}
				});
			});
		}

		void Refresh (object sender, EventArgs e)
		{
			System.Threading.Tasks.Task.Run (() => {
				var success = _viewModel.GetLoadedAds ();
				InvokeOnMainThread (() => {
					if (success && _viewModel.Advertisments?.Count > 0) {
						_adsTableView.Source = new MainPageAdsTableSource (_viewModel.Advertisments);
						_bottomView.AddSubview (_adsTableView);
						_adsTableView.ReloadData ();
						_newsCounter.BadgeCount = _viewModel.Advertisments.Count;

						UIApplication.SharedApplication.InvokeOnMainThread (() => {
							_newsBarButton.Image = ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
						});
					}
					_refreshController.EndRefreshing ();
				});
			});
		}

		private void OpenNewController (UIViewController newController)
		{
			var controller = new UINavigationController ();
			controller.PushViewController (newController, false);
			controller.NavigationBar.BarStyle = UIBarStyle.Black;
			controller.NavigationBarHidden = false;
			AppDelegate.Instance.SidebarController.ChangeContentView (controller);
		}

		#endregion

		#region Events

		void StopNavigation (object sender, EventArgs e)
		{
			Navigation.Active = false;
			OpenNewController (new MapViewController ());
		}

		void SeeAllAds (object sender, EventArgs e)
		{
			OpenNewController (new AdsViewController (UIPageViewControllerTransitionStyle.Scroll,
				UIPageViewControllerNavigationOrientation.Horizontal,
				UIPageViewControllerSpineLocation.Min));
		}

		void StartNavigation (object sender, EventArgs e)
		{
			Navigation.Active = true;
			OpenNewController (new MapViewController ());
		}

		void BillboardPassed (object sender, Kunicardus.Billboards.Core.DbModels.Billboard e)
		{
			RefreshView ();
		}

		#endregion

	}
}

