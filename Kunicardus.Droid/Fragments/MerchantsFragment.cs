using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

using MvvmCross.Binding.BindingContext;
using Kuni.Core.ViewModels;
using Android.Util;
using MvvmCross;
using Kunikardus.Droid.Plugins.SlidingUpPanel;
using Android.Graphics;
using Android.Graphics.Drawables;
using Kunicardus.Droid.Adapters;
using Kuni.Core.Models.DB;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using System.Net;
//using Android.Gms.Maps.Utils.Clustering;
//using Android.Gms.Maps.Utils.Clustering.View;
using Kuni.Core;
using Android.Gms.Common.Apis;
using Android.Gms.Maps;
using Com.Google.Maps.Android.Clustering;
using Android.Gms.Maps.Model;
using Com.Google.Maps.Android.Clustering.View;
using Android.Gms.Common;
using Android.Gms.Location;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;

namespace Kunicardus.Droid.Fragments
{
    public class MerchantsFragment : BaseMvxFragment
                                                   //,IGooglePlayServicesClientConnectionCallbacks, IGooglePlayServicesClientOnConnectionFailedListener
                                                   , GoogleApiClient.IConnectionCallbacks
                                                   , GoogleApiClient.IOnConnectionFailedListener
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
    , IOnMapReadyCallback
                                                   , GoogleMap.IOnCameraChangeListener
    {
        TextView _title;
        AutoCompleteTextView searchText;
        ImageView imgSearch;
        View searchLine;
        bool isSearchBoxActive;
        SlidingUpPanelLayout _slidingLayout;
        FrameLayout _Frame;
        MvxListView _merchantsListView;

        MainView parentActivity;
        Dialog _alertDialog;

        GoogleApiClient _googleApiClient;
        LocationRequest _locRequest;
        GoogleMap _map;
        SupportMapFragment _mapFrag;
        InfoWindowAdapter _infoWindowAdapter;
        Marker _activeMarker;
        MerchantInfo _activeMerchant;
        FrameLayout _mapFrame;
        MerchantsViewModel _viewModel;

        bool isFiltering, updateView, mylocationClicked;
        bool isActivated = false;
        bool _gesturesEnabled = true;

        ImageButton _imgDirection;
        ImageButton _imgLocation;
        RelativeLayout _mapToolbar;

        public event EventHandler MapCleared;

        //		List<Marker> _markers;
        private ClusterManager _clusterManager;

        ProgressDialog _progressDialog;

        DisplayMetrics metrics;
        int? _organizationId;
        bool _markerClicked;

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

        public bool LocationUpdated { get; set; }

        public MerchantsFragment()
        {
            _organizationId = null;
            _viewModel = Mvx.IoCProvider.IoCConstruct<MerchantsViewModel>();
            ViewModel = (MvvmCross.ViewModels.IMvxViewModel)_viewModel;
        }

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
                    if (_googleApiClient != null && _googleApiClient.IsConnected)
                    {

                        LocationServices.FusedLocationApi.RemoveLocationUpdates(
                            _googleApiClient, this);
                        _googleApiClient.Disconnect();
                        if (_map != null)
                            _map.MyLocationEnabled = false;
                    }
                }
            }
        }

        public override void SetMenuVisibility(bool menuVisible)
        {
            base.SetMenuVisibility(menuVisible);

            if (menuVisible)
            {
                isActivated = true;

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
                OrganizationId = parentActivity.OrgId;

                if (OrganizationId.HasValue)
                {
                    isFiltering = true;
                    _progressDialog = ProgressDialog.Show(parentActivity, null, Resources.GetString(Resource.String.loadingMerchants));
                    _viewModel.GetMerchants(OrganizationId.Value);
                }
                else
                {
                    isFiltering = false;
                    if (_viewModel.Merchants == null || _viewModel.ShouldUpdateMap)
                    {
                        _progressDialog = ProgressDialog.Show(parentActivity, null, Resources.GetString(Resource.String.loadingMerchants));
                        _viewModel.GetMerchants();
                    }
                }
            }
            else
            {
                if (isFiltering)
                    CloseSearch();
            }
        }

        public void ClearOrganisationFilter()
        {
            isFiltering = false;
            _organizationId = null;
        }

        public bool CheckOrganisationFilter()
        {
            if (OrganizationId.HasValue && isFiltering)
            {
                return true;
            }
            return false;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.MerchantsListView, null);

            _viewModel = (MerchantsViewModel)this.ViewModel;
            parentActivity = (MainView)this.Activity;

            metrics = new DisplayMetrics();
            parentActivity.WindowManager.DefaultDisplay.GetMetrics(metrics);

            #region Init Views
            _title = view.FindViewById<TextView>(Resource.Id.pageTitle);
            //			Task.Run (async () => {
            view.FindViewById<TextView>(Resource.Id.pageTitle).Text = GetString(Resource.String.merchants);
            searchText = view.FindViewById<AutoCompleteTextView>(Resource.Id.search);
            imgSearch = view.FindViewById<ImageView>(Resource.Id.alert);
            searchLine = view.FindViewById<View>(Resource.Id.view1);
            searchText.SetOnEditorActionListener(this);
            searchText.TextChanged += searchText_TextChanged;

            _imgDirection = view.FindViewById<ImageButton>(Resource.Id.imgDirection);
            _imgLocation = view.FindViewById<ImageButton>(Resource.Id.imgLocation);
            _mapToolbar = view.FindViewById<RelativeLayout>(Resource.Id.mapToolbar);

            _imgDirection.Click += OnDirectionsClick;
            _imgLocation.Click += OnLocationClick;

            _slidingLayout = view.FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_layout);
            _slidingLayout.AnchorPoint = 0.13f;
            _slidingLayout.PanelCollapsed += PanelCollapsed;
            _slidingLayout.PanelExpanded += PanelExpanded;
            //			_slidingLayout.PanelExpandedFromSlide += PanelExpandedFromSlide;

            imgSearch.Click += SearchClick;
            isSearchBoxActive = false;

            _mapFrame = view.FindViewById<FrameLayout>(Resource.Id.mapFrame);
            _merchantsListView = view.FindViewById<MvxListView>(Resource.Id.merchantsList);
            _merchantsListView.SetOnScrollListener(this);
            _merchantsListView.OnItemClickListener = this;

            view.FindViewById<ImageButton>(Resource.Id.menuImg).Click += (o, e) => (parentActivity).ShowMenu();
            #endregion


            var set = this.CreateBindingSet<MerchantsFragment, MerchantsViewModel>();
            set.Bind(this).For(v => v.MerchantsUpdated).To(vmod => vmod.DataPopulated);
            set.Apply();

            InitMapFragment();

            if (_googleApiClient == null)
            {
                _googleApiClient = new GoogleApiClient.Builder(this.Activity)
                                        .AddApi(LocationServices.Api)
                                        .AddConnectionCallbacks(this)
                                        .AddOnConnectionFailedListener(this)
                                        .Build();
                _googleApiClient.Disconnect();
            }
            //			_viewModel.GetMerchants ();
            return view;
        }

        private void SetupMapIfNeeded()
        {
            if (_map == null)
            {
                _mapFrag.GetMapAsync(this);
            }
        }


        public void SetViewPoint(LatLng latlng, bool animated)
        {
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(latlng);
            builder.Zoom(18F);
            CameraPosition cameraPosition = builder.Build();

            if (animated)
            {
                _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
            else
            {
                _map.MoveCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
        }


        private void InitMapFragment()
        {
            _mapFrag = parentActivity.SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
            if (_mapFrag == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);
                var fragTx = parentActivity.SupportFragmentManager.BeginTransaction();
                _mapFrag = SupportMapFragment.NewInstance(mapOptions);
                fragTx.Replace(Resource.Id.mapFrame, _mapFrag);
                fragTx.Commit();
            }
        }
        //Cluster override methods
        public bool OnClusterClick(ICluster cluster)
        {
            Toast.MakeText(parentActivity, cluster.Items.Count + " ობიექტი", ToastLength.Short).Show();
            return false;
        }

        public bool OnClusterItemClick(Java.Lang.Object item)
        {
            _markerClicked = true;
            _activeMerchant = _viewModel.Merchants.FirstOrDefault(x => x.MerchantId == ((ClusterItem)item).MerchantId);
            ZoomMap(((ClusterItem)item).Position.Longitude, ((ClusterItem)item).Position.Latitude, true);
            return true;
        }

        public void OnCameraChange(CameraPosition position)
        {
            //_clusterManager.(position);

            if (_markerClicked && _activeMerchant != null)
            {
                Marker marker = _clusterManager.MarkerCollection.Markers.FirstOrDefault(x => x.Snippet == _activeMerchant.MerchantId);
                if (marker != null)
                {
                    OnMarkerClick(marker);
                }
            }
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

        private void OnMerchantsUpdated()//List<MerchantInfo> merchants)
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
                    //zoom to georgia
                    //	ZoomMap (0, 0, false); 
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
                //				Toast.MakeText (parentActivity, "error occured", ToastLength.Short).Show ();
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        #region ListViewControls

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
                ArrayAdapter<string> adapter = new ArrayAdapter<string>(parentActivity, Android.Resource.Layout.SimpleDropDownItem1Line, suggestions);
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
                imgSearch.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.abc_ic_search_api_material));
                isSearchBoxActive = true;
                isFiltering = true;
            }
            else
            {
                updateView = true;
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
                //_title.Visibility = ViewStates.Visible;
                HideKeyboard();
                return true;
            }
            return false;
        }

        private void HideKeyboard()
        {
            View view = base.Activity.CurrentFocus;
            if (view != null)
            {
                InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);
            }
            searchText.ClearFocus();
        }

        private void ShowKeyboard()
        {
            View view = base.Activity.CurrentFocus;
            if (view != null)
            {
                InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                inputManager.ShowSoftInput(view, ShowFlags.Forced);
            }
        }


        #endregion


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

        private void ShowFilteredMarkers()
        {
            LatLngBounds.Builder builder = new LatLngBounds.Builder();
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


        private void SetInitialMapLocation(double latitude, double longitude, bool isDefault)
        {
            parentActivity.RunOnUiThread(() =>
            {
                if (_map != null)
                {
                    var container_height = _mapFrame.Height;

                    Projection projection = _map.Projection;

                    LatLng location = new LatLng(latitude, longitude);
                    Point markerScreenPosition = projection.ToScreenLocation(location);
                    Point pointHalfScreenAbove = new Point(markerScreenPosition.X, markerScreenPosition.Y - container_height / 3);
                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(location);

                    var zoomLevel = CalculateZoomLevel(isDefault);
                    builder.Zoom(zoomLevel);

                    CameraPosition cameraPosition = builder.Build();
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    _map.AnimateCamera(cameraUpdate, 700, null);

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
                _clusterManager.ClearItems();
                _clusterManager.ClusterMarkerCollection.Clear();
                _clusterManager.MarkerCollection.Clear();

  
                Drawable marker = parentActivity.Resources.GetDrawable(Resource.Drawable.round_map_pointer);
                Bitmap icon = DrawableToBitmap(marker);
                if (_map != null)
                {
                    var count = merchants.Count;

                    List<ClusterItem> items = new List<ClusterItem>();

                    //						for (int i = 0; i < count; i++) {
                    //							var item = merchants [i];
                    //							if (item.Longitude.HasValue && item.Latitude.HasValue) {
                    //								double lat, lng;
                    //								lat = item.Latitude.Value;
                    //								lng = item.Longitude.Value;
                    //								var it = new ClusterItem (lat, lng, item.MerchantId, icon);
                    //								items.Add (it);
                    //							}
                    //						}
                    items = merchants.Where(item => item.Longitude.HasValue && item.Latitude.HasValue)
                        .Select(item => new ClusterItem(item.Latitude.Value, item.Longitude.Value, item.MerchantId, icon)).ToList();
                    _clusterManager.AddItems(items);
                }
                _clusterManager.Cluster();
                _infoWindowAdapter = new InfoWindowAdapter(this, merchants, this.GetLayoutInflater(null));
                _map.SetInfoWindowAdapter(_infoWindowAdapter);
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
                    Toast.MakeText(parentActivity, Resource.String.errorImageLoading, ToastLength.Short).Show();
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    return null;
                }

                Bitmap bitmap = await BitmapFactory.DecodeByteArrayAsync(imageBytes, 0, imageBytes.Length);
                return bitmap;
            }
        }

        #region Map Controls

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
                Toast.MakeText(Activity, Resource.String.error_occured, ToastLength.Long).Show();
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
                Toast.MakeText(Activity, Resource.String.error_occured, ToastLength.Long).Show();
            }
        }

        public bool OnMyLocationButtonClick()
        {
            updateView = false;
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
                            parentActivity.RunOnUiThread(() =>
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

                OrganisationDetailsFragment fragment = new OrganisationDetailsFragment(item.OrganizationId);

                ////GAService.GetGASInstance().Track_App_Event(GAServiceHelper.Events.PartnersDetailClicked, GAServiceHelper.From.FromMap);
                ////GAService.GetGASInstance().Track_App_Page(GAServiceHelper.Page.PartnersDetails);

                var fragmentTransaction = this.Activity.SupportFragmentManager.BeginTransaction();
                fragmentTransaction.SetCustomAnimations(Resource.Animation.slide_in, Resource.Animation.slide_out);
                fragmentTransaction.AddToBackStack("partners_detail_fragment");
                fragmentTransaction.Add(Resource.Id.main_fragment, fragment).Commit();
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
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        #endregion


        public void OnConnected(Bundle p0)
        {
            _locRequest = LocationRequest.Create();
            _locRequest.SetPriority(LocationRequest.PriorityBalancedPowerAccuracy);

            LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, _locRequest, this);
        }

        public void OnConnectionSuspended(int cause)
        {
            Toast.MakeText(parentActivity, Resource.String.locationSuspended, ToastLength.Short).Show();
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            _viewModel.Latitude = location.Latitude;
            _viewModel.Longitude = location.Longitude;
            LocationUpdated = true;
            mylocationClicked = true;
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
            catch (System.Exception ex)
            {

            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Toast.MakeText(parentActivity, Resource.String.locationFailed, ToastLength.Short).Show();
        }

        #region Native Overrides

        public override void OnResume()
        {
            base.OnResume();
            SetupMapIfNeeded();
        }

        public override void OnPause()
        {
            HideKeyboard();

            if (_googleApiClient.IsConnected)
            {
                _googleApiClient.Disconnect();
                if (_map != null)
                    _map.MyLocationEnabled = false;
            }

            base.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnAttach(Activity activity)
        {

            base.OnAttach(activity);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            if (_map != null)
            {
                _clusterManager = new ClusterManager(parentActivity, _map);
                _clusterManager.SetOnClusterItemClickListener(this);
                _clusterManager.SetOnClusterClickListener(this);
                _map.SetOnCameraChangeListener(this);
                _map.SetOnMarkerClickListener(_clusterManager);
                _map.SetOnMapClickListener(this);
                _map.SetOnMyLocationButtonClickListener(this);
                _map.SetOnInfoWindowClickListener(this);
                _map.UiSettings.MapToolbarEnabled = false;

                _clusterManager.Renderer = new CustomClusterRenderer(parentActivity, _map, _clusterManager);
            }
        }


        #endregion
    }

    public class ClusterItem : Java.Lang.Object, IClusterItem
    {
        public LatLng Position { get; set; }

        public string MerchantId { get; set; }

        public Bitmap Icon { get; set; }

        public string Snippet => string.Empty;

        public string Title => string.Empty;

        public ClusterItem(double lat, double lng, string id, Bitmap icon)
        {
            Position = new LatLng(lat, lng);
            MerchantId = id;
            Icon = icon;
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
                markerOptions.InvokeIcon(BitmapDescriptorFactory.FromBitmap(item.Icon));
                markerOptions.SetSnippet(item.MerchantId);
            }
            catch (System.Exception ex)
            {
                //				Debug..WriteLine ();
            }
        }

        protected override bool ShouldRenderAsCluster(ICluster cluster)
        {
            return cluster.Size > 25;
        }
    }
}