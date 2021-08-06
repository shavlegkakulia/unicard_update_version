using System;
using UIKit;
using System.Collections.Generic;
using Kunicardus.Billboards.Core.Models;
using Foundation;
using MediaPlayer;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core;

namespace iCunOS.BillBoards
{
	public class HistoryTableSource : UITableViewSource
	{
		#region Variables

		private List<HistoryModel> _source;
		NSString _cellIdentifier = new NSString ("HistoryCell");

		#endregion

		#region Constructor

		public HistoryTableSource (List<HistoryModel> source)
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
			HistoryTableCell cell = (HistoryTableCell)tableView.DequeueReusableCell (_cellIdentifier);
			HistoryModel item = _source [indexPath.Row];

			if (cell == null) {
				cell = new HistoryTableCell (_cellIdentifier);
			}
			var hours = String.Format ("{0}:{1}", 14, 32);
			var days = String.Format ("{0} {1}", 14, "ივნისი");

			cell.UpdateCell (item.OrgName, days, hours, item.ImageUrl);

			return cell;
		}

		#endregion
	}
}
