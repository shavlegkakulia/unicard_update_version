using System;
using Kunicardus.Core;
using UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreGraphics;
using Kunicardus.Core.ViewModels.iOSSpecific;

namespace Kunicardus.Touch
{
	public class NewPinViewController : BaseMvxViewController
	{
		private SettingsPinView _currentLayout;
		private UITextField _all;

		public new iNewPinViewModel ViewModel {
			get { return (iNewPinViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public NewPinViewController ()
		{
			HideMenuIcon = true;
		}

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff");
			InitUI ();
			Title = this.ViewModel.HeaderTitle;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
			NavigationController.NavigationBar.Translucent = false;
			_all.BecomeFirstResponder ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationController.NavigationBarHidden = true;
			base.ViewWillDisappear (animated);
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			_all = new UITextField (new CGRect (0, 0, 1, 1));
			this.CreateBinding (_all).To ((iNewPinViewModel vm) => vm.NewPin).Apply ();
			_all.BecomeFirstResponder ();
			_currentLayout = new SettingsPinView (this.ViewModel.PageTitle, View);
			_currentLayout.PinProgressFinished += (sender, e) => {
				if (Title == ApplicationStrings.ChangePin) {
					ViewModel.FromSetPin = false;
					ViewModel.PinInputFinished = true;
				} else if (Title == ApplicationStrings.set_pin) {
					ViewModel.FromSetPin = true;
					ViewModel.PinInputFinished = true;
				}
			}; 
			_currentLayout.InitUI (_all, GetStatusBarHeight ());
			View.AddSubview (_all);
		}

		#endregion


	}
}

