using System;
using UIKit;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core.ViewModels.iOSSpecific;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Touch.Helpers.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Kunicardus.Core;
using System.Linq;

namespace Kunicardus.Touch
{
	public class BuyProductViewController : BaseMvxViewController
	{
		
		#region Props

		public new iBuyProductViewModel ViewModel {
			get { return (iBuyProductViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		private List<PaymentInfoDTO> _paymentInfos;

		public List<PaymentInfoDTO>  PaymentInfos { 
			get{ return _paymentInfos; }
			set {
				_paymentInfos = value;
				UIApplication.SharedApplication.InvokeOnMainThread (() => {
					InitPaymendDetailsList ();
				});
			}
		}

		private bool _buySuccess;

		public bool BuySuccess {
			get {
				return _buySuccess;
			}
			set {
				_buySuccess = value;
				if (value) {
					var alert = new UIAlertView ("", "თქვენ წარმატებით შეიძინეთ პროდუქტი!", null, "გასაგებია");
					alert.Clicked += delegate {
						NavigationController.PopViewController (true);
					};
					alert.Show ();
				}
			
			}
		}

		#endregion

		#region UI

		private UIScrollView _scroll;
		private KeyboardTopBar _keyboardBar;
		private UIView _finalBlockContainer;
		private UIView _onlinePaymentDetails;

		#endregion

		#region Ctors

		public BuyProductViewController ()
		{
			HideMenuIcon = true;
			ScrollViewOnKeyboardShow = true;
		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBar.Translucent = false;
			NavigationController.NavigationBarHidden = false;
			View.BackgroundColor = UIColor.Clear.FromHexString ("#ebebeb");
			this.Title = ApplicationStrings.Buy;
			InitUI ();
			this.CreateBinding (this).For (x => x.BuySuccess).To ((iBuyProductViewModel vm) => vm.BuySuccess).Apply ();
		}

		public override void ViewWillAppear (bool animated)
		{
			try {
				NavigationController.NavigationBarHidden = false;
			} catch {
			}
			base.ViewWillAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			try {
				NavigationController.NavigationBarHidden = true;
			} catch {
			}
			base.ViewWillDisappear (animated);
		}

		#endregion

		#region Methods

		private void InitPaymendDetailsList ()
		{
			// Removing old data
			foreach (var item in _onlinePaymentDetails.Subviews) {
				item.RemoveFromSuperview ();
			}
			_onlinePaymentDetails.Frame = new CGRect (_onlinePaymentDetails.Frame.X, _onlinePaymentDetails.Frame.Y, View.Frame.Width, 0);
			nfloat paddingLeft = 10f;
			// Filling data
			if (_paymentInfos != null) {
				UIView _lastLine = null;
				nfloat tmpTop = 5;
				foreach (var info in _paymentInfos) {
					UILabel title = new UILabel (new CGRect (paddingLeft, tmpTop, (View.Frame.Width - (2 * paddingLeft)) / 2 - 5, 20));
					title.TextColor = UIColor.Black;
					title.LineBreakMode = UILineBreakMode.WordWrap;
					title.Lines = 0;
					title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
					title.Text = info.Name;
					title.SizeToFit ();
					title.Frame = new CGRect (paddingLeft, tmpTop, (View.Frame.Width - (2 * paddingLeft)) / 2 - 5, title.Frame.Height); 
					_onlinePaymentDetails.AddSubview (title);

					UILabel value = new UILabel (new CGRect (title.Frame.Right + 10, tmpTop, (View.Frame.Width - (2 * paddingLeft)) / 2 - 5, 20));
					value.TextAlignment = UITextAlignment.Right;
					value.TextColor = UIColor.Black;
					value.LineBreakMode = UILineBreakMode.WordWrap;
					value.Lines = 0;
					value.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
					value.Text = info.Value;
					value.SizeToFit ();
					value.Frame = new CGRect (title.Frame.Right + 10, tmpTop, (View.Frame.Width - (2 * paddingLeft)) / 2 - 5, value.Frame.Height);
					_onlinePaymentDetails.AddSubview (value);

					UIView line = new UIView (new CGRect (paddingLeft, (Math.Max (title.Frame.Bottom, value.Frame.Bottom)) + 2, View.Frame.Width - (2 * paddingLeft), 1.5f));
					line.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
					_onlinePaymentDetails.AddSubview (line);
					_onlinePaymentDetails.Frame = 
						new CGRect (_onlinePaymentDetails.Frame.X, _onlinePaymentDetails.Frame.Y, View.Frame.Width, line.Frame.Bottom + 5);
					tmpTop = line.Frame.Bottom + 2;
					_lastLine = line;
				}
				if (_lastLine != null)
					_lastLine.RemoveFromSuperview ();
			}

			// adjusting screen dimensions
			_finalBlockContainer.Frame = new CGRect (_finalBlockContainer.Frame.X, _onlinePaymentDetails.Frame.Bottom + 10,
				_finalBlockContainer.Frame.Width, _finalBlockContainer.Frame.Height);
			_scroll.ContentSize = new CGSize (View.Frame.Width, _finalBlockContainer.Frame.Bottom + GetStatusBarHeight () + 10);

		}

		private void InitUI ()
		{
			_keyboardBar = new KeyboardTopBar ();
			string titleColor = "#757575";
			_scroll = new UIScrollView (new CGRect (0, 0, View.Frame.Width, View.Frame.Height));

			#region Top View
			UIView topView = new UIView ();
			topView.BackgroundColor = UIColor.White;

			UIImageView image = new UIImageView (new CGRect (5, 5, 80, 80));
			image.Layer.MasksToBounds = false;
			image.ClipsToBounds = true;
			image.ContentMode = UIViewContentMode.ScaleAspectFit;
			image.Image = ImageHelper.FromUrl (ViewModel.CurrentImageUrl);
			topView.AddSubview (image);

			UILabel title = new UILabel (new CGRect (image.Frame.Right + 10, image.Frame.Top, View.Frame.Width - image.Frame.Right - 20, 0));
			title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
			title.TextColor = UIColor.Clear.FromHexString (titleColor);
			title.Text = ViewModel.ProductName;
			title.LineBreakMode = UILineBreakMode.WordWrap;
			title.Lines = 0;
			title.SizeToFit ();
			title.Frame = new CGRect (image.Frame.Right + 10, image.Frame.Top, title.Frame.Width, title.Frame.Height);
			topView.AddSubview (title);

			UIView line = new UIView (new CGRect (title.Frame.Left - 2, title.Frame.Bottom + 5, title.Frame.Width + 2, 1.5f));
			line.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
			topView.AddSubview (line);

			UILabel points = new UILabel (
				                 new CGRect (title.Frame.Left, line.Frame.Bottom + 5, 100, 0)
			                 );
			points.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 20);
			points.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			points.Text = ViewModel.ProductPrice.ToString ();
			points.SizeToFit ();
			points.Frame = new CGRect (title.Frame.Left, line.Frame.Bottom + 5, points.Frame.Width, points.Frame.Height);
			topView.AddSubview (points);

			UILabel pointTitle = new UILabel ();
			pointTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			pointTitle.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			pointTitle.Text = ApplicationStrings.Score;
			pointTitle.SizeToFit ();
			pointTitle.Frame = new CGRect (points.Frame.Right + 1, points.Frame.Y + 6, pointTitle.Frame.Width, pointTitle.Frame.Height);
			topView.AddSubview (pointTitle);

			topView.Frame = new CGRect (0, 0, View.Frame.Width, Math.Max (pointTitle.Frame.Bottom, image.Frame.Bottom) + 10);
			_scroll.AddSubview (topView);
			#endregion



			switch (ViewModel.DeliveryMethodId) {
			//ადგილზე მიტანა - 3
			case 3:
				{
					InitDeliveryView (topView.Frame.Bottom);
					break;
				}
			//პარტნიორი ორგანიზაციიდან გატანა 2
			//სერვის ცენტრიდან 1
			//შოუ რუმიდან 5
			case 2:
			case 1:
			case 5:
				{
					InitTakeFromServiceCenter (topView.Frame.Bottom);
					break;
				}
			//მომენტალური მიღება 4
			//მომენტალური გადახდა 10
			case 10:
			case 4:
				{
					if (ViewModel.ProductTypeID == 5) {
						InitMobilePaymentView (topView.Frame.Bottom);
					} else {
						InitOnlinePaymentView (topView.Frame.Bottom);
					}
					break;
				}
			}


			View.AddSubview (_scroll);
		}

		private void InitOnlinePaymentView (nfloat topOfView)
		{
			string titleColor = "#757575";
			nfloat paddingLeft = 10f;

			UILabel title = new UILabel (new CGRect (
				                paddingLeft, topOfView + 10, View.Frame.Width - (2 * paddingLeft), 20));
			title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15);
			title.TextColor = UIColor.Clear.FromHexString (titleColor);
			title.Text = ViewModel.IndentifierTitle;		
			title.SizeToFit ();
			_scroll.AddSubview (title);

			UITextField identifier = new UITextField (
				                         new CGRect (paddingLeft, title.Frame.Bottom + 7, View.Frame.Width - 150f, 30));

			identifier.InputAccessoryView = _keyboardBar;
			_keyboardBar.OnDone += delegate {
				identifier.ResignFirstResponder ();
			};
			StyleUITextField (identifier);
			_scroll.AddSubview (identifier);

			UIButton check = new UIButton (UIButtonType.System);
			check.Frame = new CGRect (identifier.Frame.Right + 5, title.Frame.Bottom + 5, 125f, 30);
			check.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			check.SetTitleColor (UIColor.White, UIControlState.Normal);
			check.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
			check.TintColor = UIColor.White;
			check.DropShadowDependingOnBGColor ();
			check.SetTitle (ApplicationStrings.Check, UIControlState.Normal);
			check.Layer.CornerRadius = 5;
			_scroll.AddSubview (check); 

			this.CreateBinding (identifier).To ((iBuyProductViewModel vm) => vm.OnlinePaymentIdentifier).Apply ();
			this.CreateBinding (check).To ((iBuyProductViewModel vm) => vm.CheckInfoClickedCommand).Apply ();
			this.CreateBinding (this).For (x => x.PaymentInfos).To ((iBuyProductViewModel vm) => vm.PaymentInfos).Apply ();

			_onlinePaymentDetails = new UIView (new CGRect (0, identifier.Frame.Bottom + 10, View.Frame.Width, 0));
			_scroll.AddSubview (_onlinePaymentDetails);

			InitFinalPriceAndBuyBlock_ForOnlinePayments (paddingLeft, 
				_onlinePaymentDetails.Frame.Bottom + 10, 
				View.Frame.Width - (2 * paddingLeft));
			
		}


		private void InitMobilePaymentView (nfloat topOfView)
		{
			string titleColor = "#757575";
			nfloat paddingLeft = 10f;

			UILabel mobileTitle = new UILabel (new CGRect (
				                      paddingLeft, topOfView + 10, View.Frame.Width - (2 * paddingLeft), 20));
			mobileTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15);
			mobileTitle.TextColor = UIColor.Clear.FromHexString (titleColor);
			mobileTitle.Text = ApplicationStrings.MobilePhoneNumber;		
			mobileTitle.SizeToFit ();
			_scroll.AddSubview (mobileTitle);

			UILabel mobilePhoneDesc = new UILabel (new CGRect (0, 0, View.Frame.Width - (2 * paddingLeft), 20));
			mobilePhoneDesc.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
			mobilePhoneDesc.LineBreakMode = UILineBreakMode.WordWrap;
			mobilePhoneDesc.Lines = 0;
			mobilePhoneDesc.TextColor = UIColor.Clear.FromHexString (titleColor);
			mobilePhoneDesc.Text = ApplicationStrings.MobilePhoneNumberDesc;		
			mobilePhoneDesc.SizeToFit ();
			mobilePhoneDesc.Frame = new CGRect (paddingLeft, mobileTitle.Frame.Bottom + 5, mobilePhoneDesc.Frame.Width, mobilePhoneDesc.Frame.Height);
			_scroll.AddSubview (mobilePhoneDesc);

			UITextField mobile = new UITextField (
				                     new CGRect (paddingLeft, mobilePhoneDesc.Frame.Bottom + 5, View.Frame.Width - (2 * paddingLeft), 30));

			mobile.InputAccessoryView = _keyboardBar;
			_keyboardBar.OnDone += delegate {
				mobile.ResignFirstResponder ();
			};
			mobile.Placeholder = "5XXXXXXXX";
			mobile.KeyboardType = UIKeyboardType.NumberPad;
			mobile.ShouldChangeCharacters = (textField, range, replacementString) => {
				var newLength = textField.Text.Length + replacementString.Length - range.Length;

				return newLength <= 9;
			};
			mobile.EditingChanged += delegate {				
				if (mobile.IsFirstResponder && mobile.Text.Length == 9) {
					mobile.ResignFirstResponder ();
				}
			};		
			StyleUITextField (mobile);
			_scroll.AddSubview (mobile);

			InitFinalPriceAndBuyBlock (paddingLeft, mobile.Frame.Bottom + 10, View.Frame.Width - (2 * paddingLeft));

			this.CreateBinding (mobile).To ((iBuyProductViewModel vm) => vm.PhoneNumber).Apply ();
		}

