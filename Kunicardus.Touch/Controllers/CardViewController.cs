using System;
using UIKit;

namespace Kunicardus.Touch
{
	public class CardViewController : UIViewController
	{
		
		#region Variables

		private string _cardNumber;

		#endregion

		#region Ctors

		public CardViewController (string cardNumber) : base ()
		{
			_cardNumber = cardNumber;
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CardForScanView cardView = new CardForScanView (this.View.Frame, _cardNumber);
			cardView.Close.TouchUpInside += Close;
			View.AddSubview (cardView);
		}

		public override void ViewWillAppear (bool animated)
		{
			UIApplication.SharedApplication.SetStatusBarHidden (true, false);
			base.ViewWillAppear (animated);
		}

		#endregion

		#region Events

		void Close (object sender, EventArgs e)
		{			
			this.DismissModalViewController (true);
			UIApplication.SharedApplication.SetStatusBarHidden (false, false);
		}

		#endregion
	}
}

