
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Kuni.Core.Models.DB;
using Kuni.Core.ViewModels;
using Kunikardus.Droid.Plugins.SlidingUpPanel;
//using MvvmCross.Binding.Droid.Views;
using Android.Gms.Common.Apis;
using MvvmCross.Binding.BindingContext;
using Android.Gms.Common;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using System.Net;
using Kunicardus.Droid.Adapters;
using Android.Gms.Location;
using Kuni.Core;
using Kunicardus.Droid.Fragments;
using Com.Google.Maps.Android.Clustering;
using Com.Google.Maps.Android.Clustering.View;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.Views;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android;

namespace Kunicardus.Droid
{
    [Activity(Label = "MerchantsView",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
        Name = "ge.unicard.unicardmobileapp.MerchantsView")]
    public class MerchantsView : MvxFragmentActivity
                                ,GoogleApiClient.IConnectionCallbacks
                                ,GoogleApiClient.IOnConnectionFailedListener
                                , Android.Gms.Location.ILocationListener
                                , GoogleMap.IOnMarkerClickListener
                                , GoogleMap.IOnInfoWindowClickListener
                                , GoogleMap.IOnMapClickListener
                                , GoogleMap.IOnMyLocationButtonClickListener
                                , AbsListView.IOnScrollListener
                                , AdapterView.IOnItemClickListener
                                , TextView.IOnEditorActionListener
                                , ClusterManager.IOnClusterClickListener
                                , ClusterManager.IOnClusterItemClickListener
                                , GoogleMap.IOnCameraChangeListener
        ,IOnMapReadyCallback
    {
        MerchantsViewModel _viewModel;
        ClusterManager _clusterManager;
        SupportMapFragment _mapFrag;
        GoogleMap _map;
        GoogleApiClient _googleApiClient;
        LocationRequest _locRequest;

        bool _markerClicked;
        int? _organizationId;
        bool isSearchBoxActive;
        bool isFiltering, mylocationClicked,
            _gesturesEnabled = true, isActivated = false;

        TextView _title;
        AutoCompleteTextView searchText;
        ImageView imgSearch;
        View searchLine;
        ImageButton _imgDirection;
        ImageButton _imgLocation;
        RelativeLayout _mapToolbar;
        SlidingUpPanelLayout _slidingLayout;
        FrameLayout _mapFrame;
        MvxListView _merchantsListView;
        Marker _activeMarker;
        MerchantInfo _activeMerchant;
        ProgressDialog _progressDialog;
        InfoWindowAdapter _infoWindowAdapter;

        public static BitmapDescriptor _icon
        {
            get;
            set;
        }

        public int? OrganizationId
        {
            get
            {
                return _organizationId;
            }
            set
            {
                _organizationId = value;
            }
        }

        public bool MerchantsUpdated
        {
            set
            {
                if (value)
                {
                    OnMerchantsUpdated();
                }
            }
            get { return false; }
        }

        public int OrgId { get; set; }

        public bool LocationUpdated { get; set; }


        #region Location Permission Request
        public static int LOCATION_Permission_REQUEST_CODE = 1;
        protected readonly string[] PermissionsLocation =
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

        static readonly int RequestLocationId = 98;

        public void RequestLocationPermissions()
        {
            const string permission = Manifest.Permission.AccessFineLocation;
            if (ActivityCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted)
            {
                LoadMapAndGoogleServices();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
                return;
            }

            ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [Android.Runtime.GeneratedEnum] Permission[] grantResults)
        {
            if (grantResults[0] == Permission.Granted)
            {
                LoadMapAndGoogleServices();
            }
        }
        #endregion

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            OrgId = this.Intent.GetIntExtra("OrgId", -1);

        }