		private void InitTakeFromServiceCenter (nfloat topOfView)
		{
			string titleColor = "#757575";
			nfloat paddingLeft = 10f;

			UILabel selectSCTitle = new UILabel (new CGRect (
				                        paddingLeft, topOfView + 10, View.Frame.Width - (2 * paddingLeft), 20));
			selectSCTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15);
			selectSCTitle.TextColor = UIColor.Clear.FromHexString (titleColor);
			selectSCTitle.Text = ApplicationStrings.SelectServiceCenter;		
			selectSCTitle.SizeToFit ();
			_scroll.AddSubview (selectSCTitle);

			UITextField sc = new UITextField (
				                 new CGRect (paddingLeft, selectSCTitle.Frame.Bottom + 5, View.Frame.Width - (2 * paddingLeft) - 35, 30));
			StyleUITextField (sc);
			ViewModel.SelectedSCenter = ViewModel.ServiceCenters.FirstOrDefault (x => x.ID == -1);
			sc.Text = ViewModel.SelectedSCenter.Name;
			sc.ShouldChangeCharacters = (textField, range, replacementString) => {
				return false;
			};
			_scroll.AddSubview (sc);

			UIImageView downArrow = new UIImageView (UIImage.FromBundle ("arrow_down_32").ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal));
			downArrow.SizeToFit ();
			downArrow.Frame = new CGRect (sc.Frame.Right + 3, sc.Frame.Top, downArrow.Frame.Width, downArrow.Frame.Height);
			_scroll.AddSubview (downArrow);
			SetupPicker (sc, downArrow);

