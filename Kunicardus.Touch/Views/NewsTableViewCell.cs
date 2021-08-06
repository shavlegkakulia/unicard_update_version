using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Drawing;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core.Models.DB;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	[Register ("NewsTableViewCell")]
	public class NewsTableViewCell : MvxTableViewCell
	{
		public NewsTableViewCell ()
		{
			CreateLayout ();
			InitializeBindings ();
		}

		public NewsTableViewCell (IntPtr handle) : base (handle)
		{
			CreateLayout ();
			InitializeBindings ();
		}

		private UILabel _title;
		private UILabel _date;
		private UIImageView _image;
		private MvxImageViewLoader _loader;

		private void CreateLayout ()
		{
			const int offsetStart = 10;
			const float imageWidth = 80;
			const float imageHeight = 50;
			const int padding = 5;
			Accessory = UITableViewCellAccessory.None;
			_title = new UILabel (new RectangleF (offsetStart + imageWidth + padding, 3, (float)ContentView.Frame.Width - offsetStart - imageWidth - 30, 40));
			_title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 7);
			_title.LineBreakMode = UILineBreakMode.WordWrap;
			_title.Lines = 0;

			_date = new UILabel (new RectangleF (offsetStart + imageWidth + padding, (float)_title.Frame.Height + 5, 200, 20));
			_date.TextAlignment = UITextAlignment.Left;
			_date.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 8);
			_date.TextColor = UIColor.LightGray;

			_image = new UIImageView (new CGRect (offsetStart, padding, imageWidth, imageHeight));
			_loader = new MvxImageViewLoader (() => _image);

			ContentView.AddSubviews (_title, _date, _image);

		}


		private void InitializeBindings ()
		{
			this.DelayBind (() => {
				var set = this.CreateBindingSet<NewsTableViewCell, NewsInfo> ();
				set.Bind (_title).To (vm => vm.Title);
				set.Bind (_date).To (vm => vm.CreateDate).WithConversion ("NewsDate", "HH:MM:ss");
				set.Bind (_loader).To (vm => vm.Image);
				set.Bind (_title).For (x => x.TextColor).To (t => t.IsRead).WithConversion ("NewsIsReadColor");			
				set.Apply ();
			});
		}
	}
}

