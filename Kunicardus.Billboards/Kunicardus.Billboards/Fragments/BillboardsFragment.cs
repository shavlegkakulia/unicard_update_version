using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Locations;
using Android.Support.V4.App;
using Android.Provider;
using Android.Graphics;
using Android.Graphics.Drawables;

using Kunicardus.Billboards.Activities;
using Kunicardus.Billboards.Core.ViewModels;
using Kunicardus.Billboards.Core.Services;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Plugins.Geofencing;
using Kunicardus.Billboards.Adapters;
using Kunicardus.Billboards.Plugins;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Autofac;

namespace Kunicardus.Billboards.Fragments
{
    public class BillboardsFragment : BaseFragment
                                                   , IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
                                                   , Android.Gms.Location.ILocationListener
                                                   , GoogleMap.IOnMarkerClickListener, GoogleMap.IOnInfoWindowClickListener
                                                   , GoogleMap.IOnMapClickListener, GoogleMap.IOnMyLocationButtonClickListener
                                                   , IOnMapReadyCallback
    {

        #region Map And Marker Variables

        IGoogleApiClient _googleApiClient;
        LocationRequest _locRequest;
        GoogleMap _map;
        Dictionary<Marker, Billboard> markersDictionary;
        Marker _activeMarker;
        CustomMapFragment _mapFrag;
        InfoWindowAdapter _infoWindowAdapter;

        #endregion

        #region Preview Mode Variables

        public bool GeofencesAreOn { get; set; }

        private bool _follow = false;

        private bool _previewModeEnabled;

        public bool PreviewModeEnabled
        {
            get { return _previewModeEnabled; }
            set
            {
                _previewModeEnabled = TooglePreviewMode(Location, value);
            }
        }

        TextView _distanceUnit;
        TextView _distance;
        Button _btnPreviewMode;
        ImageButton _imgPreviewMode;
        View _rootView;

        public Android.Locations.Location Location { get; set; }

        RelativeLayout _nearestBillboardLayout;

        #endregion

        BillboardsViewModel _viewModel;

        bool isActivated = false;
        bool _pointersSet;

        public override void OnActivate(object o = null)
        {
            //Activity.ActionBar.SetTitle(Resource.String.map);
            if (!_googleApiClient.IsConnected)
            {
                _googleApiClient.Connect();
            }

            if (_map != null && markersDictionary != null && markersDictionary.Count == 0 && !PreviewModeEnabled)
            {
                GetMarkers();
            }

            if (markersDictionary != null && markersDictionary.Count > 0)
            {
                ShowFilteredMarkers();
            }

            isActivated = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BillboardsLayout, null);

            using (var scope = App.Container.BeginLifetimeScope())
            {
                _viewModel = scope.Resolve<BillboardsViewModel>();
            }
            _distanceUnit = view.FindViewById < TextView>(Resource.Id.txtUnit);
            _distance = view.FindViewById<TextView>(Resource.Id.txtDistance);
            _btnPreviewMode = view.FindViewById<Button>(Resource.Id.btnPreviewMode);
            _imgPreviewMode = view.FindViewById<ImageButton>(Resource.Id.imgPreviewMode);
            _nearestBillboardLayout = view.FindViewById<RelativeLayout>(Resource.Id.nearestBillboardLayout);
            _nearestBillboardLayout.Clickable = true;

            var mapFrame = view.FindViewById<FrameLayout>(Resource.Id.mapFrame);

            _rootView = view.RootView;
            _rootView.Click += PrevButtonClick;
            _imgPreviewMode.Click += PrevButtonClick;
            _btnPreviewMode.Click += PrevButtonClick;

            if (_googleApiClient == null)
            {
                _googleApiClient = new GoogleApiClientBuilder(this.Activity)
                                        .AddApi(LocationServices.API)
                                        .AddApi(ActivityRecognition.API)
                                        .AddConnectionCallbacks(this)
                                        .AddOnConnectionFailedListener(this)
                                        .Build();
                _googleApiClient.Connect();
            }

            LoadMap();

            return view;
        }

