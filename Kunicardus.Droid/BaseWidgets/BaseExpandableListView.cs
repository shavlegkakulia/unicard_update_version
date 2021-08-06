using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Runtime;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.BaseExpandableListView")]
	public class BaseExpandableListView: ExpandableListView
	{
		bool expanded = true;

		public BaseExpandableListView (Context context) : base (context)
		{
		}

		public BaseExpandableListView (Context context, IAttributeSet attrs) : base (context, attrs)
		{
		}

		public BaseExpandableListView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
		}

		public bool isExpanded ()
		{
			return expanded;
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			if (isExpanded ()) {
				int expandSpec = 0;
				expandSpec = MeasureSpec.MakeMeasureSpec (int.MaxValue, MeasureSpecMode.AtMost);
				if (expandSpec == -1 || expandSpec == int.MaxValue)
					expandSpec = MeasureSpec.MakeMeasureSpec (int.MaxValue, MeasureSpecMode.Exactly);
				base.OnMeasure (widthMeasureSpec, expandSpec);
				ViewGroup.LayoutParams parameters = LayoutParameters;
				parameters.Height = MeasuredHeight;
			} else {
				base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			}
		}

		public void setExpanded (bool expanded)
		{
			this.expanded = expanded;
		}
	}
}

