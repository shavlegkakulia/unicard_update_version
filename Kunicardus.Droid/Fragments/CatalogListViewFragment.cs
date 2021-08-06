using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Kuni.Core.ViewModels;
using Kuni.Core;
using Android.Support.V4.Widget;
//using MvvmCross.Binding.Droid.Views;
using Kunicardus.Droid.Adapters;
using Android.Graphics;
using MvvmCross.Binding.BindingContext;
using System.Threading.Tasks;
using Kunicardus.Droid.Plugins.Connectivity;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
//using MvvmCross.Platforms.Android.Binding.Views;

namespace Kunicardus.Droid.Fragments
{
	public class CatalogListViewFragment : BaseMvxFragment, Android.Widget.AdapterView.IOnItemClickListener,
	Android.Widget.AbsListView.IOnScrollListener
	{
		#region Private Variables

		//		private MainView _activity;
		private SwipeRefreshLayout _refresher;
		private DrawerLayout _dLayout;
		public LinearLayout _drawerListViewLayout, _removeFilter;
		private ImageButton catalogFilter;
		private ProgressBar _loadingMore;
		private ExpandableListView _expandableListView;
		private CatalogListViewModel _currentViewModel;
		private MainView _mainView;
		public static MvxGridView _maingrid;

		public ExpandableListAdapter FilterAdapter{ get; set; }

		CatalogListAdapter _mainAdapter;

		#endregion

		#region OncreateView

		ProgressDialog _dialog;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.CatalogListView, null);

			View.FindViewById<View> (Resource.Id.transparentV).Touch += (o, e) => {
				e.Handled = true;
			};
			_currentViewModel = this.ViewModel as CatalogListViewModel;
			_mainView = base.Activity as MainView;
			_maingrid = View.FindViewById<MvxGridView> (Resource.Id.catalog_gridview);
			_mainAdapter = new CatalogListAdapter (this.Activity, (MvvmCross.Platforms.Android.Binding.BindingContext.IMvxAndroidBindingContext)BindingContext);
			_maingrid.Adapter = _mainAdapter;
			_maingrid.OnItemClickListener = this;
			_maingrid.SetOnScrollListener (this);
			_expandableListView = this.Activity.FindViewById<ExpandableListView> (Resource.Id.right_drawer);

			_refresher = View.FindViewById<SwipeRefreshLayout> (Resource.Id.refresher);
			_refresher.SetBackgroundColor (Color.ParseColor ("#e2e3e3"));
			_refresher.Refresh += OnRefresh;

			_loadingMore = View.FindViewById<ProgressBar> (Resource.Id.loadingMore);
			_loadingMore.Visibility = ViewStates.Gone;
			var _menu = View.FindViewById<ImageButton> (Resource.Id.catalog_list_toolbar_menu);
			_menu.Click += (o, e) => _mainView.ShowMenu ();

			#region drawer logic
			catalogFilter = View.FindViewById<ImageButton> (Resource.Id.catalog_list_filter);

			_dLayout = Activity.FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			_drawerListViewLayout = Activity.FindViewById<LinearLayout> (Resource.Id.right_drawer_layout);
			catalogFilter.Click += (sender, e) => _dLayout.OpenDrawer (_drawerListViewLayout);
			#endregion

			#region filter logic
			_removeFilter = View.FindViewById<LinearLayout> (Resource.Id.remove_filter);
			_removeFilter.Visibility = ViewStates.Invisible;
			_removeFilter.Click += (sender, e) => RemoveFilter (true);
			#endregion

			_dialog = new ProgressDialog (this.Activity);

			var set = this.CreateBindingSet<CatalogListViewFragment, CatalogListViewModel> ();
			set.Bind (this).For (v => v.DataSetUpdated).To (vmod => vmod.DataSetUpdated);
			set.Apply (); 