        private void PrevButtonClick(object sender, EventArgs e)
        {
            if (!PreviewModeEnabled)
            {
                PreviewModeEnabled = true;
                CameraPosition cameraPosition = new CameraPosition.Builder()
                    .Target(new LatLng(Location.Latitude, Location.Longitude))
                    .Zoom(14f)
                    .Build();    
                _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
            else
            {
                PreviewModeEnabled = false;
            }
        }

        public void LoadMap()
        {
            TouchableWrapper wrapper = new TouchableWrapper(this.Activity);
            wrapper.Touched += OnMapDragged;
            _mapFrag = new CustomMapFragment();
            _mapFrag.mTouchView = wrapper;

            Android.Support.V4.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mapFrame, _mapFrag);
            transaction.CommitAllowingStateLoss();
            _mapFrag.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            markersDictionary = new Dictionary<Marker, Billboard>();
            InitMap();
            GetMarkers();
        }

        public void OnMapDragged(object sender, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Move)
            {
                _follow = false;
            }
        }

        #region Map Helper Methods

        public void GetMarkers()
        {
            Task.Run(() =>
                {
                    var billboardsPopulated = _viewModel.GetBillboards();
                    if (billboardsPopulated)
                    {
                        Activity.RunOnUiThread(() =>
                            {
                                SetPointersOnMap(_viewModel.Billboards);
                                if (isActivated && markersDictionary != null && markersDictionary.Count > 0)
                                {
                                    ShowFilteredMarkers();
                                }
                            });
                    }
                });
        }

        private void InitMap()
        {
            if (_map != null)
            {
                _map.UiSettings.ZoomControlsEnabled = true;
                _map.UiSettings.CompassEnabled = true;
                // _map.UiSettings.MapToolbarEnabled = true;
                // _map.MyLocationEnabled = false;
                // _map.UiSettings.MyLocationButtonEnabled = false;

                _map.SetOnMarkerClickListener(this);
                _map.SetOnMapClickListener(this);
                _map.SetOnMyLocationButtonClickListener(this);
                _map.SetOnInfoWindowClickListener(this);
            }
        }

        private void ShowFilteredMarkers()
        {
            LatLngBounds.Builder builder = new LatLngBounds.Builder();
            foreach (var item in markersDictionary.Keys)
            {
                builder.Include(item.Position);
            }
            LatLngBounds bounds = builder.Build();
            CameraUpdate cu = CameraUpdateFactory.NewLatLngBounds(bounds, 40);
            if (_map != null)
            {
                _map.AnimateCamera(cu, 2000, null);
            }
        }

        private void SetInitialMapLocation(double latitude, double longitude)
        {
            Activity.RunOnUiThread(() =>
                {
                    if (_map != null)
                    {
                        Projection projection = _map.Projection;

                        LatLng location = new LatLng(latitude, longitude);
                        CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                        builder.Target(location);

                        int zoomLevel = CalculateZoomLevel();
                        builder.Zoom(zoomLevel);

                        CameraPosition cameraPosition = builder.Build();
                        CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                        //CameraUpdate zoom = CameraUpdateFactory.ZoomTo(zoomLevel);
                        _map.AnimateCamera(cameraUpdate, 2000, null);
                    }
                });
        }

        private int CalculateZoomLevel()
        {
            if (Location == null)
            {
                return 10;
            }
            else
            {
                return 14;
            }
        }

        private void SetPointersOnMap(List<Billboard> billboards)
        {
            if (billboards != null)
            {
                //GeofenceTransitionsIntentService.GeofenceDictionary = new Dictionary<IGeofence, CustomGeofence>();
                //GeofenceTransitionsIntentService.ViewModel = _viewModel;

                Drawable marker = Activity.Resources.GetDrawable(Resource.Drawable.round_map_pointer);
                Bitmap icon = DrawableToBitmap(marker);


                if (_map != null)
                {
                    RemoveMarkers();
                    //RemoveGeofences();
                    foreach (var item in billboards)
                    {
                        Marker mapMarker = AddMarker(item.Latitude, item.Longitude, icon);
                        // AddGeofence(mapMarker, item);
                        markersDictionary.Add(mapMarker, item);
                    }
                    _infoWindowAdapter = new InfoWindowAdapter(this, markersDictionary);
                    _map.SetInfoWindowAdapter(_infoWindowAdapter);
                    _pointersSet = true;
                }
            }
        }

        public void RemoveMarkers()
        {
            foreach (var item in markersDictionary)
            {
                item.Key.Remove();
            }
            _map.Clear();
            _activeMarker = null;
            markersDictionary.Clear();
        }

        private void ChangeActiveMarker(Marker marker)
        {
            RemoveActiveMarker();
            Drawable activePointer = Resources.GetDrawable(Resource.Drawable.round_map_pointer_active);
            Bitmap activeIcon = DrawableToBitmap(activePointer);
            marker.SetIcon(BitmapDescriptorFactory.FromBitmap(activeIcon));
        }

        private void RemoveActiveMarker()
        {
            Drawable inactivePointer = Resources.GetDrawable(Resource.Drawable.round_map_pointer);
            Bitmap inactiveIcon = DrawableToBitmap(inactivePointer);
            if (_activeMarker != null)
            {
                _activeMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(inactiveIcon));
            }
        }

        private void CenterWindow(Marker marker)
        {
            Projection projection = _map.Projection;

            LatLng markerLatLng = new LatLng(marker.Position.Latitude, marker.Position.Longitude);
            Point markerScreenPosition = projection.ToScreenLocation(markerLatLng);
            Point pointHalfScreenAbove = new Point(markerScreenPosition.X, markerScreenPosition.Y);

            LatLng aboveMarkerLatLng = projection.FromScreenLocation(pointHalfScreenAbove);

            marker.ShowInfoWindow();

            CameraUpdate center = CameraUpdateFactory.NewLatLng(aboveMarkerLatLng);
            if (center != null)
            {
                _map.AnimateCamera(center, 2000, null);
            }
        }

        public static Bitmap DrawableToBitmap(Drawable drawable)
        {
            if (drawable is BitmapDrawable)
            {
                return ((BitmapDrawable)drawable).Bitmap;
            }

            Bitmap bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);

            return bitmap;
        }

        private Marker AddMarker(decimal latitude, decimal longtitute, Bitmap icon)
        {
            MarkerOptions marker = new MarkerOptions();
            marker.SetIcon(BitmapDescriptorFactory.FromBitmap(icon));
            marker.SetPosition(new LatLng((double)latitude, (double)longtitute));
            return _map.AddMarker(marker);
        }

        private bool TooglePreviewMode(Location location, bool value)
        {
            #region Empty Check
            if (markersDictionary == null || markersDictionary.Count == 0)
            {
                Toast toast = Toast.MakeText(Activity, Resource.String.no_billboards, ToastLength.Short);
                TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                if (v != null)
                    v.Gravity = GravityFlags.Center;
                toast.Show();
                return false;
            }

            if (location == null)
            {
                Toast toast = Toast.MakeText(Activity, Resource.String.no_location, ToastLength.Short);
                TextView v = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                if (v != null)
                    v.Gravity = GravityFlags.Center;
                toast.Show();

                return false;
            }
            #endregion

            #region Toogle Preview Mode Off
            if (!value)
            {
                CameraPosition cPosition = new CameraPosition.Builder()
                                                                            .Target(new LatLng(location.Latitude, location.Longitude))
                                                                            .Zoom(_map.CameraPosition.Zoom)
                                                                            .Bearing(0)
                                                                            .Tilt(0)
                                                                            .Build();
                _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cPosition));
                //LocationServices.GeofencingApi.RemoveGeofences(_googleApiClient, GetGeofencePendingIntent());
                LocationServices.FusedLocationApi.RemoveLocationUpdates(_googleApiClient, this);

                _distance.Text = "0";
                _btnPreviewMode.Text = Resources.GetString(Resource.String.startTrip);
                _imgPreviewMode.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.trip));
                _rootView.SetBackgroundColor(Color.ParseColor("#C9C62B"));

                GeofencesAreOn = false;
                _map.MyLocationEnabled = false;
                _map.UiSettings.MyLocationButtonEnabled = false;
                _nearestBillboardLayout.Visibility = ViewStates.Gone;
                return false;
            }
            #endregion
            #region Toogle Preview Mode On
            else
            {
                _map.MyLocationEnabled = true;
                _map.UiSettings.MyLocationButtonEnabled = true;
                _nearestBillboardLayout.Visibility = ViewStates.Visible;
                if (_follow)
                {
                    CameraPosition cameraPosition = new CameraPosition.Builder()
                        .Target(new LatLng(location.Latitude, location.Longitude))
                        .Zoom(_map.CameraPosition.Zoom)
                        .Bearing(location.Bearing)
                        .Tilt(45)
                        .Build();    
                    _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
                }

                if (_googleApiClient.IsConnected && !GeofencesAreOn && _pointersSet)
                {
                    GeofencesAreOn = true;
                    // LocationServices.GeofencingApi.AddGeofences(_googleApiClient, GetGeofencingRequest(), GetGeofencePendingIntent());
                    LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, _locRequest, this);

                    _btnPreviewMode.Text = Resources.GetString(Resource.String.endTrip);
                    _imgPreviewMode.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.endTripNew));
                    _rootView.SetBackgroundColor(Color.ParseColor("#E95936"));
                }
                return true;
            }
            #endregion
        }

        #endregion

        #region Fused Location Service Methods

        public void OnConnected(Bundle connectionHint)
        {
            Log.Debug("GooglePlayServices: ", "Connected");

            _locRequest = LocationRequest.Create();
            _locRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            _locRequest.SetInterval(1000);

            //if (isActivated)
            // {
            LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, _locRequest, this);
            // }
        }

        public void OnConnectionSuspended(int cause)
        {
            //Toast.MakeText(Activity, Resource.String.locationSuspended, ToastLength.Short).Show();
        }

        public void OnConnectionFailed(Android.Gms.Common.ConnectionResult result)
        {
            Toast.MakeText(Activity, Resource.String.locationFailed, ToastLength.Short).Show();
        }

        
        public static decimal Direction { get; set; }

        public static decimal Speed { get; set; }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            Location = location;
            Direction = Convert.ToDecimal(location.Bearing); //CalculateDirection(location.Bearing);
            Speed = (decimal)location.Speed * 18 / 5;

            if (PreviewModeEnabled)
            {
                UpdateDistance(location);
                TooglePreviewMode(location, PreviewModeEnabled);
            }
        }

        //float prevDistance = 0;
        int prevBillboardId = -1;

        void UpdateDistance(Location location)
        {
            //var distanceList = new List<float>();
            Android.Locations.Location loc = new Location("gps");
            var markers = markersDictionary.Values.Where(x => x.BillboardId != prevBillboardId && RangeCalculator.InRange(Direction, x.StartBearing, x.EndBearing)).ToList();
            if (markers.Count > 0)
            {
                foreach (var item in markers)
                {
                    loc.Latitude = (float)item.Latitude;
                    loc.Longitude = (float)item.Longitude;
                    var distance = location.DistanceTo(loc);
                    //distanceList.Add(distance);
                    item.Distance = (decimal)distance;
                }
                var marker = markers.Aggregate((minItem, x) => (minItem == null || x.Distance < minItem.Distance ? x : minItem));
                // var distance = marker.Distance;
                _distance.Text = ConvertedDistance(marker.Distance).ToString("0.0");
                ((MainActivity)Activity).UpdateDistance(_distance.Text + _distanceUnit.Text);
                if (marker != null && marker.BillboardId != prevBillboardId && marker.Distance <= marker.AlertDistance)
                {
                    prevBillboardId = marker.BillboardId;
                    SendNotification();
                    //Task.Run(() =>
                    //{                        
                    var billbordUpdated = _viewModel.MarkBillboardAsSeen(marker.BillboardId, marker.AdvertismentId);
                    if (billbordUpdated == true)
                    {
                        ((MainActivity)Activity).IncreaseAlertCount();
                    }
                    //});                
                }
                //prevDistance = distance;
            }
        }

        decimal ConvertedDistance(decimal distance)
        {
            if (distance > 1000)
            {
                _distanceUnit.Text = "კმ";
                return distance / 1000;
            }
            else
            {
                _distanceUnit.Text = "მ";
                return distance;
            }
        }

        void SendNotification()
        {
            var notificationIntent = new Intent(Activity, typeof(MainActivity));

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Activity);

            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));

            stackBuilder.AddNextIntent(notificationIntent);

            var notificationPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            var builder = new NotificationCompat.Builder(Activity);

            using (Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.app_icon))
            {
                builder.SetSmallIcon(Resource.Drawable.app_icon)
                    .SetLargeIcon(bitmap)
                    .SetContentTitle("ახალი შეტყობინება")
                    .SetContentText(GetString(Resource.String.geofence_transition_passed))
                    .SetContentIntent(notificationPendingIntent)
                    .SetSound(Settings.System.DefaultNotificationUri);

                builder.SetAutoCancel(true);

                var mNotificationManager = (NotificationManager)Activity.GetSystemService(Context.NotificationService);

                mNotificationManager.Notify(0, builder.Build());
            }
        }

        #endregion

        #region Geofencing

        //        public void AddGeofence(Marker marker, Billboard billboard)
        //        {
        //            var geofence = new GeofenceBuilder()
        //                                                .SetRequestId(marker.Id)
        //                                                .SetTransitionTypes(Android.Gms.Location.Geofence.GeofenceTransitionEnter)
        //                                                .SetNotificationResponsiveness(1)
        //                                                .SetCircularRegion(marker.Position.Latitude, marker.Position.Longitude, (float)billboard.AlertDistance)
        //                                                .SetExpirationDuration(Android.Gms.Location.Geofence.NeverExpire)
        //                                                .Build();
        //            CustomGeofence item = new CustomGeofence
        //            {
        //                StartBearing = billboard.StartBearing,
        //                EndBearing = billboard.EndBearing,
        //                //Direction = billboard.Direction,
        //                RequestId = geofence.RequestId,
        //                BillboardId = billboard.BillboardId,
        //                //IsSeen = billboard.IsLoaded,
        //                AdvertismentId = billboard.AdvertismentId
        //            };
        //
        //            GeofenceTransitionsIntentService.GeofenceDictionary.Add(geofence, item);
        //            _geofenceList.Add(geofence);
        //        }

        //        private PendingIntent GetGeofencePendingIntent()
        //        {
        //            // Reuse the PendingIntent if we already have it.
        //            if (_geofencePendingIntent != null)
        //            {
        //                return _geofencePendingIntent;
        //            }
        //            Intent intent = new Intent(Activity, typeof(GeofenceTransitionsIntentService));
        //            // We use FLAG_UPDATE_CURRENT so that we get the same pending intent back when
        //            // calling addGeofences() and removeGeofences().
        //            return PendingIntent.GetService(Activity, 0, intent, PendingIntentFlags.UpdateCurrent);
        //        }

        //        private GeofencingRequest GetGeofencingRequest()
        //        {
        //            GeofencingRequest.Builder builder = new GeofencingRequest.Builder();
        //            builder.SetInitialTrigger(GeofencingRequest.InitialTriggerEnter);
        //            builder.AddGeofences(_geofenceList);
        //            return builder.Build();
        //        }

        #endregion

        #region Map Element Click Events

        public bool OnMarkerClick(Android.Gms.Maps.Model.Marker marker)
        {
            ChangeActiveMarker(marker);
            // _mapToolbar.Visibility = ViewStates.Visible;
            _activeMarker = marker;
            // _activeBillboard = markersDictionary.Where(x => x.Key.Id == _activeMarker.Id).FirstOrDefault().Value;

            if (marker.IsInfoWindowShown)
            {
                marker.HideInfoWindow();
            }
            else
            {
                CenterWindow(marker);
            }
            return true;
        }

        public void OnInfoWindowClick(Android.Gms.Maps.Model.Marker marker)
        {
            //throw new NotImplementedException();
        }

        public bool OnMyLocationButtonClick()
        {
            _follow = true;
            return true;
        }

        public void OnMapClick(Android.Gms.Maps.Model.LatLng point)
        {
            if (_activeMarker != null && _activeMarker.IsInfoWindowShown)
            {
                if (_activeMarker != null && _activeMarker.IsInfoWindowShown)
                {
                    RemoveActiveMarker();
                }
            }
        }

        #endregion

        public override bool UserVisibleHint
        {
            get
            {
                return base.UserVisibleHint;
            }
            set
            {
                base.UserVisibleHint = value;
                if (!value)
                {
                    isActivated = false;
                    if (_googleApiClient != null && _googleApiClient.IsConnected && PreviewModeEnabled == false)
                    {
                        LocationServices.FusedLocationApi.RemoveLocationUpdates(_googleApiClient, this);
                        _googleApiClient.Disconnect();
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            if (_googleApiClient != null && _googleApiClient.IsConnected)
            {
                _googleApiClient.Disconnect();
            }
            base.OnDestroy();
        }
    }
}