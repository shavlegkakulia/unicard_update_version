using System;
using UIKit;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class KuniUILabel : UILabel
	{
		public KuniUILabel ()
		{
			this.Font = UIFont.FromName ("bpg_extrasquare_mtavruli.ttf", 14);
		}

		public KuniUILabel (CGRect rect)
		{
			this.Frame = rect;
			//this.Font = UIFont.FromName (@"bpg_extrasquare_mtavruli", 14f);
		}
	}
}