        protected override async void OnViewModelSet()
        {
            base.OnViewModelSet();
            SetContentView(Resource.Layout.MerchantsListView);

            _progressDialog = ProgressDialog.Show(this, null, Resources.GetString(Resource.String.loadingMerchants));

            FindViewById<ImageView>(Resource.Id.menuImg).Click += (o, e) =>
            {

                HideKeyboard();
                this.Finish();
            };
            _viewModel = this.ViewModel as MerchantsViewModel;

            #region Init Views
            _title = FindViewById<TextView>(Resource.Id.pageTitle);

            //await Task.Run(() =>
            //{
                FindViewById<TextView>(Resource.Id.pageTitle).Text = GetString(Resource.String.merchants);
                searchText = FindViewById<AutoCompleteTextView>(Resource.Id.search);
                imgSearch = FindViewById<ImageView>(Resource.Id.alert);
                searchLine = FindViewById<View>(Resource.Id.view1);
                searchText.SetOnEditorActionListener(this);
                searchText.TextChanged += searchText_TextChanged;

                _imgDirection = FindViewById<ImageButton>(Resource.Id.imgDirection);
                _imgLocation = FindViewById<ImageButton>(Resource.Id.imgLocation);
                _mapToolbar = FindViewById<RelativeLayout>(Resource.Id.mapToolbar);

                _imgDirection.Click += OnDirectionsClick;
                _imgLocation.Click += OnLocationClick;

                _slidingLayout = FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_layout);
                _slidingLayout.AnchorPoint = 0.13f;
                _slidingLayout.PanelCollapsed += PanelCollapsed;
                _slidingLayout.PanelExpanded += PanelExpanded;

                imgSearch.Click += SearchClick;
                isSearchBoxActive = false;

                _mapFrame = FindViewById<FrameLayout>(Resource.Id.mapFrame);
                _merchantsListView = FindViewById<MvxListView>(Resource.Id.merchantsList);
                _merchantsListView.SetOnScrollListener(this);
                _merchantsListView.OnItemClickListener = this;

                #endregion

                isActivated = true;

