using System;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Kunicardus.Core.ViewModels.iOSSpecific;
using MonoTouch.Dialog.Utilities;
using Google.Maps;
using CoreAnimation;

namespace Kunicardus.Touch
{
	public class MyPageViewController : BaseMvxViewController
	{
		#region Props

		UITableView _tableView;

		public new iMyPageViewModel ViewModel {
			get { return (iMyPageViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public UIRefreshControl RefreshControl {
			get;
			set;
		}

		public bool DataPopulated {
			get{ return true; }
			set {
				if (value) {
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					RefreshControl.EndRefreshing ();
					_tableView.ReloadData ();
					_bgView.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
					if (ViewModel.Transactions != null && ViewModel.Transactions.Count > 0) {
						_devider.Hidden = false;
					} else {
						_devider.Hidden = true;
					}
				}
			}
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			RefreshControl = new UIRefreshControl ();
			RefreshControl.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			NavigationController.NavigationBar.Translucent = false;
			View.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			Title = ApplicationStrings.MyPage;
			InitUI ();
			InitRefreshControl ();
		}

		#endregion

		#region Methods

		private void NavBarImage ()
		{
			var size = new CGSize (View.Frame.Width, GetStatusBarHeight ());
			UIGraphics.BeginImageContextWithOptions (size, true, 0);
			var context = UIGraphics.GetCurrentContext ();
			context.FillRect (new CGRect (0, 0, View.Frame.Width, GetStatusBarHeight ()));
			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			NavigationController.NavigationBar.BackIndicatorImage = image;
			NavigationController.NavigationBar.ShadowImage = image;
		}

		private void InitRefreshControl ()
		{
			RefreshControl.ValueChanged += delegate {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				ViewModel.GetData ();
			};
		}

		UIImageView _devider;
		UIView _bgView;

		private void InitUI ()
		{
			nfloat statusBarHeight = 0; 

			#region Top Part
			_devider = new UIImageView (ImageHelper.MaxResizeImage (UIImage.FromBundle ("devider_white_green"), View.Frame.Width, 0));
			_devider.SizeToFit ();
			_devider.BackgroundColor = UIColor.White;
			UIView topView = new UIView ();
			topView.Frame = new CoreGraphics.CGRect (0, statusBarHeight, View.Frame.Width, 120f + _devider.Frame.Height);
			topView.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			//View.AddSubview (topView);
					
			_devider.Frame = new CoreGraphics.CGRect (0, 120, _devider.Frame.Width, _devider.Frame.Height);
			topView.AddSubview (_devider);

			UILabel currentBalanceTitle = new UILabel (new CGRect (0, 10, View.Frame.Width, 16));
			currentBalanceTitle.TextAlignment = UITextAlignment.Center;
			currentBalanceTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
			currentBalanceTitle.TextColor = UIColor.White;
			currentBalanceTitle.Text = ApplicationStrings.CurrentBalance;
			topView.AddSubview (currentBalanceTitle);

			UILabel currentBalance = new UILabel (new CGRect (0, currentBalanceTitle.Frame.Bottom + 4, View.Frame.Width, 20));
			currentBalance.TextAlignment = UITextAlignment.Center;
			currentBalance.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 26);
			currentBalance.TextColor = UIColor.White;
			topView.AddSubview (currentBalance);

			nfloat labelWidth = View.Frame.Width / 3.0f;
			nfloat subFont = 11f;
			nfloat subTop = currentBalance.Frame.Bottom + 20f;
			UILabel blockedTitle = new UILabel (new CGRect (0, subTop, labelWidth, 14));
			blockedTitle.TextAlignment = UITextAlignment.Center;
			blockedTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, subFont);
			blockedTitle.TextColor = UIColor.White;
			blockedTitle.Text = ApplicationStrings.Blocked;
			topView.AddSubview (blockedTitle);

			UILabel spentTitle = new UILabel (new CGRect (blockedTitle.Frame.Right, subTop, labelWidth, 14));
			spentTitle.TextAlignment = UITextAlignment.Center;
			spentTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, subFont);
			spentTitle.TextColor = UIColor.White;
			spentTitle.Text = ApplicationStrings.Spent;
			topView.AddSubview (spentTitle);

			UILabel totalTitle = new UILabel (new CGRect (spentTitle.Frame.Right, subTop, labelWidth, 14));
			totalTitle.TextAlignment = UITextAlignment.Center;
			totalTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, subFont);
			totalTitle.TextColor = UIColor.White;
			totalTitle.Text = ApplicationStrings.SumTotal;
			topView.AddSubview (totalTitle);

			subTop = totalTitle.Frame.Bottom + 2;
			subFont = 18f;
			UILabel blocked = new UILabel (new CGRect (0, subTop, labelWidth, 20));
			blocked.TextAlignment = UITextAlignment.Center;
			blocked.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, subFont);
			blocked.TextColor = UIColor.White;
			topView.AddSubview (blocked);

			UILabel spent = new UILabel (new CGRect (blocked.Frame.Right, subTop, labelWidth, 20));
			spent.TextAlignment = UITextAlignment.Center;
			spent.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, subFont);
			spent.TextColor = UIColor.White;
			topView.AddSubview (spent);

			UILabel total = new UILabel (new CGRect (spent.Frame.Right, subTop, labelWidth, 20));
			total.TextAlignment = UITextAlignment.Center;
			total.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, subFont);
			total.TextColor = UIColor.White;
			topView.AddSubview (total);

			#endregion

			#region Transactions grid

			_tableView = new UITableView (new CGRect (0, 
				0, 
				View.Frame.Width, 
				View.Frame.Height - GetStatusBarHeight ()));
			_bgView = new UIView (new CGRect (0, 0, View.Frame.Width, 20f));
			_bgView.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff");
			_tableView.BackgroundView = _bgView;
			_tableView.TableHeaderView = topView;
			_tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			_tableView.AllowsSelection = false;
			_tableView.AddSubview (RefreshControl);

			var source = new MvxSimpleTableViewSource (_tableView, typeof(MyPageTransactionRow));

			_tableView.Source = source;
			_tableView.RowHeight = 90f;

			this.CreateBinding (source).To ((iMyPageViewModel vm) => vm.Transactions).Apply ();
			_tableView.ReloadData ();
			View.AddSubview (_tableView);

			#endregion

			// Bindings
			this.CreateBinding (currentBalance).To ((iMyPageViewModel vm) => vm.PointsText).Apply ();
			this.CreateBinding (blocked).To ((iMyPageViewModel vm) => vm.BlockedText).Apply ();
			this.CreateBinding (spent).To ((iMyPageViewModel vm) => vm.SpentText).Apply ();
			this.CreateBinding (total).To ((iMyPageViewModel vm) => vm.TotalText).Apply ();
			this.CreateBinding (this).For (x => x.DataPopulated).To ((iMyPageViewModel vm) => vm.DataPopulated).Apply ();
		}

		#endregion
	}
}