			UIView line = new UIView (new CGRect (sc.Frame.Left, sc.Frame.Bottom + 10, View.Frame.Width - (2 * paddingLeft), 1.5f));
			line.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
			_scroll.AddSubview (line);

			UILabel responsiblePersonTitle = new UILabel (new CGRect (
				                                 paddingLeft, line.Frame.Bottom + 10, View.Frame.Width - (2 * paddingLeft), 20));
			responsiblePersonTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15);
			responsiblePersonTitle.TextColor = UIColor.Clear.FromHexString (titleColor);
			responsiblePersonTitle.Text = ApplicationStrings.ResponsiblePerson;		
			responsiblePersonTitle.SizeToFit ();
			_scroll.AddSubview (responsiblePersonTitle);

			UILabel respPersonDesc = new UILabel (new CGRect (0, 0, View.Frame.Width - (2 * paddingLeft), 20));
			respPersonDesc.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
			respPersonDesc.LineBreakMode = UILineBreakMode.WordWrap;
			respPersonDesc.Lines = 0;
			respPersonDesc.TextColor = UIColor.Clear.FromHexString (titleColor);
			respPersonDesc.Text = ApplicationStrings.ResponsiblePersonDesc;		
			respPersonDesc.SizeToFit ();
			respPersonDesc.Frame = new CGRect (paddingLeft, responsiblePersonTitle.Frame.Bottom + 5, respPersonDesc.Frame.Width, respPersonDesc.Frame.Height);
			_scroll.AddSubview (respPersonDesc);

			UITextField firstName = new UITextField (
				                        new CGRect (paddingLeft, respPersonDesc.Frame.Bottom + 5, View.Frame.Width - (2 * paddingLeft), 30));

			firstName.InputAccessoryView = new KeyboardTopBar ();
			((KeyboardTopBar)firstName.InputAccessoryView).OnDone += delegate {
				firstName.ResignFirstResponder ();
			};
			StyleUITextField (firstName);
			_scroll.AddSubview (firstName);

			UITextField lastName = new UITextField (
				                       new CGRect (paddingLeft, firstName.Frame.Bottom + 5, View.Frame.Width - (2 * paddingLeft), 30));

			lastName.InputAccessoryView = new KeyboardTopBar ();
			((KeyboardTopBar)lastName.InputAccessoryView).OnDone += delegate {
				lastName.ResignFirstResponder ();
			};
			StyleUITextField (lastName);
			_scroll.AddSubview (lastName);

			UITextField personalId = new UITextField (
				                         new CGRect (paddingLeft, lastName.Frame.Bottom + 5, View.Frame.Width - (2 * paddingLeft), 30));

			personalId.InputAccessoryView = new KeyboardTopBar ();
			((KeyboardTopBar)personalId.InputAccessoryView).OnDone += delegate {
				personalId.ResignFirstResponder ();
			};
			StyleUITextField (personalId);
			_scroll.AddSubview (personalId);

			// prev next fields -----------------------------------
			((KeyboardTopBar)firstName.InputAccessoryView).NextEnabled = true;
			((KeyboardTopBar)firstName.InputAccessoryView).PreviousEnabled = false;

			((KeyboardTopBar)lastName.InputAccessoryView).NextEnabled = true;
			((KeyboardTopBar)lastName.InputAccessoryView).PreviousEnabled = true;

			((KeyboardTopBar)personalId.InputAccessoryView).NextEnabled = false;
			((KeyboardTopBar)personalId.InputAccessoryView).PreviousEnabled = true;

			((KeyboardTopBar)firstName.InputAccessoryView).OnNext += delegate {
				firstName.ResignFirstResponder ();
				lastName.BecomeFirstResponder ();
			};

			((KeyboardTopBar)lastName.InputAccessoryView).OnNext += delegate {
				lastName.ResignFirstResponder ();
				personalId.BecomeFirstResponder ();
			};
			((KeyboardTopBar)lastName.InputAccessoryView).OnPrev += delegate {
				lastName.ResignFirstResponder ();
				firstName.BecomeFirstResponder ();
			};

			((KeyboardTopBar)personalId.InputAccessoryView).OnPrev += delegate {
				personalId.ResignFirstResponder ();
				lastName.BecomeFirstResponder ();
			};
			// -----------------------------------
			InitFinalPriceAndBuyBlock (paddingLeft, personalId.Frame.Bottom + 10, View.Frame.Width - (2 * paddingLeft));

			this.CreateBinding (firstName).To ((iBuyProductViewModel vm) => vm.FirstName).Apply ();
			this.CreateBinding (lastName).To ((iBuyProductViewModel vm) => vm.LastName).Apply ();
			this.CreateBinding (personalId).To ((iBuyProductViewModel vm) => vm.PersonalNumber).Apply ();
		}

		private void InitDeliveryView (nfloat topOfView)
		{
			string titleColor = "#757575";
			nfloat paddingLeft = 10f;
			UILabel addressTitle = new UILabel (new CGRect (
				                       paddingLeft, topOfView + 10, View.Frame.Width - (2 * paddingLeft), 20));
			addressTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15);
			addressTitle.TextColor = UIColor.Clear.FromHexString (titleColor);
			addressTitle.Text = ApplicationStrings.Address;		
			addressTitle.SizeToFit ();
			_scroll.AddSubview (addressTitle);

			UITextField address = new UITextField (
				                      new CGRect (paddingLeft, addressTitle.Frame.Bottom + 5, View.Frame.Width - (2 * paddingLeft), 30));

			address.InputAccessoryView = _keyboardBar;
			_keyboardBar.OnDone += delegate {
				address.ResignFirstResponder ();
			};
			StyleUITextField (address);
			_scroll.AddSubview (address);

			UILabel noteLabel = new UILabel (new CGRect (0, 0, View.Frame.Width - (2 * paddingLeft), 20));
			noteLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
			noteLabel.LineBreakMode = UILineBreakMode.WordWrap;
			noteLabel.Lines = 0;
			noteLabel.TextColor = UIColor.Clear.FromHexString (titleColor);
			noteLabel.Text = ScrubHtml (ViewModel.Note);		
			noteLabel.SizeToFit ();
			noteLabel.Frame = new CGRect (paddingLeft, address.Frame.Bottom + 5, noteLabel.Frame.Width, noteLabel.Frame.Height);
			_scroll.AddSubview (noteLabel);

			InitFinalPriceAndBuyBlock (paddingLeft, noteLabel.Frame.Bottom + 10, View.Frame.Width - (2 * paddingLeft));

			// Bindings
			this.CreateBinding (address).To ((iBuyProductViewModel vm) => vm.UserAddress).Apply ();

		}

		private void InitFinalPriceAndBuyBlock_ForOnlinePayments (nfloat x, nfloat yy, nfloat width)
		{
			_finalBlockContainer = new UIView (new CGRect (0, yy, View.Frame.Width, 0));
			string titleColor = "#757575";
			UIView line = new UIView (new CGRect (x, 0, width, 1.5f));
			line.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
			_finalBlockContainer.AddSubview (line);

			nfloat top = line.Frame.Bottom + 20;

			// Discounts -----
			if (ViewModel.UserDiscounts != null && ViewModel.UserDiscounts.Count > 0) {

				UIImageView image = new UIImageView (ImageHelper.MaxResizeImage (UIImage.FromBundle ("gift"), 0, 26));
				image.SizeToFit ();
				image.Frame = new CGRect (x, top, image.Frame.Width, image.Frame.Height);
				_finalBlockContainer.AddSubview (image);

				UILabel uHaveDiscounts = new UILabel (
					                         new CGRect (image.Frame.Right + 10, top, width - image.Frame.Width - 20, image.Frame.Height));
				uHaveDiscounts.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
				uHaveDiscounts.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
				uHaveDiscounts.Text = ApplicationStrings.UHaveDiscounts;
				_finalBlockContainer.AddSubview (uHaveDiscounts);

				UIView l = new UIView (new CGRect (x, image.Frame.Bottom + 5, width, 0.65f));
				l.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
				_finalBlockContainer.AddSubview (l);

				top = l.Frame.Bottom + 10;
				_radioButtons = new List<RadioButton> ();
				foreach (var d in ViewModel.UserDiscounts) {
					UILabel parcentLabel = new UILabel ();
					parcentLabel.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
					parcentLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 18);
					parcentLabel.Text = string.Format ("{0}%", d.DiscountPercent);
					parcentLabel.SizeToFit ();
					parcentLabel.Frame = new CGRect (x, top, parcentLabel.Frame.Width, parcentLabel.Frame.Height);
					_scroll.AddSubview (parcentLabel);

					UILabel desc = new UILabel ();
					desc.TextColor = UIColor.Black;
					desc.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
					desc.Text = d.DiscountDescription;
					desc.SizeToFit ();
					desc.Frame = 
						new CGRect (parcentLabel.Frame.Right + 1, 
						parcentLabel.Frame.Bottom - desc.Frame.Height - 2, 

						line.Frame.Right - parcentLabel.Frame.Right - 2 - parcentLabel.Frame.Height - 5,
						desc.Frame.Height);
					_finalBlockContainer.AddSubview (desc);



					RadioButton radioButton = new RadioButton (new CGRect (
						                          line.Frame.Right - parcentLabel.Frame.Height - 5, 
						                          parcentLabel.Frame.Top, 
						                          parcentLabel.Frame.Height, 
						                          parcentLabel.Frame.Height));
					radioButton.Font = UIFont.SystemFontOfSize (30);
					radioButton.Checked += RadioButton_Checked;
					radioButton.Value = d.DiscountID.ToString ();
					_finalBlockContainer.AddSubview (radioButton);
					_radioButtons.Add (radioButton);

					top = parcentLabel.Frame.Bottom + 10;
				}

				top += 20;
			}
			// ---------------


			//---
			UILabel finalPointTitle = new UILabel ();
			finalPointTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			finalPointTitle.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			finalPointTitle.Text = ApplicationStrings.Score;
			finalPointTitle.SizeToFit ();
			finalPointTitle.Frame = new CGRect (line.Frame.Right - finalPointTitle.Frame.Width, 
				top, 
				finalPointTitle.Frame.Width, finalPointTitle.Frame.Height);
			_finalBlockContainer.AddSubview (finalPointTitle);

			UILabel finalpoints = new UILabel ();
			finalpoints.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 20);
			finalpoints.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			finalpoints.Text = ViewModel.ProductPrice.ToString ();
			finalpoints.SizeToFit ();
			finalpoints.TextAlignment = UITextAlignment.Right;
			finalpoints.Frame = new CGRect (finalPointTitle.Frame.Left - (2 * finalpoints.Frame.Width) - 1, 
				finalPointTitle.Frame.Bottom - finalpoints.Frame.Height + 2, 2 * finalpoints.Frame.Width, finalpoints.Frame.Height);
			_finalBlockContainer.AddSubview (finalpoints);

			UILabel finalPointDesc = new UILabel ();
			finalPointDesc.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			finalPointDesc.TextColor = UIColor.Clear.FromHexString (titleColor);
			finalPointDesc.Text = ApplicationStrings.FinalPrice;
			finalPointDesc.SizeToFit ();
			finalPointDesc.Frame = new CGRect (finalpoints.Frame.Left - finalPointDesc.Frame.Width - 1, 
				finalpoints.Frame.Bottom - finalPointDesc.Frame.Height, 
				finalPointDesc.Frame.Width, finalPointDesc.Frame.Height);
			_finalBlockContainer.AddSubview (finalPointDesc);

			UIButton buy = new UIButton (UIButtonType.System);
			buy.Frame = new CGRect (30, finalPointDesc.Frame.Bottom + 20, View.Frame.Width - 60, 40);
			buy.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			buy.SetTitleColor (UIColor.White, UIControlState.Normal);
			buy.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 20);
			buy.TintColor = UIColor.White;
			buy.DropShadowDependingOnBGColor ();
			buy.SetTitle (ApplicationStrings.Buy, UIControlState.Normal);
			buy.Layer.CornerRadius = buy.Frame.Height / 2.0f;
			_finalBlockContainer.AddSubview (buy);
			_finalBlockContainer.Frame = new CGRect (_finalBlockContainer.Frame.X,
				_finalBlockContainer.Frame.Y, _finalBlockContainer.Frame.Width, buy.Frame.Bottom + 1);

			_scroll.AddSubview (_finalBlockContainer);

			_scroll.ContentSize = new CGSize (View.Frame.Width, _finalBlockContainer.Frame.Bottom + GetStatusBarHeight () + 10);

			this.CreateBinding (finalpoints).To ((iBuyProductViewModel vm) => vm.Score).Apply ();
			this.CreateBinding (buy).To ((iBuyProductViewModel vm) => vm.BuyProductCommand).Apply ();
		}

		private List<RadioButton> _radioButtons;

		private void InitFinalPriceAndBuyBlock (nfloat x, nfloat y, nfloat width)
		{
			
			string titleColor = "#757575";
			UIView line = new UIView (new CGRect (x, y, width, 1.5f));
			line.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
			_scroll.AddSubview (line);
			nfloat top = line.Frame.Bottom + 20;

			// Discounts -----
			if (ViewModel.UserDiscounts != null && ViewModel.UserDiscounts.Count > 0) {

				UIImageView image = new UIImageView (ImageHelper.MaxResizeImage (UIImage.FromBundle ("gift"), 0, 26));
				image.SizeToFit ();
				image.Frame = new CGRect (x, top, image.Frame.Width, image.Frame.Height);
				_scroll.AddSubview (image);

				UILabel uHaveDiscounts = new UILabel (
					                         new CGRect (image.Frame.Right + 10, top, width - image.Frame.Width - 20, image.Frame.Height));
				uHaveDiscounts.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
				uHaveDiscounts.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
				uHaveDiscounts.Text = ApplicationStrings.UHaveDiscounts;
				_scroll.AddSubview (uHaveDiscounts);

				UIView l = new UIView (new CGRect (x, image.Frame.Bottom + 5, width, 0.65f));
				l.BackgroundColor = UIColor.Clear.FromHexString ("#e2e3e3");
				_scroll.AddSubview (l);

				top = l.Frame.Bottom + 10;
				_radioButtons = new List<RadioButton> ();
				foreach (var d in ViewModel.UserDiscounts) {
					UILabel parcentLabel = new UILabel ();
					parcentLabel.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
					parcentLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 18);
					parcentLabel.Text = string.Format ("{0}%", d.DiscountPercent);
					parcentLabel.SizeToFit ();
					parcentLabel.Frame = new CGRect (x, top, parcentLabel.Frame.Width, parcentLabel.Frame.Height);
					_scroll.AddSubview (parcentLabel);

					UILabel desc = new UILabel ();
					desc.TextColor = UIColor.Black;
					desc.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 12);
					desc.Text = d.DiscountDescription;
					desc.SizeToFit ();
					desc.Frame = 
						new CGRect (parcentLabel.Frame.Right + 1, 
						parcentLabel.Frame.Bottom - desc.Frame.Height - 2, 

						line.Frame.Right - parcentLabel.Frame.Right - 2 - parcentLabel.Frame.Height - 5,
						desc.Frame.Height);
					_scroll.AddSubview (desc);



					RadioButton radioButton = new RadioButton (new CGRect (
						                          line.Frame.Right - parcentLabel.Frame.Height - 5, 
						                          parcentLabel.Frame.Top, 
						                          parcentLabel.Frame.Height, 
						                          parcentLabel.Frame.Height));
					radioButton.Font = UIFont.SystemFontOfSize (30);
					radioButton.Checked += RadioButton_Checked;
					radioButton.Value = d.DiscountID.ToString ();
					_scroll.AddSubview (radioButton);
					_radioButtons.Add (radioButton);

					top = parcentLabel.Frame.Bottom + 10;
				}

				top += 20;
			}
			// ---------------



			//---
			UILabel finalPointTitle = new UILabel ();
			finalPointTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			finalPointTitle.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			finalPointTitle.Text = ApplicationStrings.Score;
			finalPointTitle.SizeToFit ();
			finalPointTitle.Frame = new CGRect (line.Frame.Right - finalPointTitle.Frame.Width, 
				top, 
				finalPointTitle.Frame.Width, finalPointTitle.Frame.Height);
			_scroll.AddSubview (finalPointTitle);

			UILabel finalpoints = new UILabel ();
			finalpoints.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 20);
			finalpoints.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			finalpoints.Text = ViewModel.ProductPrice.ToString ();
			finalpoints.SizeToFit ();
			finalpoints.TextAlignment = UITextAlignment.Right;
			finalpoints.Frame = new CGRect (finalPointTitle.Frame.Left - (2 * finalpoints.Frame.Width) - 1, 
				finalPointTitle.Frame.Bottom - finalpoints.Frame.Height + 2, 2 * finalpoints.Frame.Width, finalpoints.Frame.Height);
			_scroll.AddSubview (finalpoints);

			UILabel finalPointDesc = new UILabel ();
			finalPointDesc.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			finalPointDesc.TextColor = UIColor.Clear.FromHexString (titleColor);
			finalPointDesc.Text = ApplicationStrings.FinalPrice;
			finalPointDesc.SizeToFit ();
			finalPointDesc.Frame = new CGRect (finalpoints.Frame.Left - finalPointDesc.Frame.Width - 1, 
				finalpoints.Frame.Bottom - finalPointDesc.Frame.Height, 
				finalPointDesc.Frame.Width, finalPointDesc.Frame.Height);
			_scroll.AddSubview (finalPointDesc);

			UIButton buy = new UIButton (UIButtonType.System);
			buy.Frame = new CGRect (30, finalPointDesc.Frame.Bottom + 20, View.Frame.Width - 60, 40);
			buy.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			buy.SetTitleColor (UIColor.White, UIControlState.Normal);
			buy.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 20);
			buy.TintColor = UIColor.White;
			buy.DropShadowDependingOnBGColor ();
			buy.SetTitle (ApplicationStrings.Buy, UIControlState.Normal);
			buy.Layer.CornerRadius = buy.Frame.Height / 2.0f;
			_scroll.AddSubview (buy);

			_scroll.ContentSize = new CGSize (View.Frame.Width, buy.Frame.Bottom + GetStatusBarHeight () + 10);


			this.CreateBinding (finalpoints).To ((iBuyProductViewModel vm) => vm.ProductPrice).Apply ();
			this.CreateBinding (buy).To ((iBuyProductViewModel vm) => vm.BuyProductCommand).Apply ();
		}

		void RadioButton_Checked (object sender, EventArgs e)
		{
			var rb = sender as RadioButton;
			bool isChecked = rb.IsChecked;
			foreach (var item in _radioButtons) {
				item.IsChecked = false;
			}
			rb.IsChecked = isChecked;
			if (isChecked) {
				ViewModel.SelectedDiscount = ViewModel.UserDiscounts.FirstOrDefault (x => x.DiscountID == int.Parse (rb.Value));
			} else {
				ViewModel.SelectedDiscount = null;
			}
		}

		public static string ScrubHtml (string value)
		{
			var step1 = Regex.Replace (value, @"<[^>]+>|&nbsp;", "").Trim ();
			var step2 = Regex.Replace (step1, @"\s{2,}", " ");
			return step2;
		}

		private void StyleUITextField (UITextField field)
		{
			field.Layer.BorderColor = UIColor.Clear.FromHexString ("#aaaaaa").CGColor;
			field.Layer.BorderWidth = 1.2f;
			field.Layer.CornerRadius = 2;
			field.TextColor = UIColor.Black;
			field.BackgroundColor = UIColor.White;
			field.LeftView = new UIView (new CGRect (0, 0, 5, field.Frame.Height)){ ClipsToBounds = true };
			field.LeftViewMode = UITextFieldViewMode.Always;
		}

		private void SetupPicker (UITextField textField, UIImageView downImageArrow)
		{
			// Setup the picker and model
			PickerModel model = new PickerModel (ViewModel.ServiceCenters);
			model.PickerChanged += (sender, e) => {
				ViewModel.SelectedSCenter = ViewModel.ServiceCenters.FirstOrDefault (x => x.ID == e.SelectedValue);
				//Change view
			};

			UIPickerView picker = new UIPickerView ();
			picker.ShowSelectionIndicator = true;
			picker.Model = model;

			// Setup the toolbar
			UIToolbar toolbar = new UIToolbar ();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.Translucent = true;
			toolbar.SizeToFit ();

			// Create a 'done' button for the toolbar and add it to the toolbar
			UIBarButtonItem doneButton = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain,
				                             (s, e) => {
					textField.Text = ViewModel.SelectedSCenter.Name;
					textField.ResignFirstResponder ();
				});
			toolbar.SetItems (
				new UIBarButtonItem[]{ new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace), doneButton }, true);

			// Tell the textbox to use the picker for input
			textField.InputView = picker;
			var rec = new UITapGestureRecognizer (() => {

				textField.BecomeFirstResponder ();
			});

			downImageArrow.UserInteractionEnabled = true;
			downImageArrow.AddGestureRecognizer (rec);// = new UIGestureRecognizer[] { rec };

			textField.InputAccessoryView = toolbar;
			//downImageArrow.InputAccessoryView = toolbar;
		}

		#endregion
	}

	public class PickerChangedEventArgs : EventArgs
	{
		public int SelectedValue { get; set; }

		public string SelectedName { get; set; }

		public int Index { get; set; }
	}

	public class PickerModel : UIPickerViewModel
	{
		private readonly List<ServiceCenterDTO> values;

		public event EventHandler<PickerChangedEventArgs> PickerChanged;

		public PickerModel (List<ServiceCenterDTO> values)
		{
			this.values = values;
		}

		public override nint GetComponentCount (UIPickerView picker)
		{
			return 1;
		}

		public override nint GetRowsInComponent (UIPickerView picker, nint component)
		{
			return values.Count;
		}

		public override string GetTitle (UIPickerView picker, nint row, nint component)
		{
			return values [(int)row].Name;
		}

		public override nfloat GetRowHeight (UIPickerView picker, nint component)
		{
			return 40f;
		}

		public override void Selected (UIPickerView picker, nint row, nint component)
		{
			if (this.PickerChanged != null) {
				this.PickerChanged (this, new PickerChangedEventArgs { 
					SelectedValue = values [(int)row].ID, 
					SelectedName = values [(int)row].Name, 
					Index = (int)row
				});
			}
		}
	}
}