                RequestLocationPermissions();
            //});

        }

        void LoadMapAndGoogleServices()
        {
            MapsInitializer.Initialize(this);
            InitMapFragment();

            if (_googleApiClient == null)
            {
                _googleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(LocationServices.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();
                _googleApiClient.Disconnect();
            }
            _mapFrag.GetMapAsync(this);

            Drawable marker = this.Resources.GetDrawable(Resource.Drawable.round_map_pointer);
            Bitmap icon = DrawableToBitmap(marker);
            _icon = BitmapDescriptorFactory.FromBitmap(icon);

            var set = this.CreateBindingSet<MerchantsView, MerchantsViewModel>();
            set.Bind(this).For(v => v.MerchantsUpdated).To(vmod => vmod.DataPopulated);
            set.Apply();

            (this.ViewModel as MerchantsViewModel).UpdateMerchants();
            (this.ViewModel as MerchantsViewModel).GetUserSettings();
        }

        bool _appWasInBackground;

        DateTime _lastDateTime;

        public bool _pinIsOpened;

        protected override void OnPause()
        {
            _appWasInBackground = true;
            _lastDateTime = DateTime.Now;
            base.OnPause();
        }

        protected override void OnResume()
        {
            if (_appWasInBackground)
            {
                _appWasInBackground = false;

                var secondsDifference = DateTime.Now.Ticks / TimeSpan.TicksPerSecond - _lastDateTime.Ticks / TimeSpan.TicksPerSecond;
                if (secondsDifference > 60 && !_pinIsOpened)
                {
                    if (_viewModel.UserSettings != null && _viewModel.UserSettings.Pin != null)
                    {
                        OpenPinInputDialog(_viewModel.UserSettings.Pin);
                        HideKeyboard();
                    }
                }
            }
            base.OnResume();
        }

        public void OpenPinInputDialog(string pinFromDB)
        {
            _pinIsOpened = true;
            PinInputDialogFragment d = new PinInputDialogFragment(pinFromDB, true);
            d.Show(FragmentManager, "");
            d.SetStyle(DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
            d.Cancelable = false;
        }

        public override void OnBackPressed()
        {
            var frag = this.SupportFragmentManager.FindFragmentByTag("details");
            if (frag != null)
            {
                this.SupportFragmentManager.BeginTransaction().Remove(frag).Commit();
            }
            else
            {
                base.OnBackPressed();
                this.Finish();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (_clusterManager != null)
            {
                _clusterManager.Dispose();
            }
            //			GC.Collect (0);
        }

        #region SlidingPanel Events

        private void PanelCollapsed(object sender, SlidingUpPanelEventArgs args)
        {
            if (_map != null)
            {
                _map.UiSettings.SetAllGesturesEnabled(true);
                _gesturesEnabled = true;
                _map.MyLocationEnabled = true;
                _map.UiSettings.CompassEnabled = true;
            }
        }

        void PanelExpanded(object sender, SlidingUpPanelEventArgs args)
        {
            if (_map != null)
            {
                _map.UiSettings.SetAllGesturesEnabled(false);
                _gesturesEnabled = false;
                _map.MyLocationEnabled = false;
                _map.UiSettings.CompassEnabled = false;
            }
        }

        #endregion

        #region SearchBox Controls

        void searchText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CreateSuggestionsAdapter(searchText.Text);
        }

        public void CreateSuggestionsAdapter(string text)
        {
            if (_viewModel != null)
            {
                var suggestions = _viewModel.GetFilterSuggestions(text);
                ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, suggestions);
                searchText.Adapter = adapter;
            }
        }

        void SearchClick(object sender, EventArgs e)
        {
            if (!_slidingLayout.IsCollapsed)
            {
                _slidingLayout.CollapsePane();
            }
            if (!isSearchBoxActive)
            {
                _title.Visibility = ViewStates.Invisible;
                searchLine.Visibility = ViewStates.Visible;
                searchText.Visibility = ViewStates.Visible;
                searchText.RequestFocus();
                ShowKeyboard();
                imgSearch.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.abc_ic_clear_material));
                isSearchBoxActive = true;
                isFiltering = true;
            }
            else
            {
                _viewModel.GetMerchants();
                CloseSearch();
            }
        }

        private void CloseSearch()
        {
            _title.Visibility = ViewStates.Visible;
            searchLine.Visibility = ViewStates.Gone;
            searchText.Visibility = ViewStates.Gone;
            searchText.Text = "";
            isSearchBoxActive = false;
            isFiltering = false;
            imgSearch.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.search));
            HideKeyboard();
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Search)
            {
                _viewModel.FilterMerchants(searchText.Text);
                HideKeyboard();
                return true;
            }
            return false;
        }

        private void HideKeyboard()
        {
            View view = base.CurrentFocus;
            if (view != null)
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);
            }
            searchText.ClearFocus();
        }

        private void ShowKeyboard()
        {
            View view = base.CurrentFocus;
            if (view != null)
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
                inputManager.ShowSoftInput(view, ShowFlags.Forced);
            }
        }


        #endregion

        void OnMerchantsUpdated()
        {
            try
            {
                if (!isFiltering)
                {
                    if (_viewModel != null && _viewModel.Latitude.HasValue && _viewModel.Longitude.HasValue)
                    {

                        ZoomMap(_viewModel.Longitude.Value, _viewModel.Latitude.Value, true);
                    }
                    else
                    {
                        ZoomMap(0, 0, false);
                    }
                }
                _activeMarker = null;

                if (mylocationClicked)
                {
                    if (_progressDialog != null)
                    {
                        _progressDialog.Dismiss();
                    }
                    mylocationClicked = false;
                    return;
                }

                if (isFiltering || _clusterManager.MarkerCollection.Markers.Count == 0 || _viewModel.ShouldUpdateMap)
                {
                    SetPointersOnMap(_viewModel.Merchants);
                }
                if (isFiltering)
                {
                    ShowFilteredMarkers();
                }
                if (_progressDialog != null)
                {
                    _progressDialog.Dismiss();
                }
            }
            catch (System.Exception ex)
            {
                if (_progressDialog != null)
                {
                    _progressDialog.Dismiss();
                }
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        private void InitMapFragment()
        {
            _mapFrag = SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
            if (_mapFrag == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);
                var fragTx = SupportFragmentManager.BeginTransaction();
                _mapFrag = SupportMapFragment.NewInstance(mapOptions);
                fragTx.Replace(Resource.Id.mapFrame, _mapFrag);
                fragTx.Commit();
            }
        }

        private async void SetupMapIfNeeded()
        {
            //if (_map == null)
            //{
                //_map = _mapFrag.GetMapAsync(this);
                if (_map != null)
                {
                    _clusterManager = new ClusterManager(this, _map);
                    _clusterManager.SetOnClusterItemClickListener(this);
                    _clusterManager.SetOnClusterClickListener(this);
                    _map.SetOnCameraChangeListener(this);
                    _map.SetOnMarkerClickListener(_clusterManager);
                    _map.SetOnMapClickListener(this);
                    _map.SetOnMyLocationButtonClickListener(this);
                    _map.SetOnInfoWindowClickListener(this);
                    _map.UiSettings.MapToolbarEnabled = false;


                //Initialize cluster manager. Setting the CameraIdleListener is mandatory       
                _map.SetOnCameraIdleListener(_clusterManager);
                _map.SetInfoWindowAdapter(_clusterManager.MarkerManager);

                //Handle Info Window's click event
                _map.SetOnInfoWindowClickListener(_clusterManager);
              //  _clusterManager.SetOnClusterItemInfoWindowClickListener(this);

                _clusterManager.Renderer = (new CustomClusterRenderer(this, _map, _clusterManager));
                }
            //}

            if (!isFiltering)
            {
                if (_viewModel.LocationEnabled())
                {
                    if (!_googleApiClient.IsConnected)
                    {
                        _googleApiClient.Connect();
                    }
                }
                else
                {
                    if (_googleApiClient.IsConnected)
                    {
                        _googleApiClient.Disconnect();
                        _map.MyLocationEnabled = false;
                    }
                }
            }
            _slidingLayout.CollapsePane();

            if (OrgId != -1)
            {
                isFiltering = true;
                _viewModel.GetMerchants(OrgId);
            }
            else
            {
                isFiltering = false;
                if (_viewModel.Merchants == null || _viewModel.ShouldUpdateMap)
                {
                    _viewModel.GetMerchants();
                }
            }
        }

        public bool OnClusterClick(ICluster cluster)
        {
            Toast.MakeText(this, cluster.Items.Count + " ობიექტი", ToastLength.Short).Show();
            return false;
        }

        public bool OnClusterItemClick(Java.Lang.Object item)
        {
            _markerClicked = true;
            _activeMerchant = _viewModel.Merchants.FirstOrDefault(x => x.MerchantId == ((ClusterItem)item).MerchantId);
            ZoomMap(((ClusterItem)item).Position.Longitude, ((ClusterItem)item).Position.Latitude, true);
            return true;
        }


        private void ZoomMap(double longitude, double latitude, bool havePosition)
        {
            if (_map != null)
            {
                if (havePosition)
                {
                    SetInitialMapLocation(latitude, longitude, false);
                }
                else
                {
                    SetInitialMapLocation(42.04113401, 43.84643555, true);
                }
            }
        }

        private void SetInitialMapLocation(double latitude, double longitude, bool isDefault)
        {
            RunOnUiThread(() =>
            {
                if (_map != null)
                {
                    var container_height = _mapFrame.Height;

                    Projection projection = _map.Projection;

                    LatLng location = new LatLng(latitude, longitude);
                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(location);

                    var zoomLevel = CalculateZoomLevel(isDefault);
                    builder.Zoom(zoomLevel);

                    CameraPosition cameraPosition = builder.Build();
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    _map.AnimateCamera(cameraUpdate, 1000, null);

                }
            });
        }

        private float CalculateZoomLevel(bool isDefault)
        {
            if (isDefault)
            {
                return 6.5f;
            }
            else
            {
                return 16;
            }
        }

        private void SetPointersOnMap(List<MerchantInfo> merchants)
        {
            if (merchants != null)
            {
                if (isFiltering)
                {
                    _clusterManager.ClearItems();
                }
                _clusterManager.ClearItems();
                _clusterManager.ClusterMarkerCollection.Clear();
                _clusterManager.MarkerCollection.Clear();


                if (_map != null)
                {
                    var count = merchants.Count;

                    List<ClusterItem> items = new List<ClusterItem>();

                    items = merchants.Where(item => item.Longitude.HasValue && item.Latitude.HasValue)
                        .Select(item => new ClusterItem(item.Latitude.Value, item.Longitude.Value, item.MerchantId)).ToList();
                    _clusterManager.AddItems(items);
                }
                _clusterManager.Cluster();
                _infoWindowAdapter = new InfoWindowAdapter(null, merchants, this.LayoutInflater);
                _map.SetInfoWindowAdapter(_infoWindowAdapter);
            }
        }

        private void ShowFilteredMarkers()
        {
            LatLngBounds.Builder builder = new LatLngBounds.Builder();
            if (_viewModel.Merchants == null || _viewModel.Merchants.Count == 0)
            {
                return;
            }
            foreach (var item in _viewModel.Merchants.Where(x => x.Latitude.HasValue && x.Longitude.HasValue))
            {
                LatLng position = new LatLng(item.Latitude.Value, item.Longitude.Value);
                builder.Include(position);
            }

            LatLngBounds bounds = builder.Build();
            CameraUpdate cu = CameraUpdateFactory.NewLatLngBounds(bounds, 40);
            if (_map != null)
            {
                _map.AnimateCamera(cu, 2000, null);
            }
        }


        private void OnLocationClick(object sender, EventArgs e)
        {
            try
            {
                string url = string.Format("http://maps.google.com/maps?q={0},{1}&iwloc=A&hl=es", _activeMerchant.Latitude, _activeMerchant.Longitude);
                Intent mapIntent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse(url));
                mapIntent.SetPackage("com.google.android.apps.maps");
                StartActivity(mapIntent);
            }
            catch
            {
                Toast.MakeText(this, Resource.String.error_occured, ToastLength.Long).Show();
            }
        }

        void OnDirectionsClick(object sender, EventArgs e)
        {
            try
            {
                string url = string.Format("http://maps.google.com/maps?daddr={0},{1}", _activeMerchant.Latitude, _activeMerchant.Longitude);
                Intent intent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse(url));
                StartActivity(intent);
            }
            catch
            {
                Toast.MakeText(this, Resource.String.error_occured, ToastLength.Long).Show();
            }
        }

        public bool OnMyLocationButtonClick()
        {
            isFiltering = false;
            mylocationClicked = true;
            _viewModel.Merchants = _viewModel.GetMerchantsWithDistance(_viewModel.Merchants);
            return true;
        }

        public bool OnMarkerClick(Marker marker)
        {
            _markerClicked = false;
            if (_gesturesEnabled)
            {

                ChangeActiveMarker(marker);
                _mapToolbar.Visibility = ViewStates.Visible;
                _activeMerchant = _viewModel.Merchants.FirstOrDefault(x => x.MerchantId == marker.Snippet);
                if (!_slidingLayout.IsCollapsed)
                {
                    _slidingLayout.CollapsePane();
                }

                if (marker.IsInfoWindowShown)
                {
                    marker.HideInfoWindow();
                }
                else
                {
                    if (_activeMerchant != null)
                    {
                        Task.Run(async () =>
                        {
                            var image = await GetImageBitmapFromUrl(_activeMerchant.Image);
                            RunOnUiThread(() =>
                            {
                                _infoWindowAdapter.SetImageBitmap(image);
                                marker.ShowInfoWindow();
                            });
                        });
                    }
                    marker.ShowInfoWindow();
                    _activeMarker = marker;
                }
            }
            else
            {
                _slidingLayout.CollapsePane();
            }
            return true;
        }

        public void OnInfoWindowClick(Marker marker)
        {
            if (_gesturesEnabled)
            {

                var item = _viewModel.Merchants.FirstOrDefault(x => x.MerchantId == marker.Snippet);

                OrganisationDetailsFragment fragment = new OrganisationDetailsFragment(item.OrganizationId, true);

                ////GAService.GetGASInstance().Track_App_Event(GAServiceHelper.Events.PartnersDetailClicked, GAServiceHelper.From.FromMap);
                ////GAService.GetGASInstance().Track_App_Page(GAServiceHelper.Page.PartnersDetails);
                HideKeyboard();
                var fragmentTransaction = this.SupportFragmentManager.BeginTransaction();
                fragmentTransaction.SetCustomAnimations(Resource.Animation.slide_in, Resource.Animation.slide_out);
                fragmentTransaction.Add(Resource.Id.main_fragment, fragment, "details").Commit();
            }

            if (!_slidingLayout.IsCollapsed)
            {
                _slidingLayout.CollapsePane();
            }
        }



        public void OnMapClick(LatLng point)
        {
            if (_gesturesEnabled)
            {
                if (_activeMarker != null)
                {
                    RemoveActiveMarker();
                    _mapToolbar.Visibility = ViewStates.Gone;
                }
                HideKeyboard();
            }
            if (!_slidingLayout.IsCollapsed)
            {
                _slidingLayout.CollapsePane();
            }
        }

        #region Implement listeners

        private void ChangeActiveMarker(Marker marker)
        {
            RemoveActiveMarker();
            Drawable activePointer = Resources.GetDrawable(Resource.Drawable.round_map_pointer_active);
            Bitmap activeIcon = DrawableToBitmap(activePointer);
            marker.SetIcon(BitmapDescriptorFactory.FromBitmap(activeIcon));
        }

        private void RemoveActiveMarker()
        {
            try
            {
                _mapToolbar.Visibility = ViewStates.Gone;
                Drawable inactivePointer = Resources.GetDrawable(Resource.Drawable.round_map_pointer);
                Bitmap inactiveIcon = DrawableToBitmap(inactivePointer);
                if (_activeMarker != null)
                {
                    _activeMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(inactiveIcon));
                    _activeMarker = null;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        public void OnConnected(Bundle p0)
        {
            _locRequest = LocationRequest.Create();
            _locRequest.SetPriority(LocationRequest.PriorityBalancedPowerAccuracy);

            LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, _locRequest, this);
        }

        public void OnConnectionSuspended(int cause)
        {
            Toast.MakeText(this, Resource.String.locationSuspended, ToastLength.Short).Show();
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            _viewModel.Latitude = location.Latitude;
            _viewModel.Longitude = location.Longitude;
            try
            {
                if (isActivated)
                {
                    if (!isFiltering)
                    {
                        SetInitialMapLocation(location.Latitude, location.Longitude, false);
                        isActivated = false;
                    }
                    _viewModel.Merchants = _viewModel.GetMerchantsWithDistance(_viewModel.Merchants);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Toast.MakeText(this, Resource.String.locationFailed, ToastLength.Short).Show();
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {

        }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            if (scrollState == ScrollState.Idle && _merchantsListView.FirstVisiblePosition == 0)
            {
                _slidingLayout.CollapsePane();
            }
        }

        public void OnCameraChange(CameraPosition position)
        {
            //_clusterManager.OnCameraChange(position);

            if (_markerClicked && _activeMerchant != null)
            {
                Marker marker = _clusterManager.MarkerCollection.Markers.FirstOrDefault(x => x.Snippet == _activeMerchant.MerchantId);
                if (marker != null)
                {
                    OnMarkerClick(marker);
                }
            }
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (_slidingLayout.IsCollapsed || _slidingLayout.IsAnchored)
            {
                _slidingLayout.ExpandPane();
                return;
            }
            _slidingLayout.CollapsePane();
            var item = _viewModel.Merchants[position];
            _activeMerchant = item;
            if (item.Latitude.HasValue && item.Longitude.HasValue)
            {
                _markerClicked = true;
                ZoomMap(item.Longitude.Value, item.Latitude.Value, true);
            }
        }

        #endregion

        #region methods

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

        private async Task<Bitmap> GetImageBitmapFromUrl(string url)
        {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(url);
                byte[] imageBytes = null;

                try
                {
                    imageBytes = await webClient.DownloadDataTaskAsync(uri);
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, Resource.String.errorImageLoading, ToastLength.Short).Show();
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    return null;
                }

                Bitmap bitmap = await BitmapFactory.DecodeByteArrayAsync(imageBytes, 0, imageBytes.Length);
                return bitmap;
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            SetupMapIfNeeded();
        }


        #endregion
    }

    #region ClusterItem
    public class ClusterItem : Java.Lang.Object, IClusterItem
    {
        public LatLng Position { get; set; }

        public string MerchantId { get; set; }

        public Bitmap Icon { get; set; }

        public string Snippet => "";

        public string Title => string.Empty;

        public ClusterItem(double lat, double lng, string id)
        {
            Position = new LatLng(lat, lng);
            MerchantId = id;
            //			Icon = icon;
        }
    }

 

    public class CustomClusterRenderer : DefaultClusterRenderer
    {
        public CustomClusterRenderer(Android.Content.Context ctx, GoogleMap map, ClusterManager clusterMgr) : base(ctx, map, clusterMgr)
        {

        }


        protected override void OnBeforeClusterItemRendered(Java.Lang.Object obj, MarkerOptions markerOptions)
        {
            base.OnBeforeClusterItemRendered(obj, markerOptions);
            try
            {
                ClusterItem item = obj as ClusterItem;
                markerOptions.SetIcon(MerchantsView._icon);//BitmapDescriptorFactory.FromBitmap (item.Icon));
                markerOptions.SetSnippet(item.MerchantId);
            }
            catch (System.Exception ex)
            {
                //								Debug.wri ();
            }
        }

        protected override bool ShouldRenderAsCluster(ICluster cluster)
        {
            return cluster.Size > 25;
        }
    }
    #endregion
}

