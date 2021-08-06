using Android.Views;
using Android.Content;
using Android.Widget;
using System.Collections.Generic;
using Kuni.Core.Models;
using Kuni.Core;
using Kunicardus.Droid.Fragments;
using Kuni.Core.ViewModels;

namespace Kunicardus.Droid
{
	public class ExpandableListAdapter : BaseExpandableListAdapter
	{
		#region Private Variables & Properties

		private readonly Context _context;
		List<CategoryModel> _categoryType;
		List<UserTypeModel> _usersType;
		List<PriceRange> _pointsType;
		List<string> _groupHeaders;
		List<LinearLayout> _drawerItems;
		CatalogListViewFragment _catalogListViewFragment;

		public int SelectedChildID  { get; set; }

		public int SelectedGroupID  { get; set; }

		#endregion

		#region Constructor Implementation

		public ExpandableListAdapter (CatalogListViewFragment catalogListViewFragment,
		                              Context context, 
		                              List<CategoryModel> categoryType,
		                              List<UserTypeModel> usersType,
		                              List<PriceRange> pointsType,
		                              List<string> groupHeaders,
		                              List<LinearLayout> drawerItems)
		{
			_catalogListViewFragment = catalogListViewFragment;
			_context = context;
			_drawerItems = drawerItems;
			_categoryType = categoryType;
			_usersType = usersType;
			_pointsType = pointsType;
			_groupHeaders = groupHeaders;

			SelectedChildID = SelectedGroupID = -1;
		}

		#endregion

		#region Methods

		private void FilterLastModified (int groupPosition)
		{
			if (groupPosition == 1) {
				(_catalogListViewFragment.ViewModel as CatalogListViewModel).AssignFilterInfo (null, null, 1, null, null, string.Empty);
				(_catalogListViewFragment.ViewModel as CatalogListViewModel).GetOrFilterProductList (true);
				_catalogListViewFragment.ToggleRemoveFilterLayout (true);
				_catalogListViewFragment.CloseDrawer ();
			}
		}

		private void FilterBySale (int groupPosition)
		{
			if (groupPosition == 0) {
				(_catalogListViewFragment.ViewModel as CatalogListViewModel).AssignFilterInfo (null, null, null, null, 1, string.Empty);
				(_catalogListViewFragment.ViewModel as CatalogListViewModel).FilterBySale ();
				_catalogListViewFragment.ToggleRemoveFilterLayout (true);
				_catalogListViewFragment.CloseDrawer ();
			}
		}

		#endregion

		#region Child Native Methods

		public override void OnGroupExpanded (int groupPosition)
		{
			if (groupPosition == 1) {
				_catalogListViewFragment.ToggleRemoveFilterLayout (true);
				_catalogListViewFragment.ChangeFilterIcon (true);
				FilterLastModified (groupPosition);
				this.NotifyDataSetInvalidated ();
			} else if (groupPosition == 0) {
				_catalogListViewFragment.ToggleRemoveFilterLayout (true);
				_catalogListViewFragment.ChangeFilterIcon (true);
				FilterBySale (groupPosition);
				this.NotifyDataSetInvalidated ();
			}
			if (groupPosition == 0 || groupPosition == 1) {
				SelectedChildID = -1;
				SelectedGroupID = groupPosition;
			}
			base.OnGroupExpanded (groupPosition);
		}

		public override void OnGroupCollapsed (int groupPosition)
		{
			if (groupPosition == 1) {
				_catalogListViewFragment.ChangeFilterIcon (true);
				_catalogListViewFragment.ToggleRemoveFilterLayout (true);
				FilterLastModified (groupPosition);
				this.NotifyDataSetInvalidated ();
			} else if (groupPosition == 0) {
				_catalogListViewFragment.ChangeFilterIcon (true);
				_catalogListViewFragment.ToggleRemoveFilterLayout (true);
				FilterBySale (groupPosition);
				this.NotifyDataSetInvalidated ();
			}
			if (groupPosition == 0 || groupPosition == 1) {
				SelectedChildID = -1;
				SelectedGroupID = groupPosition;
			}
			base.OnGroupCollapsed (groupPosition);
		}

		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			return null;
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override int GetChildrenCount (int groupPosition)
		{
			int childrenCount = 0;
			switch (groupPosition) {
			case 2:
				childrenCount = _categoryType.Count;
				break;
			//case 3:
				//childrenCount = _usersType.Count;
				//break;
			case 3:
				if (_pointsType != null) {
					childrenCount = _pointsType.Count;
				}
				break;
			}
			return childrenCount;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			var View = convertView;

			if (View == null) {
				var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				View = inflater.Inflate (Resource.Layout.drawer_item, null);
			}

			var child = View.FindViewById<TextView> (Resource.Id.drawer_item);
			if (groupPosition == 2) {
				if (childPosition < _categoryType.Count)
					child.Text = _categoryType [childPosition].CategoryName;
			}
			//if (groupPosition == 3) {
			//	if (childPosition < _usersType.Count)
			//		child.Text = _usersType [childPosition].UserTypeName;
			//} 
			if (groupPosition == 3) {
				if (childPosition < _pointsType.Count) {
					child.Text = _pointsType [childPosition].Name;
				}
			}
			if (SelectedGroupID == groupPosition && SelectedChildID == childPosition) {
				View.FindViewById<BaseTextView> (Resource.Id.drawer_item).SetBackgroundResource (Resource.Drawable.catalog_filter_orange_button_background);	
			} else {
				View.FindViewById<BaseTextView> (Resource.Id.drawer_item).SetBackgroundResource (Resource.Drawable.catalog_filter_item_background);
			}
			return View;
		}

		#endregion

		#region Group Native Methods

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return null;
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			var View = convertView;

			var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
			if (groupPosition <= 1) {
				View = inflater.Inflate (Resource.Layout.drawer_item, null);
			} else {
				View = inflater.Inflate (Resource.Layout.drawer_group_item, null);
			}

			TextView parentView;
			if (groupPosition <= 1) {
				parentView = View.FindViewById<BaseTextView> (Resource.Id.drawer_item);
				parentView.Text = _groupHeaders [groupPosition];
				if (SelectedGroupID == groupPosition && SelectedChildID == -1) {
					parentView.FindViewById<BaseTextView> (Resource.Id.drawer_item).SetBackgroundResource (Resource.Drawable.catalog_filter_orange_button_background);	
				} else {
					parentView.FindViewById<BaseTextView> (Resource.Id.drawer_item).SetBackgroundResource (Resource.Drawable.catalog_filter_item_background);
				}
			} else {
				parentView = View.FindViewById<BaseTextView> (Resource.Id.drawer_group_item);
				parentView.Text = _groupHeaders [groupPosition];
			}


			return View;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			return true;
		}

		public override int GroupCount {
			get { return  _groupHeaders.Count; }
		}

		public override bool HasStableIds {
			get { return true; }
		}

		#endregion
	}
}