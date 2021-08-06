using System;
using UIKit;

namespace iCunOS.BillBoards
{
	public class AboutViewController : BaseViewController
	{
		public AboutViewController ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			Title = "About Us";
		}
	}
}

