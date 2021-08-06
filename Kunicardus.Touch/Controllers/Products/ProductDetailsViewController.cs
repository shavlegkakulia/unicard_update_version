using System;
using UIKit;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core.ViewModels.iOSSpecific;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Touch.Helpers.UI;
using System.Linq;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.Models;
using System.Collections.Generic;

namespace Kunicardus.Touch
{
	public class ProductDetailsViewController : BaseMvxViewController
	{
		
		#region Props

		public new iCatalogDetailViewModel ViewModel {
			get { return (iCatalogDetailViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		private string _imageCountName;

		public string ImageCountName {
			get {
				return _imageCountName;
			}
			set {
				_imageCountName = value;
				imageCount.SetTitle (value, UIControlState.Normal);
			}
		}

		private List<DiscountModel> _userDiscounts;

		public List<DiscountModel> UserDiscounts { 
			get{ return _userDiscounts; }
			set {
				_userDiscounts = value;
				if (_userDiscounts != null && _userDiscounts.Count > 0) {
					InitAdditionalDiscountsView ();
				}
			}
		}

		#endregion

		#region UI

		UIScrollView _scroll;
		UIButton imageCount;
		BindableUIWebView _webview;
		UILabel _idLabel;

		#endregion

		#region Ctors

		public ProductDetailsViewController ()
		{
			HideMenuIcon = true;
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBar.Translucent = false;
			NavigationController.NavigationBarHidden = false;
			View.BackgroundColor = UIColor.White;
			this.Title = ApplicationStrings.WhereToSpend;
			InitUI ();
		}

		public override void ViewWillAppear (bool animated)
		{
			NavigationController.NavigationBarHidden = false;
			base.ViewWillAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationController.NavigationBarHidden = true;
			base.ViewWillDisappear (animated);
		}

		#endregion

		#region Methods

		private void InitAdditionalDiscountsView ()
		{
			// Discounts
			UserDiscountsExpandableList additionalDiscounts = new UserDiscountsExpandableList (View.Frame.Width - 20, 10, _idLabel.Frame.Bottom + 10,
				                                                  ApplicationStrings.UHaveDiscounts);

			nfloat tmpTop = additionalDiscounts.Frame.Height + 5;

			UIView line = new UIView (new CGRect (0, tmpTop, additionalDiscounts.Frame.Width, 1.5f));
			line.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
			additionalDiscounts.AddSubview (line);
			tmpTop += 5;
			foreach (var d in _userDiscounts) {
				UILabel parcentLabel = new UILabel ();
				parcentLabel.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
				parcentLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
				parcentLabel.Text = string.Format ("{0}%", d.DiscountPercent);
				parcentLabel.SizeToFit ();
				parcentLabel.Frame = new CGRect (0, tmpTop, parcentLabel.Frame.Width, parcentLabel.Frame.Height);
				additionalDiscounts.AddSubview (parcentLabel);

				UILabel pointTitle = new UILabel ();
				pointTitle.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
				pointTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
				pointTitle.Text = ApplicationStrings.Score;
				pointTitle.SizeToFit ();
				pointTitle.Frame = 
					new CGRect (additionalDiscounts.Frame.Width - pointTitle.Frame.Width, 
					parcentLabel.Frame.Bottom - pointTitle.Frame.Height - 2, pointTitle.Frame.Width, pointTitle.Frame.Height);
				additionalDiscounts.AddSubview (pointTitle);

				UILabel points = new UILabel ();
				points.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
				points.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
				points.Text = Math.Round (ViewModel.ProductPrice - (ViewModel.ProductPrice * d.DiscountPercent / 100f), 2).ToString ();
				points.SizeToFit ();
				points.Frame = 
					new CGRect (pointTitle.Frame.Left - points.Frame.Width - 1, 
					parcentLabel.Frame.Bottom - points.Frame.Height - 1, 
					points.Frame.Width, 
					points.Frame.Height);
				additionalDiscounts.AddSubview (points);

				UILabel desc = new UILabel ();
				desc.TextColor = UIColor.Black;
				desc.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 10);
				desc.Text = d.DiscountDescription;
				desc.SizeToFit ();
				desc.Frame = 
					new CGRect (parcentLabel.Frame.Right + 1, 
					parcentLabel.Frame.Bottom - desc.Frame.Height - 2, 
					//desc.Frame.Width, 
					points.Frame.Left - parcentLabel.Frame.Right - 2,
					desc.Frame.Height);
				additionalDiscounts.AddSubview (desc);

				tmpTop = parcentLabel.Frame.Bottom + 5;
				additionalDiscounts.contentHeight = tmpTop + 10;
			}


			_webview.Frame = new CGRect (_webview.Frame.X, additionalDiscounts.Frame.Bottom + 10, _webview.Frame.Width, _webview.Frame.Height);
			_scroll.AddSubview (additionalDiscounts);
			additionalDiscounts.AfterClick = () => {
				_webview.Frame = new CGRect (_webview.Frame.X,
					additionalDiscounts.Frame.Bottom + 10,
					_webview.Frame.Width,
					_webview.Frame.Height);

				_scroll.ContentSize = new CGSize (_scroll.Frame.Width, 
					_webview.Frame.Bottom + 10);
			};
			// ---------------------------------------

		}

		private void InitUI ()
		{			
			nfloat buttonFrameHeight = 120;
			_scroll = new UIScrollView (new CGRect (0, 0, View.Frame.Width, View.Frame.Height - buttonFrameHeight));
			View.AddSubview (_scroll);
			UIImageView _image = new UIImageView (new CGRect (45, 0, View.Frame.Width - 90, View.Frame.Width - 90));
			_image.Layer.MasksToBounds = false;
			_image.ClipsToBounds = true;
			_image.ContentMode = UIViewContentMode.ScaleAspectFit;
			MvxImageViewLoader _loader;
			_loader = new MvxImageViewLoader (() => _image);
			_scroll.AddSubview (_image);
			_image.UserInteractionEnabled = true;
			UITapGestureRecognizer imageTap = new UITapGestureRecognizer (() => {
				OpenImageViewer ();
			});
			_image.AddGestureRecognizer (imageTap);


			imageCount = new UIButton (UIButtonType.RoundedRect);
			imageCount.Frame = new CGRect (View.Frame.Width - 110, _image.Frame.Bottom - 50, 100, 34);
			imageCount.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			imageCount.Alpha = 0.8f;
			imageCount.Layer.BorderColor = UIColor.Clear.FromHexString (Styles.Colors.PlaceHolderColor).CGColor;
			imageCount.Layer.BorderWidth = 1.7f;
			imageCount.Layer.CornerRadius = imageCount.Frame.Height / 2.0f;
			imageCount.SetTitleColor (UIColor.White, UIControlState.Normal);
			imageCount.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			imageCount.TouchUpInside += delegate {
				OpenImageViewer ();
			};
			_scroll.AddSubview (imageCount);

			nfloat discountWidth = 60;
			nfloat discountHeight = 35;
			ProductDetailsDiscountView discount = 
				new ProductDetailsDiscountView (
					new CGRect (0,
						imageCount.Frame.Bottom - (imageCount.Frame.Height / 2.0f) - (discountHeight / 2.0f), 
						discountWidth, discountHeight));
			_scroll.AddSubview (discount);


			UILabel name = new UILabel (new CGRect (10, _image.Frame.Bottom + 10, View.Frame.Width - 130, 45));
//			name.Layer.BorderColor = UIColor.Red.CGColor;
//			name.Layer.BorderWidth = 1;
			name.Lines = 0;
			name.LineBreakMode = UILineBreakMode.WordWrap;
			name.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
			name.TextAlignment = UITextAlignment.Left;
			_scroll.AddSubview (name);

			UILabel pointTitle = new UILabel ();
			pointTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			pointTitle.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			pointTitle.Text = ApplicationStrings.Score;
			pointTitle.SizeToFit ();
			pointTitle.Frame = 
				new CGRect (
				View.Frame.Width - pointTitle.Frame.Width - 10,
				name.Frame.Bottom - (name.Frame.Height / 2.0f) - (pointTitle.Frame.Height / 2.0f),
				//name.Frame.Y,
				pointTitle.Frame.Width,
				pointTitle.Frame.Height);
			_scroll.AddSubview (pointTitle);

			UILabel points = new UILabel (
				                 new CGRect (name.Frame.Right + 10, 
					                 pointTitle.Frame.Bottom - 22, 
					                 pointTitle.Frame.Left - 2 - name.Frame.Right - 10, 
					                 22));
			points.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 20);
			points.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			points.TextAlignment = UITextAlignment.Right;
			_scroll.AddSubview (points);

			_idLabel = new UILabel (new CGRect (10, name.Frame.Bottom + 10, name.Frame.Width, 20));
			_idLabel.TextColor = UIColor.LightGray;
			_idLabel.Font = UIFont.SystemFontOfSize (13);
			_idLabel.TextAlignment = UITextAlignment.Left;
			_scroll.AddSubview (_idLabel);

			// Old Points ----------------------------
			UILabel oldPointTitle = new UILabel ();
			oldPointTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
			oldPointTitle.TextColor = UIColor.Gray;
			oldPointTitle.Text = ApplicationStrings.Score;
			oldPointTitle.SizeToFit ();
			oldPointTitle.Frame = 
				new CGRect (
				View.Frame.Width - oldPointTitle.Frame.Width - 10,
				points.Frame.Bottom + 10,
					//name.Frame.Y,
				oldPointTitle.Frame.Width,
				oldPointTitle.Frame.Height);
			_scroll.AddSubview (oldPointTitle);

			UILabel oldPoints = new UILabel (
				                    new CGRect (name.Frame.Right + 10, 
					                    oldPointTitle.Frame.Bottom - 18, 
					                    oldPointTitle.Frame.Left - 2 - name.Frame.Right - 10, 
					                    18));
			oldPoints.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 18);
			oldPoints.TextColor = UIColor.Gray;
			oldPoints.TextAlignment = UITextAlignment.Right;
			_scroll.AddSubview (oldPoints);

			CrossLinesView cross = 
				new CrossLinesView (new CGRect (
					oldPoints.Frame.X + 15, 
					oldPoints.Frame.Y, 
					oldPointTitle.Frame.Right - oldPoints.Frame.X - 15 + 2, 
					oldPoints.Frame.Height + 4
				));
			_scroll.AddSubview (cross);
			// ---------------------------------------

			// Webview
			_webview = new BindableUIWebView (new CGRect (0, _idLabel.Frame.Bottom + 10,
				View.Frame.Width, 100));
			_webview.LoadFinished += HTMLContentLoaded;
			_webview.BackgroundColor = UIColor.White;
			_scroll.AddSubview (_webview);

			// Order button
			UIButton buy = new UIButton (UIButtonType.System);
			buy.Frame = new CGRect (30, View.Frame.Height - buttonFrameHeight + 5, View.Frame.Width - 60, 40);
			buy.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			buy.SetTitleColor (UIColor.White, UIControlState.Normal);
			buy.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 20);
			buy.TintColor = UIColor.White;
			buy.DropShadowDependingOnBGColor ();
			buy.SetTitle (ApplicationStrings.Buy, UIControlState.Normal);
			buy.Layer.CornerRadius = buy.Frame.Height / 2.0f;
			buy.TouchUpInside += BuyClick;
			View.AddSubview (buy);




