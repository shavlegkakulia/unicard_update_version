using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross;
using Kuni.Core.ViewModels;
using Kuni.Core;
using Android.Support.V4.Widget;
using Android.Support.V4.App;
using Android.Views.InputMethods;
using MvvmCross.Binding.BindingContext;

namespace Kunicardus.Droid.Fragments
{
	public class BaseCatalogFragment : BaseMvxFragment, ExpandableListView.IOnChildClickListener
	{

		#region ExpandableListView ChildClick event

		Kunicardus.Droid.BaseEditText _searchText;

		public bool OnChildClick (ExpandableListView parent, View clickedView, int groupPosition, int childPosition, long id)
		{
			_catalogListViewFragment.ChangeFilterIcon (true);

			_parentActivity.RunOnUiThread (() => {
				_dLayout.CloseDrawer (_drawerListViewLayout);
			});
			_expandableListAdapter.SelectedChildID = childPosition;
			_expandableListAdapter.SelectedGroupID = groupPosition;

			switch (groupPosition) {
			//filter by product category
			case 2:
				var categoryId = _currentViewModel.DataToSendForFilter.ProductsCategory [childPosition].CategoryID;
				(_catalogListViewFragment.ViewModel as CatalogListViewModel).AssignFilterInfo (Convert.ToInt32 (categoryId), null, null, null, null, null);
				break;
			//filter by customer type
			case 3:
				var customerTypeId = _currentViewModel.DataToSendForFilter.UsersTypeCategory [childPosition].UserTypeID;
				(_catalogListViewFragment.ViewModel as CatalogListViewModel).AssignFilterInfo (null, customerTypeId, null, null, null, null);
				break;
			//filter by points
			case 4:
				var rangeId = _currentViewModel.PointsCategoryList [childPosition].ID;
				(_catalogListViewFragment.ViewModel as CatalogListViewModel).AssignFilterInfo (null, null, null, rangeId, null, null);
				break;
			}

			(_catalogListViewFragment.ViewModel as CatalogListViewModel).GetOrFilterProductList (true);
			_expandableListAdapter.NotifyDataSetInvalidated ();
			_catalogListViewFragment.ToggleRemoveFilterLayout (true);
			return true;
		}

		#endregion

		#region Constructor Implementation

		public BaseCatalogFragment ()
		{
			if (this.ViewModel == null)
				this.ViewModel = Mvx.IoCProvider.IoCConstruct<BaseCatalogViewModel>();
		}

		#endregion

		#region implemented abstract members of BaseMvxFragment

		public override void OnActivate ()
		{
			_catalogListViewFragment.OnActivate ();
		}

		#endregion

		#region Private Variables

		private BaseCatalogViewModel.FilterDataModel _filterData;
		private ExpandableListView _filterListView;
		private BaseCatalogViewModel _currentViewModel;
		private CatalogListViewFragment _catalogListViewFragment;
		private DrawerLayout _dLayout;
		private LinearLayout _drawerListViewLayout;
		private ExpandableListAdapter _expandableListAdapter;
		private List<string> _groupItems;
		private List<LinearLayout> _drawerItems;
		private MainView _parentActivity;

		#endregion

		#region Fragmanet Native Methods

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.BaseCatalogView, null);
			_groupItems = new List<string> () { 
				"ფასდაკლებულები",
				"ბოლოს დამატებულები",
				"კატეგორიები",
				//"მომხმარებლის ტიპი",
				"ქულების შუალედები"
			};
			_drawerItems = new List<LinearLayout> ();
			_currentViewModel = (this.ViewModel as BaseCatalogViewModel);
			_filterListView = View.FindViewById<ExpandableListView> (Resource.Id.right_drawer);