			return View;
		}

		private bool _dataSetUpdated;

		public bool DataSetUpdated {
			get {
				return _dataSetUpdated;
			}
			set {
				if (value) {
					_maingrid.SmoothScrollToPosition (0);
				}
				_loadingMore.Visibility = ViewStates.Gone;
				_refresher.Refreshing = false;
				if (!string.IsNullOrEmpty (_currentViewModel.DisplayText)) {
					Toast.MakeText (_mainView, _currentViewModel.DisplayText, ToastLength.Short).Show ();
					return;
				}
				_mainAdapter.NotifyDataSetChanged ();
				_dataSetUpdated = value;
			}
		}

		private void RemoveFilter (bool showDialog)
		{
			ToggleRemoveFilterLayout (false);
			ChangeFilterIcon (false);
			var searchEditText = Activity.FindViewById<EditText> (Resource.Id.drawer_search_edittext);
			if (!string.IsNullOrWhiteSpace (searchEditText.Text))
				searchEditText.Text = String.Empty;
			_dLayout = Activity.FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			_drawerListViewLayout = Activity.FindViewById<LinearLayout> (Resource.Id.right_drawer_layout);

			//change product list
			_currentViewModel.RemoveFilter (showDialog);

			if (FilterAdapter != null) {
				if (FilterAdapter.SelectedGroupID > 1)
					_expandableListView.CollapseGroup (FilterAdapter.SelectedGroupID);
				FilterAdapter.SelectedChildID
				= FilterAdapter.SelectedGroupID = -1;
				FilterAdapter.NotifyDataSetInvalidated ();

			}
			_mainView.RunOnUiThread (() => _dLayout.CloseDrawer (_drawerListViewLayout));
		}

		public void ToggleRemoveFilterLayout (bool layoutShoundBeVisible)
		{
			if (layoutShoundBeVisible)
				_removeFilter.Visibility = ViewStates.Visible;
			else
				_removeFilter.Visibility = ViewStates.Invisible;
				
		}

		public void ChangeFilterIcon (bool filtered)
		{
			if (filtered) {
				catalogFilter.SetImageResource (Resource.Drawable.filter_checked);
			} else {
				catalogFilter.SetImageResource (Resource.Drawable.filter);
			}
		}

		public override void OnResume ()
		{
			_maingrid.Clickable = true;
			base.OnResume ();
		}



		public override void OnActivate ()
		{
			RemoveFilter (false);
			_currentViewModel.GetOrFilterProductList (true);
		}

		public void CloseDrawer ()
		{
			_dLayout.CloseDrawer (_drawerListViewLayout);
		}

		public void OnItemClick (AdapterView parent, View View, int position, long id)
		{
			_mainView._dialog.Show ();
			Task.Run (async () => {
				var netwService = new DroidConnectivityProviderPlugin ();
				if (!(netwService.IsWifiReachable || netwService.IsNetworkReachable)) {
					this.Activity.RunOnUiThread (() => {
						Toast.MakeText (Activity, Resource.String.no_network, ToastLength.Short).Show ();
						_mainView._dialog.Dismiss ();
					});
					return;
				}
				var pid = _currentViewModel.Products [position].ProductID;
				CatalogDetailFragment _catalogDetailFragment = new CatalogDetailFragment (pid);

				////GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.CatalogClicked, GAServiceHelper.From.FromCatalogList);
				////GAService.GetGASInstance ().Track_App_Page (GAServiceHelper.Page.CatalogDetail); 

				var fragmentTransaction = this.Activity.SupportFragmentManager.BeginTransaction ();
				fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
				fragmentTransaction.AddToBackStack (null);//"catalog_details");
				fragmentTransaction.Replace (Resource.Id.main_fragment, _catalogDetailFragment).Commit ();
			});
		}

		#endregion

		void OnRefresh (object sender, EventArgs e)
		{
			_currentViewModel.GetOrFilterProductList (false);
		}

		private readonly object Lock = new object ();

		public void OnScroll (AbsListView View, int firstVisibleItem, int visibleItemCount, int totalItemCount)
		{
			lock (this.Lock) {
				//Todo add loading and check total count
				var loadMore = firstVisibleItem + visibleItemCount >= _currentViewModel.rowCount * _currentViewModel.PageIndex;
				if (loadMore && this.ViewModel != null) {
					_loadingMore.Visibility = ViewStates.Visible;
					_currentViewModel.PageIndex++;
					_currentViewModel.LoadMore ();
				}
			}
		}

		public void OnScrollStateChanged (AbsListView View, ScrollState scrollState)
		{

		}
	}
}