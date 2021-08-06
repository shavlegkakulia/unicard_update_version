using System;
using Kunicardus.Core.ViewModels;
using UIKit;
using CoreGraphics;
using Google.Maps;
using CoreLocation;
using Kunicardus.Core.ViewModels.iOSSpecific;
using System.Collections.Generic;
using Kunicardus.Core.Models.DB;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using System.Threading.Tasks;
using Kunicardus.Touch.Helpers.UI;
using System.Linq;
using CoreAnimation;
using Foundation;
using CoreVideo;
using SQLite;

namespace Kunicardus.Touch
{
    public class AroundMeViewController : BaseMvxViewController
    {
        #region Vars

        nfloat _listMinY = 0;
        nfloat _listMaxY = 0;
        private List<Marker> _markers;
        private LocationManager _locationManager;

        #endregion

        #region Properties

        public new iMerchantsAroundMeViewModel ViewModel
        {
            get { return (iMerchantsAroundMeViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public int? OrganisationId
        {
            get;
            set;
        }

        private List<MerchantInfo> _merchants;

        public List<MerchantInfo> Merchants
        {
            get
            {
                return _merchants;
            }
            set
            {
                _merchants = value;
                if (ViewModel.ShouldRedrawMap)
                {
                    ViewModel.ShouldRedrawMap = false;
                    _markers = GetMarkers();
                }
            }
        }

        private bool? _mapIsActive;

        public bool? MapIsActive
        {
            get
            {
                return _mapIsActive;
            }
            set
            {
                _mapIsActive = value;
                if (!value.HasValue)
                {
                    _mapOverlay.RemoveFromSuperview();
                    _mapView.AddSubview(_mapOverlay);

                    _listOverlay.RemoveFromSuperview();
                    _list.AddSubview(_listOverlay);
                }
                if (value.HasValue && value.Value)
                {
                    UIView.Animate(0.2, () =>
                    {
                        _list.Frame = new CGRect(0, _listMaxY, _list.Frame.Width, _list.Frame.Height);
                    }, () =>
                    {
                        _mapOverlay.RemoveFromSuperview();
                        _listOverlay.RemoveFromSuperview();
                        _list.AddSubview(_listOverlay);
                    });
                }
                if (value.HasValue && !value.Value)
                {
                    UIView.Animate(0.2, () =>
                    {
                        _list.Frame = new CGRect(0, _listMinY, _list.Frame.Width, _list.Frame.Height);
                    }, () =>
                    {
                        _mapOverlay.RemoveFromSuperview();
                        _mapView.AddSubview(_mapOverlay);
                        _listOverlay.RemoveFromSuperview();
                    });
                }
            }
        }

        #endregion

        #region UI

        private AroundMeList _list;
        private CGRect _originalFrame;
        public MapView _mapView;
        private UITableView _listTableView;

        private UIControl _mapOverlay;
        private UIControl _listOverlay;

        private UIBarButtonItem _SearchButton;
        private UISearchBar _SearchWindow;

        #endregion

        #region Overrides

        UIImage _greenMarker;
        UIImage _orangeMarker;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;
            Title = ApplicationStrings.AroundMe;


            InitUI();

            try
            {
                // Request user to allow app using location
                int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
                if (SystemVersion >= 8)
                {
                    CLLocationManager locationManager = new CLLocationManager();
                    locationManager.RequestWhenInUseAuthorization();
                }
            }
            catch
            {
            }
            // Create bindings
            this.CreateBinding(this)
                .For(view => view.Merchants)
                .To<iMerchantsAroundMeViewModel>(vm => vm.Merchants)
                .Apply();

            _becameActiveObserverObject = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, BecameActive);
            if (ViewModel._orgId.HasValue)
            {
                HideMenuIcon = true;
                NavigationItem.LeftBarButtonItem = null;
                NavigationItem.HidesBackButton = false;
                ShowMenuIcon();
            }
            ViewModel.GetMerchants();
        }

        private NSObject _becameActiveObserverObject;

        // Method style
        void BecameActive(NSNotification notification)
        {
            Console.WriteLine("Received a notification UIApplication", notification);
            StartLocationListening();
        }

        public override void LoadView()
        {
            base.LoadView();
            _greenMarker = UIImage.FromBundle("marker_icon_green");
            _orangeMarker = UIImage.FromBundle("marker_icon_orange");
            InitMap();
        }

        public override void ViewWillAppear(bool animated)
        {
            StartLocationListening();
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (_becameActiveObserverObject != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_becameActiveObserverObject);
            }
            if (_locationManager != null)
            {
                _locationManager.StopLocationUpdates();
            }
            base.ViewWillDisappear(animated);
        }

