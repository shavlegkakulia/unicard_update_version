using System;
using UIKit;
using Foundation;
using Kunicardus.Core.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreGraphics;
using System.Xml;
using MonoTouch.Dialog;
using Accelerate;

namespace Kunicardus.Touch
{
	public class NewsListViewController : MvxTableViewController
	{
		#region Properties

		public new NewsListViewModel ViewModel {
			get { return (NewsListViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public bool DataPopulated {
			get{ return true; }
			set {
				RefreshControl.EndRefreshing ();
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				TableView.ReloadData ();	
			}
		}

		#endregion

		#region Overrides

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.ViewModel.GetNewsList ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			Title = ApplicationStrings.News;
			TableView = new UITableView (new CGRect (0, 0, View.Frame.Width, View.Frame.Height));
			var source = new MvxSimpleTableViewSource (TableView, typeof(NewsTableViewCell));
			TableView.Source = source;
			TableView.RowHeight = 70f;

			RefreshControl = new UIRefreshControl ();
			RefreshControl.ValueChanged += delegate {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				ViewModel.RefreshNewsList ();
			};
			var set = this.CreateBindingSet<NewsListViewController,NewsListViewModel> ();
			set.Bind (source).To (vm => vm.News);
			set.Bind (this).For (v => v.DataPopulated).To (vm => vm.DataPopulated);
			set.Bind (source).For (x => x.SelectionChangedCommand).To (vm => vm.ItemClickCommand);
			set.Apply ();
			TableView.ReloadData ();

			ViewModel.RefreshNewsList ();

			NavigationController.NavigationBar.TintColor = UIColor.White;
			ShowMenuIcon ();
		}

		private void ShowMenuIcon ()
		{
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			NavigationItem.SetLeftBarButtonItem (
				new UIBarButtonItem (UIImage.FromBundle ("threelines")
						, UIBarButtonItemStyle.Plain
						, (sender, args) => app.SidebarController.ToggleMenu ()), true);
			
		}

		#endregion
	}

}

