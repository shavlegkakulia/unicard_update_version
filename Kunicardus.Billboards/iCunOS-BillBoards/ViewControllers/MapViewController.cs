using System;
using UIKit;
using Kunicardus.Billboards.Core.ViewModels;
using Autofac;
using Google.Maps;
using System.Collections.Generic;
using CoreGraphics;
using CoreAnimation;
using CoreLocation;
using System.Threading.Tasks;
using System.Linq;
using Kunicardus.Billboards.Core.DbModels;

namespace iCunOS.BillBoards
{
	public class MapViewController : BaseViewController
	{
		#region Variables

		private BillboardsViewModel _viewModel;
		private List<Marker> _markers;
		public static LocationManager _locationManager;

		#endregion

		#region UI

		private UIView _distView;
		private nfloat _navigationButtonHeight = 55f;
		private MapView _mapView;
		private UIButton _startNavigation;
		private UIButton _stopNavigation;
		private UIImage _blueMarker;
		private UIImage _redMarker;
		private UIBarButtonItem _newsBarButton;
		private KuniBadgeBarButtonItem _newsCounter;
		private UILabel _distLabel;

		#endregion

		#region Properties

		private bool _navigationMode;

		public bool NavigationModeOn {
			get {
				return _navigationMode;
			}
			set {				
				_navigationMode = value;
				_startNavigation.RemoveFromSuperview ();
				_stopNavigation.RemoveFromSuperview ();
				if (value) {	
					_viewModel.InsertDummyData ();
					// Change map to navigation mode
//					CATransaction.Begin ();
//					CATransaction.AnimationDuration = 0.5f;
					CameraUpdate cameraUpdate = CameraUpdate.SetCamera (new CameraPosition (new CLLocationCoordinate2D (_mapView.Camera.Target.Latitude, _mapView.Camera.Target.Longitude),						                            
						                            17
						, _mapView.Camera.Bearing, 45));
					_mapView.MoveCamera (cameraUpdate);
					//CATransaction.Commit ();	

					_locationManager.LocationUpdated -= HandleLocationChanged;
					_locationManager.LocationUpdated += HandleLocationChanged;
					_locationManager.BillboardPassed -= HandleBillboardPassed;
					_locationManager.BillboardPassed += HandleBillboardPassed;
					_locationManager.StartLocationUpdates ();
					_locationManager.MinDistanceCalculated -= MinDistanceCalculated;
					_locationManager.MinDistanceCalculated += MinDistanceCalculated;

					View.AddSubview (_stopNavigation);
					StartMonitoringRegions ();
				} else {						
					_locationManager.StopLocationUpdates ();

					// Change map to default mode
					CATransaction.Begin ();
					CATransaction.AnimationDuration = 0.5f;
					CameraUpdate cameraUpdate = CameraUpdate.SetCamera (new CameraPosition (new CLLocationCoordinate2D (_mapView.Camera.Target.Latitude, _mapView.Camera.Target.Longitude),
						                            _mapView.Camera.Zoom, _mapView.Camera.Bearing, 0));
					_mapView.Animate (cameraUpdate);
					CATransaction.Commit ();	

					StopMonitoringRegions ();

					View.AddSubview (_startNavigation);	
				}
				Navigation.Active = value;
			}
		}

		void HandleBillboardPassed (object sender, Billboard b)
		{
			InvokeOnMainThread (() => {
				_newsCounter.BadgeCount++;
				_newsBarButton.Image = ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
				DialogPlugin.ShowToast (string.Format ("თქვენ გაიარეთ ბილბორდი: {0}", b.MerchantName));
			});
		}

		#endregion

		#region Ctors

		public MapViewController ()
		{
			using (var scope = App.Container.BeginLifetimeScope ()) {
				_viewModel = scope.Resolve<BillboardsViewModel> ();
			}
			_locationManager = new LocationManager ();
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			Title = ApplicationStrings.Map;

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

			InitUI ();
			// Screen subscribes to the location changed event
			UIApplication.Notifications.ObserveDidBecomeActive ((sender, args) => {	
				_locationManager.LocationUpdated -= HandleLocationChanged;
				_locationManager.LocationUpdated += HandleLocationChanged;
				_locationManager.BillboardPassed -= HandleBillboardPassed;
				_locationManager.BillboardPassed += HandleBillboardPassed;
			});

			// Whenever the app enters the background state, we unsubscribe from the event 
			// so we no longer perform foreground updates
			UIApplication.Notifications.ObserveDidEnterBackground ((sender, args) => {
				_locationManager.LocationUpdated -= HandleLocationChanged;
				_locationManager.BillboardPassed -= HandleBillboardPassed;
			});
		}

		//		public override void ViewWillAppear (bool animated)
		//		{
		//			base.ViewWillAppear (animated);
		//		}
		//
		//		public override void ViewWillDisappear (bool animated)
		//		{
		//			if (_becameActiveObserverObject != null) {
		//				NSNotificationCenter.DefaultCenter.RemoveObserver (_becameActiveObserverObject);
		//			}
		//			if (_locationManager != null) {
		//				_locationManager.StopLocationUpdates ();
		//			}
		//			base.ViewWillDisappear (animated);
		//		}