        #endregion

        #region Methods

        private void StartLocationListening()
        {
            if (_locationManager != null && ViewModel.LocationEnabled() && _mapView != null && _mapView.SelectedMarker == null)
            {
                _locationManager.FirstLocationUpdated += FirstLocationUpdated;
                _locationManager.StartLocationUpdates();
            }
        }

        private List<Marker> GetMarkers()
        {
            _mapView.Clear();
            List<Marker> markers = new List<Marker>();

            if (_merchants != null && _merchants.Count > 0)
            {
                markers = _merchants.Where(m => m != null && m.Latitude > 0 && m.Longitude > 0 && !string.IsNullOrWhiteSpace(m.MerchantId)).Select(x => new Marker()
                {
                    Icon = _greenMarker,
                    Title = x.MerchantName,
                    Position = new CLLocationCoordinate2D(x.Latitude.Value, x.Longitude.Value),
                    Map = _mapView,
                    Snippet = x.MerchantId
                }
                ).ToList();
            }

            if (!_myLocationUpdated)
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    CoordinateBounds bounds = new CoordinateBounds();
                    foreach (var item in markers)
                    {
                        bounds = bounds.Including(item.Position);
                    }

                    _mapView.Animate(CameraUpdate.FitBounds(bounds));
                    //adjusting bounds for only one marker
                    if (markers.Count == 1 ||
                        (markers.Count == 2 &&
                        markers[0].Position.Latitude == markers[1].Position.Latitude &&
                        markers[0].Position.Longitude == markers[1].Position.Longitude))
                        _mapView.Animate(16f);
                });
            return markers;
        }

        private void InitUI()
        {
            nfloat statusbarHeight = GetStatusBarHeight();
            nfloat listHeight = View.Frame.Height - statusbarHeight - statusbarHeight;
            _listMinY = View.Frame.Height - listHeight;
            _listMaxY = View.Frame.Height - statusbarHeight;

            _list = new AroundMeList(new CGRect(0, View.Frame.Height / 2.0f, View.Frame.Width, listHeight));
            _list.UserInteractionEnabled = true;
            _originalFrame = _list.Frame;

            UIPanGestureRecognizer gesture = new UIPanGestureRecognizer();
            gesture.MinimumNumberOfTouches = 1;
            gesture.MaximumNumberOfTouches = 1;
            gesture.AddTarget(() => HandleDrag(gesture));
            _list.AddGestureRecognizer(gesture);

            // init list overlay
            _listOverlay = new UIControl(new CGRect(0, 0, View.Frame.Width, listHeight));
            _listOverlay.BackgroundColor = UIColor.Gray;
            _listOverlay.Alpha = 0.3f;
            _listOverlay.TouchUpInside += delegate
            {
                MapIsActive = false;
            };

            // Init map overlay
            _mapOverlay = new UIControl(new CGRect(0, 0, View.Frame.Width, View.Frame.Height - statusbarHeight - statusbarHeight));
            _listOverlay.BackgroundColor = UIColor.Gray;
            _listOverlay.Alpha = 0.3f;
            _mapOverlay.TouchUpInside += delegate
            {
                MapIsActive = true;
            };

            _locationManager = new LocationManager();

            // Maps
            //InitMap ();

            InitListData();
            View.AddSubview(_list);

            MapIsActive = true;


            // Init Search
            if (_SearchButton == null)
            {
                _SearchButton = new UIBarButtonItem(UIBarButtonSystemItem.Search, (s, e) =>
                {
                    ShowSearchWindow();
                });
            }
            NavigationItem.RightBarButtonItem = _SearchButton;
        }

        private KeyboardTopBar _keyboardBar;

        private void ShowSearchWindow()
        {
            if (_SearchWindow == null)
            {
                _SearchWindow = new UISearchBar();
                _SearchWindow.Placeholder = ApplicationStrings.Search;
                _SearchWindow.KeyboardType = UIKeyboardType.WebSearch;
                _keyboardBar = new KeyboardTopBar();
                _keyboardBar.NextEnabled = false;
                _keyboardBar.PreviousEnabled = false;
                _keyboardBar.OnDone += delegate
                {
                    _SearchWindow.ResignFirstResponder();
                };
                _SearchWindow.InputAccessoryView = _keyboardBar;
                _SearchWindow.TintColor = UIColor.White;
                _SearchWindow.ShowsScopeBar = true;
                _SearchWindow.SearchButtonClicked += (sender, e) =>
                {
                    ViewModel.FilterMerchants(_SearchWindow.Text);
                    _SearchWindow.ResignFirstResponder();
                };


                _SearchWindow.ShowsCancelButton = true;

                _SearchWindow.CancelButtonClicked += delegate
                {
                    DisposeSearchWindow();
                };
            }

            ShowSearchWindow(_SearchWindow);
        }

        private void DisposeSearchWindow()
        {
            ShowMenuIcon();
            NavigationItem.RightBarButtonItem = _SearchButton;
            ViewModel.FilterMerchants("");
            UIView.Animate(
                0.1, // duration
                () =>
                {
                    NavigationItem.TitleView = null;
                    NavigationItem.Title = ApplicationStrings.AroundMe;
                }
            );
        }

        private void ShowSearchWindow(UIView searchView)
        {
            NavigationItem.LeftBarButtonItem = null;
            NavigationItem.RightBarButtonItem = null;
            UIView.Animate(
                0.1, // duration
                () =>
                {
                    NavigationItem.TitleView = searchView;
                },
                () =>
                {
                    searchView.BecomeFirstResponder();
                }
            );
        }

        private void InitMap()
        {
            nfloat statusbarHeight = GetStatusBarHeight();
            CameraPosition camera = CameraPosition.FromCamera(41.989722, 43.59, 6.5f);

            _mapView = MapView.FromCamera(CGRect.Empty, camera);
            _mapView.MyLocationEnabled = true;
            _mapView.Settings.MyLocationButton = true;
            _mapView.Settings.CompassButton = true;
            _mapView.AddGestureRecognizer(new UIGestureRecognizer(() =>
            {
                _oldMarker.Icon = _greenMarker;
            }));
            _mapView.BuildingsEnabled = true;
            _mapView.TrafficEnabled = true;
            _mapView.Frame = new CGRect(0, statusbarHeight, View.Frame.Width, View.Frame.Height - statusbarHeight - statusbarHeight);
            _mapView.TappedMarker += delegate (MapView mapView, Marker marker)
            {
                SelectMarker(marker);
                return false;
            };

            _mapView.MarkerInfoWindow += delegate (MapView mapView, Marker marker)
            {
                int merchantId = int.Parse(marker.Snippet);
                var merchant = _merchants.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.MerchantId) && int.Parse(x.MerchantId) == merchantId);
                if (merchant != null)
                {
                    KuniMarkerInfoWindow infoWindow = new KuniMarkerInfoWindow(merchant);
                    return infoWindow;
                }
                else
                {
                    return null;
                }
            };

            _mapView.InfoTapped += delegate (object sender, GMSMarkerEventEventArgs e)
            {
                int merchantId = int.Parse(e.Marker.Snippet);
                InfoWindowClicked(merchantId);
            };

            var buttonSize = 34f;
            var padding = 10f;
            UIButton plus = new UIButton(UIButtonType.System);
            plus.SetTitle("+", UIControlState.Normal);
            plus.SetTitleColor(UIColor.Clear.FromHexString("#808080"), UIControlState.Normal);
            plus.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 22f);
            plus.BackgroundColor = UIColor.Clear.FromHexString("#ffffff", 0.8f);
            plus.TintColor = UIColor.Gray;
            plus.Frame = new CGRect(padding, _mapView.Frame.Height - buttonSize * 2 - padding, buttonSize, buttonSize);

            UIButton minus = new UIButton(UIButtonType.System);
            minus.SetTitle("-", UIControlState.Normal);
            minus.SetTitleColor(UIColor.Clear.FromHexString("#808080"), UIControlState.Normal);
            minus.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 22f);
            minus.BackgroundColor = UIColor.Clear.FromHexString("#ffffff", 0.8f);
            minus.TintColor = UIColor.Gray;
            minus.Frame = new CGRect(padding, plus.Frame.Bottom, buttonSize, buttonSize);

            _mapView.AddSubview(plus);
            _mapView.AddSubview(minus);

            plus.TouchUpInside += delegate
            {
                CATransaction.Begin();
                CATransaction.AnimationDuration = 0.2f;
                _mapView.Animate(_mapView.Camera.Zoom + 0.5f);
                CATransaction.Commit();
            };
            minus.TouchUpInside += delegate
            {
                CATransaction.Begin();
                CATransaction.AnimationDuration = 0.2f;
                _mapView.Animate(_mapView.Camera.Zoom - 0.5f);
                CATransaction.Commit();
            };

            this.View.AddSubview(_mapView);
        }

        Marker _oldMarker;

        private void SelectMarker(Marker marker, bool moveCamera = false)
        {
            MapIsActive = true;

            if (_oldMarker != null && _oldMarker != marker)
            {
                _oldMarker.Icon = _greenMarker;
            }
            marker.Icon = _orangeMarker;
            _oldMarker = marker;

            if (moveCamera)
            {
                _mapView.SelectedMarker = marker;
                CATransaction.Begin();
                CATransaction.AnimationDuration = 1.2f;
                _mapView.Animate(CameraUpdate.SetTarget(new CLLocationCoordinate2D(marker.Position.Latitude, marker.Position.Longitude),
                    15));
                CATransaction.Commit();

            }
        }

        private void InfoWindowClicked(int merchantId)
        {
            var merchant = _merchants.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.MerchantId) && int.Parse(x.MerchantId) == merchantId);
            if (merchant != null && merchant.OrganizationId > 0)
            {
                ViewModel.OpenOrgDetils(merchant.OrganizationId);
            }
        }

        private void InitListData()
        {
            _listTableView = new UITableView(new CGRect(0, 10, _list.Frame.Width, _list.Frame.Height - 10));
            _listTableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
            _listTableView.AllowsSelection = true;
            var source = new MvxSimpleTableViewSource(_listTableView, typeof(MerchantListRow));
            _listTableView.Source = source;
            _listTableView.RowHeight = 80f;

            this.CreateBinding(source).To((iMerchantsAroundMeViewModel vm) => vm.Merchants).Apply();

            _listTableView.ReloadData();
            _list.AddSubview(_listTableView);

            source.SelectedItemChanged += delegate (object sender, EventArgs e)
            {
                if (source.SelectedItem != null)
                {
                    MerchantInfo mInfo = (MerchantInfo)source.SelectedItem;
                    Marker correspondingMarker = _markers.FirstOrDefault(x => x.Snippet == mInfo.MerchantId);
                    if (correspondingMarker != null)
                    {
                        SelectMarker(correspondingMarker, true);
                    }
                }
            };
        }

        #endregion

        #region Events

        private bool _myLocationUpdated;

        void FirstLocationUpdated(object sender, LocationUpdatedEventArgs e)
        {
            if (!ViewModel._orgId.HasValue)
            {
                _myLocationUpdated = true;
                ViewModel.Latitude = e.Location.Coordinate.Latitude;
                ViewModel.Longitude = e.Location.Coordinate.Longitude;

                CATransaction.Begin();
                CATransaction.AnimationDuration = 1.2f;
                _mapView.Animate(CameraUpdate.SetTarget(new CLLocationCoordinate2D(e.Location.Coordinate.Latitude, e.Location.Coordinate.Longitude),
                    16f));
                CATransaction.Commit();
                UpdateDistances(e);
                _locationManager.FirstLocationUpdated -= FirstLocationUpdated;
                _locationManager.StopLocationUpdates();
            }
        }

        private void UpdateDistances(LocationUpdatedEventArgs e)
        {
            string searchText = "";
            if (_SearchWindow != null)
                searchText = _SearchWindow.Text;

            if (string.IsNullOrWhiteSpace(searchText) &&
                _mapView != null && _mapView.SelectedMarker == null)
            {

                ViewModel.Latitude = e.Location.Coordinate.Latitude;
                ViewModel.Longitude = e.Location.Coordinate.Longitude;

                ViewModel.UpdateDistances();

                //				CATransaction.Begin ();
                //				CATransaction.AnimationDuration = 0.4f;
                //				_mapView.Animate (CameraUpdate.SetTarget (new CLLocationCoordinate2D (e.Location.Coordinate.Latitude, e.Location.Coordinate.Longitude),
                //					_mapView.Camera.Zoom));
                //				CATransaction.Commit ();	

            }
        }

        private void HandleDrag(UIPanGestureRecognizer recognizer)
        {

            // If it's just began, cache the location of the image
            if (recognizer.State == UIGestureRecognizerState.Began)
            {
                _originalFrame = _list.Frame;
            }

            //			// Move the image if the gesture is valid
            if (recognizer.State != (UIGestureRecognizerState.Cancelled | UIGestureRecognizerState.Failed
                | UIGestureRecognizerState.Possible))
            {
                // Move the image by adding the offset to the object's frame
                CGPoint offset = recognizer.TranslationInView(_list);
                CGRect newFrame = _originalFrame;

                newFrame.Offset(_originalFrame.X, offset.Y);
                if (newFrame.Y >= _listMinY && newFrame.Y <= _listMaxY)
                {
                    _list.Frame = newFrame;
                }
            }

            if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                CGPoint offset = recognizer.TranslationInView(_list);
                CGRect newFrame = _originalFrame;

                newFrame.Offset(_originalFrame.X, offset.Y);
                if (newFrame.Y > View.Frame.Height / 1.5f)
                {
                    MapIsActive = true;
                }
                if (newFrame.Y <= View.Frame.Height / 1.5f)
                {
                    MapIsActive = false;
                }
            }
        }

        #endregion
    }
}