			// Bindings
			var set = this.CreateBindingSet<ProductDetailsViewController, iCatalogDetailViewModel> ();
			set.Bind (_loader).To (vm => vm.CurrentImageUrl);
			set.Bind (discount).For (x => x.Title).To (vm => vm.ProductDiscountPercent);
			set.Bind (discount).For (x => x.Hidden).To (vm => vm.ProductDiscountPercent).WithConversion ("ProductPercent");
			set.Bind (this).For (i => i.ImageCountName).To (vm => vm.ImageCountName);
			set.Bind (name).To (vm => vm.ProductName);
			set.Bind (points).To (vm => vm.ProductPrice);
			set.Bind (_idLabel).To (vm => vm.CatalogID);
			set.Bind (oldPoints).To (vm => vm.ProductOldPrice);
			set.Bind (oldPoints).For (x => x.Hidden).To (vm => vm.ProductDiscountPercent).WithConversion ("ProductPercent");
			set.Bind (oldPointTitle).For (x => x.Hidden).To (vm => vm.ProductDiscountPercent).WithConversion ("ProductPercent");
			set.Bind (cross).For (x => x.Hidden).To (vm => vm.ProductDiscountPercent).WithConversion ("ProductPercent");
			set.Bind (_webview).For (x => x.Text).To (vm => vm.ProductDescription);
			set.Bind (this).For (x => x.UserDiscounts).To (vm => vm.UserDiscounts);
			set.Apply (); 
		}

