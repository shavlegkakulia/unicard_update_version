using System;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using Foundation;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class BaseRegistrationViewController : BaseMvxViewController
	{
		private string _subHeading;

		public RegistrationSubHeading  SubHeading {
			get;
			set;
		}

		public BaseRegistrationViewController (string subHeading)
		{
			_subHeading = subHeading;
			HideMenuIcon = true;
		}



		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBar.Translucent = true;
			View.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			Title = ApplicationStrings.Registration;

			if (!string.IsNullOrWhiteSpace (_subHeading)) {
				SubHeading = new RegistrationSubHeading (_subHeading, View.Frame.Width);
				View.AddSubview (SubHeading);
			}



		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
			NavigationController.NavigationBar.Translucent = true;
		}

		public override void ViewWillDisappear (bool animated)
		{			
			base.ViewWillDisappear (animated);
		}
	}
}

