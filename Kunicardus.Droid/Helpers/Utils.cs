using System;
using Android.Widget;
using Android.Views;

namespace Kunicardus.Droid
{
	public class Utils
	{
		public static void SetListViewHeightBasedOnChildren (ListView listView)
		{
			IListAdapter listAdapter = listView.Adapter;
			if (listAdapter == null) {
				// pre-condition
				return;
			}

			int totalHeight = listView.PaddingBottom + listView.PaddingTop;
			int desiredWidth = Android.Views.View.MeasureSpec.MakeMeasureSpec (listView.Width, MeasureSpecMode.AtMost);


			for (int i = 0; i < listAdapter.Count; i++) {
				View listItem = listAdapter.GetView (i, null, listView);
				listItem.LayoutParameters = new Android.Views.ViewGroup.LayoutParams (
					ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
				listItem.Measure (desiredWidth, (int)MeasureSpecMode.Unspecified);

				totalHeight += (listItem.MeasuredHeight + 45);
			}

			ViewGroup.LayoutParams params_ = listView.LayoutParameters;
			params_.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
			listView.LayoutParameters = params_;
			listView.RequestLayout ();
		}

		public static void SetSimpleListViewHeight (ListView listView)
		{
			IListAdapter listAdapter = listView.Adapter;
			if (listAdapter == null) {
				// pre-condition
				return;
			}

			int totalHeight = listView.PaddingBottom + listView.PaddingTop;
			int desiredWidth = Android.Views.View.MeasureSpec.MakeMeasureSpec (listView.Width, MeasureSpecMode.AtMost);


			for (int i = 0; i < listAdapter.Count; i++) {
				View listItem = listAdapter.GetView (i, null, listView);
				listItem.LayoutParameters = new Android.Views.ViewGroup.LayoutParams (
					ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
				listItem.Measure (desiredWidth, (int)MeasureSpecMode.Unspecified);

				totalHeight += (listItem.MeasuredHeight);
			}

			ViewGroup.LayoutParams params_ = listView.LayoutParameters;
			params_.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
			listView.LayoutParameters = params_;
			listView.RequestLayout ();
		}

	}
}