		private void OpenImageViewer ()
		{
			var vc = new ImageViewerController (ViewModel.ProductName, ViewModel.ImageUrls,
				         UIPageViewControllerTransitionStyle.Scroll,
				         UIPageViewControllerNavigationOrientation.Horizontal,
				         UIPageViewControllerSpineLocation.Min);
			NavigationController.PresentModalViewController (vc, true);
		}

		#endregion

		#region Events

		private DeliveryMethodWrapper _deliveryMethodWrapper;
		private ChooseDeliveryMethod _chooseDeliveryMethod;

		void BuyClick (object sender, EventArgs e)
		{
			if (ViewModel.DeliveryMethods == null)
				return;
			// In case there is only one method user doesn't have to choose it.
			if (ViewModel.DeliveryMethods.Count == 1) {
				ViewModel.GoToBuyProduct (ViewModel.DeliveryMethods.FirstOrDefault ());
			}
			// In case there is more than one method user must first choose delivery method and then buy product
			if (ViewModel.DeliveryMethods.Count > 1) {
				_deliveryMethodWrapper = new DeliveryMethodWrapper (APP.Window.Frame);
				_chooseDeliveryMethod = new ChooseDeliveryMethod (ViewModel.DeliveryMethods);
				_chooseDeliveryMethod.DeliveryMethodSelected += delegate(object delButton, DeliveryMethod method) {
					_deliveryMethodWrapper.Close ();
					ViewModel.GoToBuyProduct (method);
				};
				_deliveryMethodWrapper.AddSubview (_chooseDeliveryMethod);
				_deliveryMethodWrapper.Alpha = 0;
				APP.Window.AddSubview (_deliveryMethodWrapper);
				UIView.Animate (0.3f, () => {
					_deliveryMethodWrapper.Alpha = 1f;
				});	
			}
		}

		void HTMLContentLoaded (object sender, EventArgs e)
		{
			BindableUIWebView _webView = sender as BindableUIWebView;
			var height = _webView.EvaluateJavascript ("document.body.scrollHeight");
			var frame = _webView.Frame;
			frame.Height = (float)Convert.ToDouble (height);
			_webView.Frame = frame;
					
			_scroll.ContentSize = new CGSize (View.Frame.Width, _webView.Frame.Bottom);

			CGSize contentSize = _webView.ScrollView.ContentSize;
			CGSize viewSize = View.Bounds.Size;
			nfloat rw = viewSize.Width / contentSize.Width;

			_webView.ScrollView.MinimumZoomScale = rw;
			_webView.ScrollView.MaximumZoomScale = rw;
			_webView.ScrollView.ZoomScale = rw;
		}

		#endregion
	}
}

