using System;
using Kunicardus.Core.ViewModels;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreGraphics;
using Cirrious.MvvmCross.Touch.Views;
using System.Xml;
using Kunicardus.Touch.Helpers.UI;
using Kunicardus.Touch.Plugins.UIDialogPlugin;
using System.Threading.Tasks;

namespace Kunicardus.Touch
{
	public class OrganisationListViewController : MvxTableViewController
	{
		public new OrganisationListViewModel ViewModel {
			get { return (OrganisationListViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public bool DataPopulated {
			get{ return true; }
			set {
				if (value) {
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					RefreshControl.EndRefreshing ();
					TableView.ReloadData ();
				}
			}
		}

		#region Search UI

		private UIBarButtonItem _SearchButton;
		private UISearchBar _SearchWindow;
		private KeyboardTopBar _keyboardBar;

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;
			Title = ApplicationStrings.Partners;
			TableView = new UITableView (new CGRect (0, 0, View.Frame.Width, View.Frame.Height));
			var source = new MvxSimpleTableViewSource (TableView, typeof(OrganizationsTableViewCell));
			TableView.Source = source;
			TableView.RowHeight = 80f;

			var set = this.CreateBindingSet<OrganisationListViewController,OrganisationListViewModel> ();
			set.Bind (source).To (vm => vm.Organisations);
			set.Bind (source).For (x => x.SelectionChangedCommand).To (vm => vm.ItemClick);
			set.Bind (this).For (x => x.DataPopulated).To (vm => vm.DataPopulated);
			set.Apply ();

			TableView.ReloadData ();

			RefreshControl = new UIRefreshControl ();
			RefreshControl.ValueChanged += delegate {
				string search = "";
				if (NavigationItem.RightBarButtonItem == null) {
					search = _SearchWindow.Text;
				}
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				ViewModel.GetOrganisations (true, search);
			};

			NavigationController.NavigationBar.TintColor = UIColor.White;
			ShowMenuIcon ();

			InitSearch ();

			new TouchUIDialogPlugin ().ShowProgressDialog ("");
			Task.Run (() => {
				this.ViewModel.GetOrganisations (false);
				UIApplication.SharedApplication.InvokeOnMainThread (() => {
					new TouchUIDialogPlugin ().DismissProgressDialog ();
				});
			});
		}

		private void ShowMenuIcon ()
		{
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			NavigationItem.SetLeftBarButtonItem (
				new UIBarButtonItem (UIImage.FromBundle ("threelines")
					, UIBarButtonItemStyle.Plain
					, (sender, args) => app.SidebarController.ToggleMenu ()), true);

			UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment (new UIOffset (0, 0), UIBarMetrics.Default);
		}

		#region Search Methods

		private void InitSearch ()
		{
			// Init Search
			if (_SearchButton == null) {
				_SearchButton = new UIBarButtonItem (UIBarButtonSystemItem.Search, (s, e) => {
					ShowSearchWindow ();
				});
			}
			NavigationItem.RightBarButtonItem = _SearchButton;
		}

		private void ShowSearchWindow ()
		{
			if (_SearchWindow == null) {
				_SearchWindow = new UISearchBar ();
				_SearchWindow.Placeholder = ApplicationStrings.Search;
				_SearchWindow.KeyboardType = UIKeyboardType.WebSearch;
				_keyboardBar = new KeyboardTopBar ();
				_keyboardBar.NextEnabled = false;
				_keyboardBar.PreviousEnabled = false;
				_keyboardBar.OnDone += delegate {
					_SearchWindow.ResignFirstResponder ();
				};
				_SearchWindow.InputAccessoryView = _keyboardBar;
				_SearchWindow.TintColor = UIColor.White;
				_SearchWindow.ShowsScopeBar = true;
				_SearchWindow.SearchButtonClicked += (sender, e) => {
					ViewModel.Filter (_SearchWindow.Text);
					_SearchWindow.ResignFirstResponder ();
				};


				_SearchWindow.ShowsCancelButton = true;

				_SearchWindow.CancelButtonClicked += delegate {
					DisposeSearchWindow ();
				};
			}

			ShowSearchWindow (_SearchWindow);
		}

		private void DisposeSearchWindow ()
		{
			ShowMenuIcon ();
			NavigationItem.RightBarButtonItem = _SearchButton;
			ViewModel.Filter ("");
			UIView.Animate (
				0.1, 
				() => {
					NavigationItem.TitleView = null;
					NavigationItem.Title = ApplicationStrings.AroundMe;
				}
			);
		}

		private void ShowSearchWindow (UIView searchView)
		{
			NavigationItem.LeftBarButtonItem = null;
			NavigationItem.RightBarButtonItem = null;
			UIView.Animate (
				0.1,
				() => {
					NavigationItem.TitleView = searchView;
				},
				() => {
					searchView.BecomeFirstResponder ();
				}
			);
		}

		#endregion
	}
}

