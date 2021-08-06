using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class ProductDetailsDiscountView : UIView
	{
		private string _title;

		public string Title {
			get { 
				return _title;
			}
			set {
				_title = value;
				_titleLabel.Text = string.Format ("{0}%", value);
			}
		}

		private UILabel _titleLabel;
		private nfloat arrowWidh = 15f;

		public ProductDetailsDiscountView (CGRect frame) : base (frame)
		{			
			this.BackgroundColor = UIColor.Clear;
			_titleLabel = new UILabel (new CGRect (0, 0, this.Frame.Width - arrowWidh, this.Frame.Height));
			_titleLabel.TextAlignment = UITextAlignment.Center;
			_titleLabel.TextColor = UIColor.White;
			_titleLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15);
			this.AddSubview (_titleLabel);
		}

		public override void Draw (CGRect rect)
		{
			using (CGContext g = UIGraphics.GetCurrentContext ()) {
				g.BeginPath ();

				g.MoveTo (0, 0);
				g.AddLineToPoint (Frame.Width, 0);
				g.AddLineToPoint (Frame.Width - arrowWidh, Frame.Height / 2.0f);
				g.AddLineToPoint (Frame.Width, Frame.Height);
				g.AddLineToPoint (0, Frame.Height);
				g.AddLineToPoint (0, 0);

				g.ClosePath ();
				g.SetFillColor (UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen).CGColor);		
				g.FillPath ();
			}
		}
	}
}

