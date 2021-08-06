using System;
using System.Collections.Generic;
using Kunicardus.Billboards.Core.DbModels;
using Foundation;
using UIKit;
using Kunicardus.Billboards.Core.Models;

namespace iCunOS.BillBoards
{
	public class MainPageAdsTableSource : UITableViewSource
	{
		#region Variables

		private List<AdsModel> _source;
		NSString _cellIdentifier = new NSString ("HistoryCell");

		#endregion

		#region Constructor

		public MainPageAdsTableSource (List<AdsModel> source)
		{
			_source = source;
		}

		#endregion

		#region Overrides

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return _source.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			MainPageAdTableCell cell = (MainPageAdTableCell)tableView.DequeueReusableCell (_cellIdentifier);
			AdsModel item = _source [indexPath.Row];

			if (cell == null) {
				cell = new MainPageAdTableCell (_cellIdentifier);
			}
			cell.UpdateCell (item.MerchantName, item.PassDate, item.MerchantLogo);

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var item = _source [indexPath.Row];

			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			var controller = new UINavigationController ();
			controller.PushViewController (new AdsViewController (
				UIPageViewControllerTransitionStyle.Scroll,
				UIPageViewControllerNavigationOrientation.Horizontal,
				UIPageViewControllerSpineLocation.Min, item.HistoryId), false);
			controller.NavigationBar.BarStyle = UIBarStyle.Black;
			controller.NavigationBarHidden = false;
			app.SidebarController.ChangeContentView (controller);
		}

		#endregion
	}
}

