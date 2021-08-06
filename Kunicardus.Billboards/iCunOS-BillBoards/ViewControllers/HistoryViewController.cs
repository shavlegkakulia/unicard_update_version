using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Billboards.Core.Models;
using System.Collections.Generic;
using Kunicardus.Billboards.Core.ViewModels;
using Autofac;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace iCunOS.BillBoards
{
	public class HistoryViewController : BaseViewController
	{
		#region Variables

		private HistoryViewModel _viewModel;

		#endregion

		#region UI

		private UITableView _historyTable;
		private UIRefreshControl _refreshControl;

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			Title = ApplicationStrings.History;
			InitUI ();
		}

		#endregion

		#region Ctor

		public HistoryViewController ()
		{
			using (var scope = App.Container.BeginLifetimeScope ()) {
				_viewModel = scope.Resolve<HistoryViewModel> ();
			}
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			_refreshControl = new UIRefreshControl ();
			_refreshControl.ValueChanged += RefreshHistory;
			_historyTable = new UITableView (new CGRect (0f, 0f, View.Frame.Width, View.Frame.Height - GetStatusBarHeight ()));
			_historyTable.RowHeight = 90f;
			_historyTable.AddSubview (_refreshControl);

			DialogPlugin.ShowProgressDialog ("");
			Task.Run (() => {
				var success = _viewModel.GetAdvertisments ();
				InvokeOnMainThread (() => {
					if (success && _viewModel.Advertisments?.Count > 0) {
						_historyTable.Source = new HistoryTableSource (_viewModel.Advertisments);
					}
					View.AddSubview (_historyTable);
					_historyTable.ReloadData ();
					DialogPlugin.DismissProgressDialog ();
				});
			});
			_historyTable.TableFooterView = new UIView ();
		}

		private void RefreshHistory (object sender, EventArgs e)
		{
			Task.Run (() => {
				var success = _viewModel.GetAdvertisments ();
				InvokeOnMainThread (() => {
					if (success && _viewModel.Advertisments?.Count > 0) {
						_historyTable.Source = new HistoryTableSource (_viewModel.Advertisments);
					}
					_historyTable.ReloadData ();
					_refreshControl.EndRefreshing ();
				});
			});
		}

		#endregion
	}
}

