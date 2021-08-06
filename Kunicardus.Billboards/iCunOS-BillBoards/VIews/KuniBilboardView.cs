using System;
using UIKit;
using Kunicardus.Billboards.Core.DbModels;
using CoreGraphics;

namespace iCunOS.BillBoards
{
	public class KuniBilboardView : UIView
	{
		private nfloat width = 160f;
		private nfloat height = 95f;

		public KuniBilboardView (Billboard billboard) : base (new CGRect (0, 0, 160f, 105))
		{
			this.BackgroundColor = UIColor.Clear;
			this.Layer.CornerRadius = 10;
			UIView contentView = new UIView (new CGRect (0, 0, width, height - 28));
			contentView.BackgroundColor = UIColor.White;
			this.AddSubview (contentView);

			var imageHeight = 62f;
			var imageWrapper = new UIView (new CGRect (5f, 5f, contentView.Frame.Width - 20f, imageHeight - 10f));
			var adLogo = new UIImageView ();
			adLogo.Frame = imageWrapper.Frame;
			adLogo.Image = ImageHelper.FromBase64 (billboard.MerchantLogo);
			adLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageWrapper.AddSubview (adLogo);

			var merchantTitle = new UILabel ();
			merchantTitle.TextColor = UIColor.White;
			merchantTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 2f);
			merchantTitle.LineBreakMode = UILineBreakMode.WordWrap;
			merchantTitle.Lines = 0;
			merchantTitle.Text = billboard.MerchantName;

			merchantTitle.SizeToFit ();

			var calculatedSize = merchantTitle.SizeThatFits (contentView.Frame.Size);
			if (merchantTitle.Frame.Width > width) {
				this.Frame = new CGRect (0, 0, 160f, this.Frame.Height + 10);
				height += 10f;
				var mFrame = merchantTitle.Frame;
				mFrame.X = 10f;
				mFrame.Y = adLogo.Frame.Bottom + 5f;
				mFrame.Width = calculatedSize.Width;
				mFrame.Height = calculatedSize.Height - 10f;
				merchantTitle.Frame = mFrame;
				merchantTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f);

			} else {
				var mFrame = merchantTitle.Frame;
				mFrame.X = 10f;
				mFrame.Y = adLogo.Frame.Bottom + 14f;
				merchantTitle.Frame = mFrame;
			}

			contentView.AddSubviews (imageWrapper, merchantTitle);

		}

		public override void Draw (CGRect rect)
		{
			using (CGContext g = UIGraphics.GetCurrentContext ()) {
				g.BeginPath ();

				g.MoveTo (0, 0);
				g.AddLineToPoint (width, 0);
				g.AddLineToPoint (width, height);
				g.AddLineToPoint (width / 2f + 10, height);
				g.AddLineToPoint (width / 2f, height + 10f);
				g.AddLineToPoint (width / 2f - 10, height);
				g.AddLineToPoint (0, height);
				g.AddLineToPoint (0, 20);

				g.ClosePath ();
				g.SetFillColor (UIColor.Clear.FromHexString (Styles.Colors.Red).CGColor);		
				g.FillPath ();
			}
		}
	}

}