		public override void LoadView ()
		{
			base.LoadView ();
			_blueMarker = ImageHelper.MaxResizeImage (UIImage.FromBundle ("marker_blue"), 24, 0);
			_redMarker = ImageHelper.MaxResizeImage (UIImage.FromBundle ("marker_red"), 24, 0);
			InitMap ();
		}

		#endregion

		#region Methods

		private UIImage ImageFromView (UIView view)
		{
			UIGraphics.BeginImageContextWithOptions (view.Frame.Size, view.Opaque, 0.0f);
			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return img;
		}


		private void InitMap ()
		{
			CameraPosition camera = CameraPosition.FromCamera (41.7327155, 44.8180819, 7);
			_mapView = MapView.FromCamera (CGRect.Empty, camera);
			_mapView.MyLocationEnabled = true;
			_mapView.Settings.MyLocationButton = true;
			_mapView.BuildingsEnabled = true;
			_mapView.TrafficEnabled = true;
			_mapView.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height - GetStatusBarHeight () - _navigationButtonHeight);				

			this.View.AddSubview (_mapView);

			Task.Run (() => {
				bool success = _viewModel.GetBillboards ();			
				if (success) {
					if (_viewModel.Billboards != null && _viewModel.Billboards.Count > 0) {
						
						UIApplication.SharedApplication.InvokeOnMainThread (() => {
							if (Navigation.Active) {
								StartMonitoringRegions ();
							}
							_markers = _viewModel.Billboards.Select (x => new Marker () {
								Icon = _blueMarker,
								Title = x.MerchantName,
								Position = new CLLocationCoordinate2D ((double)x.Latitude, (double)x.Longitude),
								Map = _mapView,
								Snippet = x.BillboardId.ToString ()
							}
							).ToList ();	

						});
					}						
				}
			});

			Marker oldMarker = null;
			_mapView.MarkerInfoWindow += delegate(MapView mapView, Marker marker) {
				if (oldMarker != null) {
					oldMarker.Icon = _blueMarker;
				}
				marker.Icon = _redMarker;
				oldMarker = marker;

				int bId = int.Parse (marker.Snippet);
				var billboard = _viewModel.Billboards.FirstOrDefault (x => x.BillboardId == bId);
				if (billboard != null) {
					KuniBilboardView infoWindow = new KuniBilboardView (billboard);
					return infoWindow;
				} else
					return null;
			};	


			var buttonSize = 32f;
			var padding = 10f;
			UIButton plus = new UIButton (UIButtonType.System);
			plus.SetTitle ("+", UIControlState.Normal);
			plus.SetTitleColor (UIColor.Clear.FromHexString ("#808080"), UIControlState.Normal);
			plus.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 22f);
			plus.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff", 0.8f);
			plus.TintColor = UIColor.Gray;
			plus.Frame = new CGRect (padding, _mapView.Frame.Height - buttonSize * 2 - padding, buttonSize, buttonSize);

