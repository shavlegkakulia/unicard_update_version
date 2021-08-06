using System;
using Google.Maps;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Runtime.InteropServices.WindowsRuntime;

namespace iCunOS.BillBoards
{
	public class HistoryTableCell : UITableViewCell
	{
		#region UI

		private UIImageView _imageView;
		private UILabel _adName, _monthDate, _hoursDate, _dateTitle;

		#endregion

		#region Ctor

		public HistoryTableCell (NSString cellId) : base (UITableViewCellStyle.Default, cellId)
		{
			_imageView = new UIImageView ();
			_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			_imageView.Layer.MasksToBounds = true;
			_dateTitle = new UILabel ();
			_adName = new UILabel ();
			_monthDate = new UILabel ();
			_hoursDate = new UILabel ();

			ContentView.AddSubviews (new UIView[] { _imageView, _dateTitle, _adName, _monthDate, _hoursDate });

		}

		#endregion

		#region Methods

		public void UpdateCell (string addName, string monthDate, string hoursDate, string url)
		{
			if (!string.IsNullOrWhiteSpace (url))
				using (var realUrl = new NSUrl (url))
				using (var data = NSData.FromUrl (realUrl))
					_imageView.Image = UIImage.LoadFromData (data);
			_adName.Text = addName;
			_monthDate.Text = monthDate;
			_hoursDate.Text = hoursDate;
			InitCell ();
		}

		private void InitCell ()
		{
			var cellHeight = 90f;
			var wrapperView = new UIView (new CGRect (10f, 5f, 80f, cellHeight - 10f));
			_imageView.Frame = new CGRect (10f, 10f, wrapperView.Frame.Width - 20f, wrapperView.Frame.Height - 20f);

			wrapperView.Layer.CornerRadius = 40f;
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
		}

		#endregion
	}
}

