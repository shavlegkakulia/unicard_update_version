using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class CatalogFilterItem : UIButton
	{
		public bool Discounted { get; set; }

		public bool LastAdded { get; set; }

		public int? CategoryId { get; set; }

		public int? UserTypeId { get; set; }

		public int? PointRangeId { get; set; }

		public CatalogFilterItem (CGRect frame) : base ()
		{						
			this.Frame = frame;
			this.BackgroundColor = UIColor.Clear.FromHexString ("#9ccd45");
			this.SetTitleColor (UIColor.White, UIControlState.Normal);
			this.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 13);
			this.Layer.CornerRadius = frame.Height / 2.0f;
			this.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			this.TitleEdgeInsets = new UIEdgeInsets (0, 10, 0, 10);
		}
	}
}

