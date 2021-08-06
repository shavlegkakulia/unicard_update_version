using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core.Models.DB;

namespace Kunicardus.Touch
{
	[Register ("MyPageTransactionRow")]
	public class MyPageTransactionRow : MvxTableViewCell
	{

		#region UI

		private UILabel _points;
		private UILabel _address;
		private UILabel _time;
		private UILabel _amount;
		private UILabel _merchant;
		private UIView _topStick;
		private UIView _bottomStick;

		#endregion

		#region Ctors

		public MyPageTransactionRow (IntPtr handle) : base (handle)
		{
			if (ContentView.Frame.Height != 90) {
				var frame = ContentView.Frame;
				frame.Height = 90f;
				ContentView.Frame = frame;
			}
			CreateLayout ();
			InitializeBindings ();
		}

		#endregion

		#region Methods

		#endregion

		private void CreateLayout ()
		{
			
			Accessory = UITableViewCellAccessory.None;

			nfloat bowlHeight = 20;
			nfloat left = 6f;
			_topStick = new UIView (new CGRect (left + (bowlHeight / 2.0f) - 1, 0, 2, (ContentView.Frame.Height - bowlHeight) / 2.0f));
			_topStick.BackgroundColor = UIColor.Clear.FromHexString ("#d4dedf");
			ContentView.AddSubviews (_topStick);

			UIButton bowl = new UIButton (UIButtonType.System);
			bowl.Frame = new CGRect (left, (ContentView.Frame.Height - bowlHeight) / 2.0f, bowlHeight, bowlHeight);
			bowl.BackgroundColor = UIColor.White;
			bowl.SetTitleColor (UIColor.Clear.FromHexString ("#d4dedf"), UIControlState.Normal);
			bowl.SetTitle ("•", UIControlState.Normal);
			bowl.Layer.CornerRadius = bowlHeight / 2.0f;
			bowl.Layer.BorderColor = UIColor.Clear.FromHexString ("#d4dedf").CGColor;
			bowl.Layer.BorderWidth = 2.0f;
			bowl.Font = UIFont.SystemFontOfSize (36);
			bowl.TitleEdgeInsets = new UIEdgeInsets (0, 0, 0, 1);
			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
				bowl.TitleEdgeInsets = new UIEdgeInsets (-2.15f, 0.15f, 2.15f, -0.15f);
			}
			bowl.Enabled = false;
			ContentView.AddSubviews (bowl);

			_bottomStick = new UIView (new CGRect (left + (bowlHeight / 2.0f) - 1, _topStick.Frame.Bottom + bowlHeight, 2, (ContentView.Frame.Height - bowlHeight) / 2.0f));
			_bottomStick.BackgroundColor = UIColor.Clear.FromHexString ("#d4dedf");
			ContentView.AddSubviews (_bottomStick);

			left = bowl.Frame.Right + 6f;
			_merchant = new UILabel (new CGRect (left, 10, ContentView.Frame.Width - left, 16));
			_merchant.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 17);
			ContentView.AddSubviews (_merchant);


			UIImageView clock = new UIImageView (UIImage.FromBundle ("clock_grey"));
			clock.SizeToFit ();
			clock.Frame = new CGRect (left, _merchant.Frame.Bottom + 5, clock.Frame.Width, clock.Frame.Height);
			ContentView.AddSubviews (clock);

			_time = new UILabel (new CGRect (clock.Frame.Right + 5, clock.Frame.Top, 135, 14));
			_time.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			ContentView.AddSubviews (_time);

			_amount = new UILabel (new CGRect (_time.Frame.Right + 1, clock.Frame.Top, 45, 14));
			_amount.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			_amount.TextAlignment = UITextAlignment.Right;
			ContentView.AddSubviews (_amount);

			UIImageView gel = new UIImageView (ImageHelper.MaxResizeImage (UIImage.FromBundle ("gel"), 14, 0));
			gel.SizeToFit ();
			gel.Frame = new CGRect (_amount.Frame.Right + 1f, clock.Frame.Top, gel.Frame.Width, gel.Frame.Height);
			ContentView.AddSubviews (gel);

		


			UIImageView marker = new UIImageView (UIImage.FromBundle ("marker"));
			marker.SizeToFit ();
			marker.Frame = new CGRect (left, clock.Frame.Bottom + 5, marker.Frame.Width, marker.Frame.Height);
			ContentView.AddSubviews (marker);

			_address = new UILabel (new CGRect (marker.Frame.Right + 5, marker.Frame.Top - 2, _amount.Frame.Right - (marker.Frame.Right + 5), 
				ContentView.Frame.Height - marker.Frame.Top - 2));
			_address.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
			_address.LineBreakMode = UILineBreakMode.WordWrap;
			_address.Lines = 2;
			_address.TextColor = UIColor.Clear.FromHexString ("#95a3a9");
			_address.TextAlignment = UITextAlignment.Left;
			ContentView.AddSubviews (_address);

			_points = new UILabel (
				new CGRect (_amount.Frame.Right, (ContentView.Frame.Height - 14) / 2.0f, ContentView.Frame.Width - _amount.Frame.Right - 6, 14));
			_points.TextAlignment = UITextAlignment.Right;
			_points.Font = UIFont.SystemFontOfSize (14);
			ContentView.AddSubviews (_points);

		}

		private void InitializeBindings ()
		{
			this.DelayBind (() => {
				var set = this.CreateBindingSet<MyPageTransactionRow, TransactionInfo> ();
				set.Bind (_merchant).To (vm => vm.OrganizationName);
				set.Bind (_address).To (vm => vm.Address);
				set.Bind (_time).To (vm => vm.Date).WithConversion ("TransactionsDate");
				set.Bind (_amount).To (vm => vm.PaymentAmount);
				set.Bind (_points).To (vm => vm.Score).WithConversion ("Points");
				set.Bind (_points).For (x => x.TextColor).To (vm => vm.Score).WithConversion ("iOSPointsColor");			

				set.Bind (_topStick)
					.For (v => v.Hidden)
					.To (vm => vm.First);
				set.Bind (_bottomStick)
					.For (v => v.Hidden)
					.To (vm => vm.Last);
				
				set.Apply ();
			});
		}
	}
}

