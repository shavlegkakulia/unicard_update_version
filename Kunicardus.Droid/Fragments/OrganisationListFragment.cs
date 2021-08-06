using System;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross;
using Kuni.Core.ViewModels;
using Kunicardus.Droid.Fragments;
using Android.Views.InputMethods;
using Android.Support.V4.Widget;
using Android.Graphics;
using MvvmCross.Binding.BindingContext;
//using MvvmCross.Binding.Droid.Views;
using Kuni.Core;
using MvvmCross.Platforms.Android.Binding.Views;

namespace Kunicardus.Droid
{
	public class OrganisationListFragment : BaseMvxFragment, TextView.IOnEditorActionListener, AdapterView.IOnItemClickListener
	{

		MvxListView _partnersListView;
		OrganisationListViewModel _Viewmodel;

		public bool DataPopulated {
			set {
				if (value) {
					_refresher.Refreshing = false;
					if (searchText.Adapter == null) {

						ArrayAdapter<string> adapter = new ArrayAdapter<string> (a, Android.Resource.Layout.SimpleDropDownItem1Line, _Viewmodel.OrganisationNames);
						searchText.Adapter = adapter;
					} else {
						((ArrayAdapter<string>)searchText.Adapter).Clear ();
						((ArrayAdapter<string>)searchText.Adapter).AddAll (_Viewmodel.OrganisationNames);

						((ArrayAdapter<string>)searchText.Adapter).NotifyDataSetChanged ();
					}

				}
			}
			get { return false; }
		}

		#region UI

		MainView _mainView;
		ImageButton _menu;

		// Toolbar and search UI controls -----
		TextView _title;
		AutoCompleteTextView searchText;
		ImageView imgSearch;
		View searchLine;
		bool isSearchBoxActive;
		// -----------------------------------

		#endregion

		#region Variables

		SwipeRefreshLayout _refresher;

		#endregion

		#region Constructor implementation

		public OrganisationListFragment ()
		{
			ViewModel = Mvx.IoCProvider.IoCConstruct<OrganisationListViewModel>();
			_Viewmodel = (OrganisationListViewModel)ViewModel;
		}

		#endregion

		#region Overrides

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);					
			var View = this.BindingInflate (Resource.Layout.PartnersListLayout, null);
			_mainView = ((MainView)base.Activity);
			_menu = View.FindViewById<ImageButton> (Resource.Id.menuImg);
			_menu.Click += (o, e) => _mainView.ShowMenu ();

			// Searchbar Initialisation
			_title = View.FindViewById<TextView> (Resource.Id.pageTitle);
			_title.Text = GetString (Resource.String.partners);
			searchText = View.FindViewById<AutoCompleteTextView> (Resource.Id.search);
			imgSearch = View.FindViewById<ImageView> (Resource.Id.alert);
			searchLine = View.FindViewById<View> (Resource.Id.view1);
			searchText.SetOnEditorActionListener (this);		
			imgSearch.Click += SearchClick;
			isSearchBoxActive = false;
			// ------

			_partnersListView = View.FindViewById<MvxListView> (Resource.Id.partnersListView);
			_partnersListView.OnItemClickListener = this;

			_refresher = View.FindViewById<SwipeRefreshLayout> (Resource.Id.refresher);
			_refresher.SetBackgroundColor (Color.ParseColor ("#e2e3e3"));
			_refresher.Refresh += OnRefresh;
			_refresher.SetProgressViewOffset (false, 0, (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, 24, Resources.DisplayMetrics));
			_refresher.Refreshing = true;

			var set = this.CreateBindingSet<OrganisationListFragment, OrganisationListViewModel> ();
			set.Bind (this).For (v => v.DataPopulated).To (vmod => vmod.DataPopulated);
			set.Apply (); 

			return View;
		}

		#endregion

		void OnRefresh (object sender, EventArgs e)
		{
			_Viewmodel.Filter (searchText.Text, true);
		}

		#region Events

		public bool OnEditorAction (TextView v, Android.Views.InputMethods.ImeAction actionId, KeyEvent e)
		{
			if (actionId == ImeAction.Search) {
				_Viewmodel.Filter (searchText.Text, false);
				HideKeyboard ();
				return true;
			}
			return false;
		}

		#region implemented abstract members of BaseMvxFragment

		public override void OnActivate ()
		{
			_refresher.Post (delegate {
				_refresher.Refreshing = true;
			});
			searchText.Text = "";
			_Viewmodel.Filter (searchText.Text, false);
		}

		#endregion

		void SearchClick (object sender, EventArgs e)
		{
			if (!isSearchBoxActive) {
				_title.Visibility = ViewStates.Invisible;
				searchLine.Visibility = ViewStates.Visible;
				searchText.Visibility = ViewStates.Visible;
				searchText.RequestFocus ();
				ShowKeyboard ();
				imgSearch.SetImageDrawable (Resources.GetDrawable (Resource.Drawable.abc_ic_clear_material));
				isSearchBoxActive = true;
			} else {
				_title.Visibility = ViewStates.Visible;
				searchLine.Visibility = ViewStates.Gone;
				searchText.Visibility = ViewStates.Gone;
				searchText.Text = "";
				isSearchBoxActive = false;
				imgSearch.SetImageDrawable (Resources.GetDrawable (Resource.Drawable.search));
				HideKeyboard ();

				_Viewmodel.Filter ("", false);
			}
		}

		#endregion

		#region Methods

		private MainView a;

		public override void OnAttach (Android.App.Activity activity)
		{
			a = (MainView)activity;
			base.OnAttach (activity);
		}

		private void HideKeyboard ()
		{
			View View = base.Activity.CurrentFocus;
			if (View != null) {
				InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService (Context.InputMethodService);
				inputManager.HideSoftInputFromWindow (View.WindowToken, HideSoftInputFlags.None);
			}
			searchText.ClearFocus ();
		}

		private void ShowKeyboard ()
		{
			View View = base.Activity.CurrentFocus;
			if (View != null) {
				InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService (Context.InputMethodService);
				inputManager.ShowSoftInput (View, ShowFlags.Forced);

			}
		}

		#endregion

		public void OnItemClick (AdapterView parent, View View, int position, long id)
		{
			OrganisationDetailsFragment fragment = new OrganisationDetailsFragment (_Viewmodel.GetOrgId (position));


			////GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.PartnersDetailClicked, GAServiceHelper.From.FromPartnersList);
			////GAService.GetGASInstance ().Track_App_Page (GAServiceHelper.Page.PartnersDetails);

			var fragmentTransaction = a.SupportFragmentManager.BeginTransaction ();
			fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
			fragmentTransaction.AddToBackStack ("partners_detail_fragment");
			fragmentTransaction.Add (Resource.Id.main_fragment, fragment, "partners_detail_fragment").Commit ();
		}
	}
}

