using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Billboards.Core.ViewModels;

namespace iCunOS.BillBoards
{
	public class AdViewWrapper : UIView
	{
		public AdView Adview {
			get ;
			set ;
		}

	
		public AdViewWrapper (CGRect frame) : base (frame)
		{
		}
	}
}

