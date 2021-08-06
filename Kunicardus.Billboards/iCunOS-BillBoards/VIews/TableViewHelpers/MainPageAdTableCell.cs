using System;
using UIKit;
using Foundation;
using System.Linq.Expressions;
using CoreGraphics;
using iCunOSBillBoards;
using Kunicardus.Billboards.Core.Helpers;

namespace iCunOS.BillBoards
{
	public class MainPageAdTableCell:UITableViewCell
	{
		#region UI

		private UIImageView _imageView;
		private UILabel _adName, _monthDate, _hoursDate, _dateTitle;

		#endregion

		#region Constructor

		public MainPageAdTableCell (NSString cellId) : base (UITableViewCellStyle.Default, cellId)
		{
			_imageView = new UIImageView ();
			_imageView.Layer.MasksToBounds = true;
			_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			_adName = new UILabel ();
			_monthDate = new UILabel ();
			_hoursDate = new UILabel ();
			_dateTitle = new UILabel ();
			ContentView.AddSubviews (new UIView[] { _imageView, _adName, _monthDate, _hoursDate, _dateTitle });
		}

		#endregion

		#region Methods

		public void UpdateCell (string addName, DateTime passDate, string imageData)
		{
			if (!string.IsNullOrWhiteSpace (imageData)) {
				_imageView.Image = UIImage.LoadFromData (new NSData (imageData, NSDataBase64DecodingOptions.None));
			}
			_adName.Text = addName;
			_monthDate.Text = passDate.ToGeoString ();
			_hoursDate.Text = passDate.ToString ("HH:mm");
			InitCell ();
		}

		private void InitCell ()
		{
			var cellHeight = 75f;
			var wrapperView = new UIView (new CGRect (10f, 5f, 65f, cellHeight - 10f));
			_imageView.Frame = new CGRect (10f, 10f, wrapperView.Frame.Width - 20f, wrapperView.Frame.Height - 20f);
			wrapperView.Layer.CornerRadius = 32f;
			wrapperView.Layer.BorderWidth = 1f;
			wrapperView.ClipsToBounds = true;
			wrapperView.Layer.BorderColor = UIColor.Clear.FromHexString ("#e6eced").CGColor;
			wrapperView.AddSubview (_imageView);
			ContentView.AddSubview (wrapperView);

			_dateTitle.Text = ApplicationStrings.BilboardSeenTime;
			_dateTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 9f);
			_dateTitle.TextColor = UIColor.Clear.FromHexString ("#999999");
			_dateTitle.SizeToFit ();
			var frame = _dateTitle.Frame;
			frame.X = ContentView.Frame.Width - _dateTitle.Frame.Width - 10f;
			frame.Y = 10f;
			_dateTitle.Frame = frame;

			_monthDate.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
			_monthDate.SizeToFit ();
			frame = _monthDate.Frame;
			frame.X = ContentView.Frame.Width - _monthDate.Frame.Width - 10f; 
			frame.Y = _dateTitle.Frame.Bottom + 4f;
			_monthDate.Frame = frame;

			_hoursDate.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
			_hoursDate.SizeToFit ();
			frame = _hoursDate.Frame;
			frame.X = ContentView.Frame.Width - _hoursDate.Frame.Width - 10f; 
			frame.Y = _monthDate.Frame.Bottom + 4f;
			_hoursDate.Frame = frame;

			_adName.Frame = new CGRect (wrapperView.Frame.Right + 10f, 0, _hoursDate.Frame.Left - _imageView.Frame.Right - 20f, cellHeight);
			_adName.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f);
			_adName.TextColor = UIColor.Black;
			_adName.LineBreakMode = UILineBreakMode.WordWrap;
			_adName.Lines = 0;

			_adName.Frame = new CGRect (wrapperView.Frame.Right + 10f, 0, _hoursDate.Frame.Left - _imageView.Frame.Right - 20f, cellHeight);
			_adName.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f);
			_adName.TextColor = UIColor.Black;
			_adName.LineBreakMode = UILineBreakMode.WordWrap;
			_adName.Lines = 0;
		}

		#endregion
	}
}