			_catalogListViewFragment = new CatalogListViewFragment ();
			if (_catalogListViewFragment.ViewModel == null)
				_catalogListViewFragment.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCConstruct<CatalogListViewModel>();
			var searchEditText = View.FindViewById<EditText> (Resource.Id.drawer_search_edittext);
			searchEditText.FocusChange += (sender, e) => {
				if (!e.HasFocus) {
					HideKeyBoard ();
				}
			};
			var searchButton = View.FindViewById<ImageButton> (Resource.Id.drawer_search);
			searchButton.Click += SearchClicked;
			var fragmentManger = (_parentActivity as MainView).SupportFragmentManager;
			fragmentManger.BeginTransaction ().Add (Resource.Id.content_frame, _catalogListViewFragment).Commit ();

			var closeDrawerIcon = View.FindViewById<ImageButton> (Resource.Id.close_drawer);

			_dLayout = View.FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			_drawerListViewLayout = View.FindViewById<LinearLayout> (Resource.Id.right_drawer_layout);

			closeDrawerIcon.Click += (sender, e) => {
				_parentActivity.RunOnUiThread (() => {
					_dLayout.CloseDrawer (_drawerListViewLayout);
				});
			};
		
			_dLayout.DrawerClosed += (sender, e) => {
				HideKeyBoard ();
			};
			_searchText = View.FindViewById<BaseEditText> (Resource.Id.drawer_search_edittext);
			_searchText.EditorAction += HandleEditorAction;

			var set = this.CreateBindingSet<BaseCatalogFragment, BaseCatalogViewModel> ();
			set.Bind (this).For (v => v.FilterData).To (vmod => vmod.FilterDataPopulated);
			set.Apply (); 

			return View;
		}


		void SearchClicked (object sender, EventArgs e)
		{
			var searchtext = (this.ViewModel as BaseCatalogViewModel).SearchText;
			if (!string.IsNullOrEmpty (searchtext)) {
				SearchData (searchtext);
			}
		}

		private void HandleEditorAction (object sender, TextView.EditorActionEventArgs e)
		{
			e.Handled = false;
			if (e.ActionId == ImeAction.Done) {
				var searchtext = (this.ViewModel as BaseCatalogViewModel).SearchText;
				if (!string.IsNullOrEmpty (searchtext)) {
					SearchData (searchtext);
				}
				e.Handled = true;   
			}
		}

		string oldsearchterm = string.Empty;

		private void SearchData (string searchtext)
		{
//				oldsearchterm = searchtext;
			(_catalogListViewFragment.ViewModel as CatalogListViewModel).SearchByName (searchtext);
			_catalogListViewFragment.ChangeFilterIcon (true);
			_catalogListViewFragment.ToggleRemoveFilterLayout (true);
			_dLayout.CloseDrawer (_drawerListViewLayout);	
		}

		#endregion

		public Kuni.Core.BaseCatalogViewModel.FilterDataModel FilterData {
			set {
				try {
					_filterData = value;
					_expandableListAdapter = new ExpandableListAdapter (
						_catalogListViewFragment,
						_parentActivity,
						_filterData.ProductsCategory,
						_filterData.UsersTypeCategory,
						_filterData.PointsCategory,
						_groupItems,
						_drawerItems
					);
					_filterListView.SetAdapter (_expandableListAdapter);
					_filterListView.SetOnChildClickListener (this);
					_catalogListViewFragment.FilterAdapter = _expandableListAdapter;

				} catch (NullReferenceException ex) {

				} catch (Exception ex) {
				}
				_filterData = value;
			}
			get{ return _filterData; }
		}


		#region Receiving Messages

		public override void OnAttach (Activity activity)
		{
			if (activity != null) {
				_parentActivity = ((MainView)activity);
			}
			base.OnAttach (activity);
		}

		#endregion

		#region Methods

		private void HideKeyBoard ()
		{
			View currentView = base.Activity.CurrentFocus;
			if (currentView != null) {
				InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService (Context.InputMethodService);
				inputManager.HideSoftInputFromWindow (currentView.WindowToken, HideSoftInputFlags.NotAlways);
			}
		}

		#endregion
	}
}