			UIButton minus = new UIButton (UIButtonType.System);
			minus.SetTitle ("-", UIControlState.Normal);
			minus.SetTitleColor (UIColor.Clear.FromHexString ("#808080"), UIControlState.Normal);
			minus.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 22f);
			minus.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff", 0.8f);
			minus.TintColor = UIColor.Gray;
			minus.Frame = new CGRect (padding, plus.Frame.Bottom, buttonSize, buttonSize);

			_mapView.AddSubview (plus);
			_mapView.AddSubview (minus);

			plus.TouchUpInside += delegate {
				CATransaction.Begin ();
				CATransaction.AnimationDuration = 0.2f;
				_mapView.Animate (_mapView.Camera.Zoom + 0.5f);
				CATransaction.Commit ();	
			};
			minus.TouchUpInside += delegate {
				CATransaction.Begin ();
				CATransaction.AnimationDuration = 0.2f;
				_mapView.Animate (_mapView.Camera.Zoom - 0.5f);
				CATransaction.Commit ();	
			};
		}


		private void InitUI ()
		{
			_distView = new UIView (new CGRect (0, 0, View.Frame.Width, 35f));
			_distView.BackgroundColor = UIColor.Clear.FromHexString ("#000000", 0.4f);

			nfloat padding = 15f;
			_distLabel = new UILabel (new CGRect (_distView.Frame.Width / 2f - padding - 5f, 0, _distView.Frame.Width / 2f, _distView.Frame.Height));
			_distLabel.TextColor = UIColor.White;
			_distLabel.Text = "0";
			_distLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 1f);
			_distLabel.TextAlignment = UITextAlignment.Right;

			var metre = new UILabel ();
			metre.TextAlignment = UITextAlignment.Right;
			metre.TextColor = UIColor.White;
			metre.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f);
			metre.Text = "მ";
			metre.SizeToFit ();
			metre.Frame = new CGRect (_distLabel.Frame.Right, 0, metre.Frame.Width, _distView.Frame.Height + 2f);

			var nearestBillboard = new UILabel (new CGRect (padding, 0, _distView.Frame.Width / 2f, _distView.Frame.Height));
			nearestBillboard.TextColor = UIColor.White;
			nearestBillboard.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
			nearestBillboard.Text = "უახლოესი ბილბორდი";
			nearestBillboard.TextAlignment = UITextAlignment.Left;

			_distView.AddSubview (metre);
			_distView.AddSubview (nearestBillboard);
			_distView.AddSubview (_distLabel);


			var response = _viewModel.GetLoadedAdsCount ();
			if (response) {
				_newsCounter.BadgeCount = _viewModel.AdvertisementsCount;
				_newsBarButton.Image = ImageFromView (_newsCounter).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
			}

			_startNavigation = new UIButton (UIButtonType.RoundedRect);
			_startNavigation.Frame = new CGRect (0, View.Frame.Height - GetStatusBarHeight () - _navigationButtonHeight, 
				View.Frame.Width, _navigationButtonHeight);
			_startNavigation.BackgroundColor = UIColor.Clear.FromHexString ("#c9c62b");
			_startNavigation.TintColor = UIColor.White;
			_startNavigation.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16f);
			_startNavigation.SetImage (
				ImageHelper.MaxResizeImage (
					UIImage.FromBundle ("startnavigation"),
					0,
					_navigationButtonHeight - 20f).
				ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal),
				UIControlState.Normal);
			_startNavigation.SetTitle (ApplicationStrings.StartJourney, UIControlState.Normal);
			_startNavigation.ImageEdgeInsets = new UIEdgeInsets (0, -5, 0, 5);
			_startNavigation.TitleEdgeInsets = new UIEdgeInsets (0, 5, 0, -5);
			_startNavigation.TouchUpInside += StartNavigationClicked;

			_stopNavigation = new UIButton (UIButtonType.RoundedRect);
			_stopNavigation.Frame = new CGRect (0, View.Frame.Height - GetStatusBarHeight () - _navigationButtonHeight, 
				View.Frame.Width, _navigationButtonHeight);
			_stopNavigation.BackgroundColor = UIColor.Clear.FromHexString ("#e95936");
			_stopNavigation.TintColor = UIColor.White;
			_stopNavigation.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16f);
			_stopNavigation.SetImage (
				ImageHelper.MaxResizeImage (
					UIImage.FromBundle ("stopnavigation"),
					0,
					_navigationButtonHeight - 20f).
				ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal),
				UIControlState.Normal);
			_stopNavigation.SetTitle (ApplicationStrings.StopJourney, UIControlState.Normal);
			_stopNavigation.ImageEdgeInsets = new UIEdgeInsets (0, -5, 0, 5);
			_stopNavigation.TitleEdgeInsets = new UIEdgeInsets (0, 5, 0, -5);
			_stopNavigation.TouchUpInside += StopNavigationClicked;

			NavigationModeOn = Navigation.Active;

		}

		private void StartMonitoringRegions ()
		{
			if (_viewModel.Billboards != null && _viewModel.Billboards.Count > 0) {				
				_locationManager.StartMonitoringRegion (_viewModel.Billboards, _viewModel);
			}
		}

		private void StopMonitoringRegions ()
		{
			_locationManager.StopMonitoringRegion ();
		}

		#endregion

		#region Events

		private decimal _distance;

		void MinDistanceCalculated (object sender, decimal e)
		{	
			InvokeOnMainThread (() => {
				_distance = e;
				if (NavigationModeOn && e > 1000) {
					_distLabel.Text = (Math.Round (e, 1)).ToString ();
					if (!_distView.IsDescendantOfView (this.View))
						View.AddSubview (_distView);
				} else if (e < 1000 && _distView.IsDescendantOfView (this.View))
					_distView.RemoveFromSuperview ();
			});
		}

		public void HandleLocationChanged (object sender, LocationUpdatedEventArgs e)
		{
			// Handle foreground updates
			CLLocation location = e.Location;
			CATransaction.Begin ();
			CATransaction.AnimationDuration = 0.5f;
			CameraUpdate cameraUpdate = CameraUpdate.SetCamera (
				                            new CameraPosition (new CLLocationCoordinate2D (location.Coordinate.Latitude, 
					                            location.Coordinate.Longitude),
					                            _mapView.Camera.Zoom, location.Course, 45));
			_mapView.Animate (cameraUpdate);
			CATransaction.Commit ();	

			Console.WriteLine ("foreground updated");
		}

		void StartNavigationClicked (object sender, EventArgs e)
		{
			InvokeOnMainThread (() => {
				if (_distance > 1000)
					View.AddSubview (_distView);
				else if (_distView.IsDescendantOfView (this.View))
					_distView.RemoveFromSuperview ();
			});
			NavigationModeOn = true;
		}

		void StopNavigationClicked (object sender, EventArgs e)
		{
			InvokeOnMainThread (() => {
				_distView.RemoveFromSuperview ();
			});
			NavigationModeOn = false;
		}

		#endregion
	}
}

