using System;
using Android.Widget;
using Android.Content;
using System.Collections.Generic;
using Kuni.Core.Models;
using Android.Views;

namespace Kunicardus.Droid
{
	public class UserDiscountsExpanadableListAdapter : BaseExpandableListAdapter
	{
		#region Private Variables

		private Context _context;
		private List<DiscountModel> _discounts;
		private int _currentProductPrice;

		#endregion

		#region Constructor Implementation

		public UserDiscountsExpanadableListAdapter (Context context, List<DiscountModel> discounts, int currentProductPrice)
		{
			_context = context;
			_discounts = discounts;
			_currentProductPrice = currentProductPrice;
		}

		#endregion

		#region Child Methods

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
			return _discounts.Count;
		}

		public override Android.Views.View GetChildView (int groupPosition, int childPosition, bool isLastChild, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var View = convertView;

			if (View == null) {
				var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				View = inflater.Inflate (Resource.Layout.user_discount_list_item, null);
			}

			var childPercentage = View.FindViewById<TextView> (Resource.Id.percentage_textview);
			childPercentage.Text = _discounts [childPosition].DiscountPercent.ToString ();
			var childText = View.FindViewById<TextView> (Resource.Id.discount_info_textview);
			childText.Text = _discounts [childPosition].DiscountDescription;
			var childDiscountedPrice = View.FindViewById<TextView> (Resource.Id.points_amount_after_discount);
			var discountedVal = (_currentProductPrice - _currentProductPrice * Convert.ToInt32 (_discounts [childPosition].DiscountPercent) / 100.0);
			childDiscountedPrice.Text = Math.Ceiling (discountedVal).ToString ();

			return View;
		}

		#endregion

		#region Group Methods

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return null;
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override Android.Views.View GetGroupView (int groupPosition, bool isExpanded, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var View = convertView;
			if (View == null) {
				var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				View = inflater.Inflate (Resource.Layout.user_discount_group_item, null);
			}
			return View;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			return true;
		}

		public override int GroupCount {
			get {
				return 1;
			}
		}

		public override bool HasStableIds {
			get { return true; }
		}

		#endregion

	}
}

