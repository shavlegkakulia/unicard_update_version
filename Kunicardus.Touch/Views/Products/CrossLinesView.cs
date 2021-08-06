using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class CrossLinesView : UIView
	{
		public CrossLinesView (CGRect frame) : base (frame)
		{
			this.BackgroundColor = UIColor.Clear;	
			this.Alpha = 0.6f;
		}

		public override void Draw (CGRect rect)
		{
			using (CGContext g = UIGraphics.GetCurrentContext ()) {
				g.SetLineWidth (2f);
				g.SetStrokeColor (UIColor.Clear.FromHexString (Styles.Colors.LightGray).CGColor);
				// line one
				g.BeginPath ();
				g.MoveTo (0, 0);
				g.AddLineToPoint (Frame.Width, Frame.Height);
				g.ClosePath ();
				g.DrawPath (CGPathDrawingMode.Stroke);

				// line two
				g.BeginPath ();
				g.MoveTo (0, Frame.Height);
				g.AddLineToPoint (Frame.Width, 0);
				g.ClosePath ();
				g.DrawPath (CGPathDrawingMode.Stroke);
			}
		}
	}
}

