using System;
using UIKit;
using Kunicardus.Core;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core.ViewModels.iOSSpecific;

namespace Kunicardus.Touch
{
	public class CardExistanceViewController : BaseRegistrationViewController
	{
		public new iChooseCardExistanceViewModel ViewModel {
			get { return (iChooseCardExistanceViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public CardExistanceViewController () : base (ApplicationStrings.DoUHaveUnicard)
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			InitUI ();
		}

		private void InitUI ()
		{						
			nfloat buttonPaddings = 38;
			UIButton yesButton = new UIButton (UIButtonType.RoundedRect);
			yesButton.SetTitle (ApplicationStrings.Yeap, UIControlState.Normal);
			yesButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 24);
			yesButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			yesButton.BackgroundColor = UIColor.Clear.FromHexString ("#8ab42f");
			yesButton.Frame = new CGRect (buttonPaddings, 180, 110, 110);
			yesButton.Layer.CornerRadius = 55;
			View.AddSubview (yesButton);


			UIButton noButton = new UIButton (UIButtonType.RoundedRect);
			noButton.SetTitle (ApplicationStrings.No, UIControlState.Normal);
			noButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 24);
			noButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			noButton.BackgroundColor = UIColor.Clear.FromHexString ("#8ab42f");
			noButton.Frame = new CGRect (View.Frame.Width - 110 - buttonPaddings, 180, 110, 110);
			noButton.Layer.CornerRadius = 55;
			View.AddSubview (noButton);

			// Bindings
			this.CreateBinding (yesButton).To ((iChooseCardExistanceViewModel vm) => vm.UnicardAvailableCommand).Apply ();
			this.CreateBinding (noButton).To ((iChooseCardExistanceViewModel vm) => vm.UnicartNotAvailableCommand).Apply ();
		}
	}
}

