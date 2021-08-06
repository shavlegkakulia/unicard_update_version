using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using ZXing;

namespace Kunicardus.Touch
{
	public class CardForScanView : UIView
	{
		#region Variables

		private string _cardNumber;

		#endregion

		#region Properties

		public UIButton Close { get; set; }

		#endregion

		#region Ctors

		public CardForScanView (CGRect frame, string cardNumber) : base (frame)
		{
			_cardNumber = cardNumber;
			this.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			Close = new UIButton (UIButtonType.RoundedRect);
		}

		#endregion

		public override void LayoutSubviews ()
		{			
			base.LayoutSubviews ();

			Close.SetImage (ImageHelper.MaxResizeImage (UIImage.FromBundle ("close"), 22, 0), UIControlState.Normal);
			Close.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			Close.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			Close.BackgroundColor = UIColor.Clear;
			Close.SetTitleColor (UIColor.White, UIControlState.Normal);
			Close.TintColor = UIColor.White;
			Close.SizeToFit ();
			Close.Frame = new CGRect (Frame.Width - Close.Frame.Width - 20, 0, Close.Frame.Width + 20, Close.Frame.Height + 20);

			UIImageView card = new UIImageView (
				                   (Screen.IsTall ? UIImage.FromBundle ("card_vertical_big") : UIImage.FromBundle ("card_vertical"))
			                   );
			card.SizeToFit ();
			card.Frame = 
				new CGRect ((Frame.Width - card.Frame.Width) / 2.0f, (Frame.Height - card.Frame.Height) / 2, 
				card.Frame.Width, card.Frame.Height);


			UILabel cardNumber = new UILabel ();
			if (!string.IsNullOrWhiteSpace (_cardNumber)) {
				cardNumber.Text = _cardNumber.Insert (4, " ").Insert (9, " ").Insert (14, " ");
			} else {
				cardNumber.Text = "Virtual card not available";
			}
			cardNumber.Transform = CGAffineTransform.MakeRotation ((nfloat)Math.PI / 2.0f);
			cardNumber.Font = UIFont.SystemFontOfSize (22);
			cardNumber.SizeToFit ();
			cardNumber.Frame = 
				new CGRect ((Frame.Width - cardNumber.Frame.Width) / 2.0f + 5f, 
				card.Frame.Top + (card.Frame.Height - (card.Frame.Height / 6.0f) - cardNumber.Frame.Height) / 2.0f, 
				cardNumber.Frame.Width, 
				cardNumber.Frame.Height);


			// barcode generation
			nfloat barcodeHeight = (card.Frame.Width - cardNumber.Frame.Width) / 2.0f - 40f;
			nfloat barcodeWidth = (card.Frame.Height - cardNumber.Frame.Height / 5.0f - 40f);
			var writer = new BarcodeWriter {
				Format = BarcodeFormat.CODE_128,
				Options = new ZXing.Common.EncodingOptions () { 					
					Width = (int)Math.Round (barcodeWidth), 
					Height = (int)Math.Round (barcodeHeight),
					Margin = 0
				}
			};

			UIImage barCode = writer.Write (_cardNumber);		
			UIImageView barCodeImageView = new UIImageView (barCode);
			barCodeImageView.SizeToFit ();

			UIView barcodeContainer = new UIView ();
			barcodeContainer.BackgroundColor = UIColor.White;
			barcodeContainer.Frame = new CGRect (0, 0, barCodeImageView.Frame.Width, barCodeImageView.Frame.Height + 10f);
			barCodeImageView.Frame = new CGRect (0, 5f, barCodeImageView.Frame.Width, barCodeImageView.Frame.Height);
			barcodeContainer.AddSubview (barCodeImageView);
			barcodeContainer.Transform = CGAffineTransform.MakeRotation ((nfloat)Math.PI / 2.0f);
			barcodeContainer.Frame = new CGRect (card.Frame.Left + 30, card.Frame.Top + 15, 
				barcodeContainer.Frame.Width, barcodeContainer.Frame.Height);

			this.AddSubview (Close);
			this.AddSubview (card);
			this.AddSubview (cardNumber);
			this.AddSubview (barcodeContainer);
		}


	}
}

