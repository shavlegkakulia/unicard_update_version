using System;
using Cirrious.MvvmCross.Touch.Views;
using Kunicardus.Core;
using Kunicardus.Core.ViewModels;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Foundation;
using MessageUI;
using System.Collections.Generic;

namespace Kunicardus.Touch
{
	public class AboutUsViewController : BaseMvxViewController
	{
		#region Private Variables

		private nfloat _padding = 15;
		private nfloat _paddingTopRisesMenu = 60;
		private UIScrollView _mainScrollView;
		private UIButton _phoneButton, _emailButton, _webpageButton, _fbButton, _rateButton, _shareButton, _unimania;

		#endregion

		#region Properties

		public new AboutViewModel ViewModel {
			get { return (AboutViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = ApplicationStrings.AboutUs;
			InitUI ();
		}

		#endregion

		#region Methods

		private void InitUI ()
		{			
			View.BackgroundColor = UIColor.White;
			_mainScrollView = new UIScrollView (
				new CGRect (0, 0, View.Frame.Width, View.Frame.Height));
			
			View.AddSubview (_mainScrollView);
			// adding image of unicard
			var logoImage = ImageHelper.MaxResizeImage (UIImage.FromBundle ("unicard_logo"), View.Frame.Width - 20, 0);
			UIImageView logo = new UIImageView (new CGRect (0, 0, View.Frame.Width, 170));
			logo.Image = logoImage;
			logo.SizeToFit ();
			logo.Frame = new CGRect ((View.Frame.Width - logo.Frame.Width) / 2.0f, 10, logo.Frame.Width, logo.Frame.Height); 

			_mainScrollView.AddSubview (logo);

			//adding other views
			nfloat tempPadding = logo.Frame.Height + 10;
			var hoursButton = new UIButton (UIButtonType.System);
			this.CreateBinding (hoursButton).For ("Title").To ((AboutViewModel vm) => vm.WorkingHours).Apply ();
			this.AddBindings (new Dictionary<object,string> (){ { hoursButton, "Title WorkingHours" } });
			AddSubView ("clock_green", tempPadding, hoursButton);
			tempPadding += _paddingTopRisesMenu;

			_phoneButton = new UIButton (UIButtonType.System);
			this.CreateBinding (_phoneButton).For ("Title").To ((AboutViewModel vm) => vm.Phone).Apply ();
			this.AddBindings (new Dictionary<object,string> (){ { _phoneButton, "Title Phone" } });
			AddSubView ("phone_green", tempPadding, _phoneButton);
			tempPadding += _paddingTopRisesMenu;

			_emailButton = new UIButton (UIButtonType.System);
			this.CreateBinding (_emailButton).For ("Title").To ((AboutViewModel vm) => vm.Mail).Apply ();
			this.AddBindings (new Dictionary<object,string> (){ { _emailButton, "Title Mail" } });
			AddSubView ("email_green", tempPadding, _emailButton);
			tempPadding += _paddingTopRisesMenu;

			_webpageButton = new UIButton (UIButtonType.System);
			this.CreateBinding (_webpageButton).For ("Title").To ((AboutViewModel vm) => vm.WebPage).Apply ();
			this.AddBindings (new Dictionary<object,string> (){ { _webpageButton, "Title WebPage" } });
			AddSubView ("webpage_green", tempPadding, _webpageButton);
			tempPadding += _paddingTopRisesMenu;

			_fbButton = new UIButton (UIButtonType.System);
			this.CreateBinding (_fbButton).For ("Title").To ((AboutViewModel vm) => vm.Facebook).Apply ();
			this.AddBindings (new Dictionary<object,string> (){ { _fbButton, "Title Facebook" } });
			AddSubView ("fb_green", tempPadding, _fbButton);
			tempPadding += _paddingTopRisesMenu;

			_unimania = new UIButton (UIButtonType.System);
			_unimania.SetTitle ("www.unimania.ge", UIControlState.Normal);
			AddSubView ("unimania", tempPadding, _unimania);
			tempPadding += _paddingTopRisesMenu;

			_shareButton = new UIButton (UIButtonType.System);
			_rateButton = new UIButton (UIButtonType.System);
			_shareButton.SetTitle (ApplicationStrings.ShareApp, UIControlState.Normal);
			_rateButton.SetTitle (ApplicationStrings.RateApp, UIControlState.Normal);
			AddSubView ("share", tempPadding, _shareButton);
			tempPadding += _paddingTopRisesMenu;
			AddSubView ("rate", tempPadding, _rateButton);

//			_unimania = new UIButton (UIButtonType.System);
//			_unimania.SetTitle (ApplicationStrings.Unimania, UIControlState.Normal);
//			tempPadding += _paddingTopRisesMenu;
//			AddSubView ("unimania", tempPadding, _unimania);


			_mainScrollView.ContentSize = new CGSize (View.Frame.Width, _rateButton.Frame.Bottom + 12f);
			LabelTouchLogic ();
		}

		private UIView LineDivider (nfloat top)
		{
			UIView view = new UIView (new CGRect (0, top, View.Frame.Width, 1f));
			view.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
			return view;
		}

		private void AddSubView (string imageSource, nfloat tempPadding, UIButton button)
		{
			_mainScrollView.AddSubview (LineDivider (tempPadding));
			UIImageView workingHoursImageView = new UIImageView (UIImage.FromBundle (imageSource));
			var imageWidth = workingHoursImageView.Image.CGImage.Width;
			var imageHeight = workingHoursImageView.Image.CGImage.Height;
			float proportion;
			nint imageSize = 28;
			if (imageWidth > imageHeight) {
				proportion = (float)imageSize / imageWidth;
				imageWidth = imageSize;
				imageHeight = Convert.ToInt32 (((float)imageHeight * proportion));

			} else {
				proportion = (float)imageSize / imageHeight;
				imageHeight = imageSize;
				imageWidth = Convert.ToInt32 (((float)imageWidth * proportion));
			}

			
			button.Frame = new CGRect (0, tempPadding + _padding, View.Frame.Width, 35);
			button.SetTitleColor (UIColor.Clear.FromHexString ("#8DBD3B"), UIControlState.Normal);
			button.SetImage (ImageHelper.MaxResizeImage (UIImage.FromBundle (imageSource), imageWidth, imageHeight), UIControlState.Normal);
			button.TintColor = UIColor.Clear.FromHexString ("#8DBD3B");
			button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			if (imageSource == "phone_green") {
				button.ImageEdgeInsets = new UIEdgeInsets (0.0f, 18.0f, 8.0f, 0.0f);
				button.TitleEdgeInsets = new UIEdgeInsets (0.0f, 27.0f, 10.0f, 0.0f);
			} else {
				button.ImageEdgeInsets = new UIEdgeInsets (0.0f, 13.0f, 7.0f, 0.0f);
				button.TitleEdgeInsets = new UIEdgeInsets (0.0f, 22.0f, 10.0f, 0.0f);
			}
			button.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 4);
			_mainScrollView.AddSubview (button);

			tempPadding += _paddingTopRisesMenu;
			_mainScrollView.AddSubview (LineDivider (tempPadding));
		}

		private void LabelTouchLogic ()
		{
			//phone number click
			_phoneButton.TouchUpInside += (sender, e) => {
				var phoneNumber = this.ViewModel.Phone.Replace ("(", "").Replace (")", "").Replace (" ", "").Trim ();
				var url = new NSUrl ("tel://" + phoneNumber);
				if (!UIApplication.SharedApplication.CanOpenUrl (url)) {
					var av = new UIAlertView ("Not supported",
						         "Scheme 'tel:' is not supported on this device",
						         null,
						         "OK",
						         null);
					av.Show ();
				} else {
					UIApplication.SharedApplication.OpenUrl (url);
				}
			};

			//mail click
			_emailButton.TouchUpInside += (sender, e) => {
				MFMailComposeViewController mailController;
				if (MFMailComposeViewController.CanSendMail) {
					mailController = new MFMailComposeViewController ();
					mailController.SetToRecipients (new string[]{ this.ViewModel.Mail });
					mailController.NavigationBar.TintColor = UIColor.White;
					this.PresentViewController (mailController, true, null);
					mailController.Finished += ( object s, MFComposeResultEventArgs args) => {
						args.Controller.DismissViewController (true, null);
					};
				}
			};

			//webpage click
			_webpageButton.TouchUpInside += (sender, e) => {
				OpenWebPage (this.ViewModel.WebPage);
			};

			//fb Click
			_fbButton.TouchUpInside += (sender, e) => {
				OpenWebPage (this.ViewModel.Facebook);
			};

			//share click
			_shareButton.TouchUpInside += (sender, e) => {
				var item = new NSObject[] { UIActivity.FromObject ("https://itunes.apple.com/us/app/unicard-app/id1033843442") };
				UIActivityViewController share = new UIActivityViewController (item, null);
				PresentViewController (share, true, () => {
				});
			};

			//share click
			_rateButton.TouchUpInside += (sender, e) => {
				
				UIApplication.SharedApplication.OpenUrl (new NSUrl ("https://itunes.apple.com/us/app/unicard-app/id1033843442"));
			};

//			//unimania click
//			_unimania.TouchUpInside += (sender, e) => {
//				OpenWebPage ("unimania.ge");
//			};
		}

		private void OpenWebPage (string webAddress)
		{
			string address;
			if (!this.ViewModel.WebPage.ToLower ().Contains ("http") && !this.ViewModel.WebPage.ToLower ().Contains ("https")) {
				address = "http://" + webAddress;
			} else
				address = webAddress;
			UIApplication.SharedApplication.OpenUrl (new NSUrl (address));
		}

		#endregion

	}
}

