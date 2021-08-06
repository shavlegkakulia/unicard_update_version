using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core.Models.DB;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	[Register ("MerchantListRow")]
	public class MerchantListRow  : MvxTableViewCell
	{
		#region UI

		private UILabel _name;
		private UILabel _address;
		private UILabel _distance;
		private UIImageView _image;
		private MvxImageViewLoader _imageLoader;

		#endregion

		#region Ctors

		public MerchantListRow ()
		{
			if (ContentView.Frame.Height != 80) {
				var frame = ContentView.Frame;
				frame.Height = 80f;
				ContentView.Frame = frame;
			}
			CreateLayout ();
			InitializeBindings ();
		}


		public MerchantListRow (IntPtr handle) : base (handle)
		{
			if (ContentView.Frame.Height != 80) {
				var frame = ContentView.Frame;
				frame.Height = 80f;
				ContentView.Frame = frame;
			}
			CreateLayout ();
			InitializeBindings ();
		}

		#endregion

		#region Methods

		private void CreateLayout ()
		{
			Accessory = UITableViewCellAccessory.None;

			nfloat imageWidth = ContentView.Frame.Height - 10f;
			nfloat imageHeight = imageWidth;

			UIView imgWrap = new UIView (new CGRect (10, 5, imageWidth, imageHeight));
			imgWrap.BackgroundColor = UIColor.Clear;
			imgWrap.Layer.CornerRadius = (float)(imageWidth / 2.0);
			imgWrap.Layer.MasksToBounds = false;
			imgWrap.ClipsToBounds = true;
			imgWrap.Layer.BorderColor = UIColor.Clear.FromHexString (Styles.Colors.LightGray).CGColor;
			imgWrap.Layer.BorderWidth = 2;

			_image = new UIImageView (new CGRect (10, 10, imageWidth - 20, imageHeight - 20));
			_image.ClipsToBounds = true;
			_image.ContentMode = UIViewContentMode.ScaleAspectFit;
			_imageLoader = new MvxImageViewLoader (() => _image);
			imgWrap.AddSubview (_image);
			ContentView.AddSubview (imgWrap);

			_name = new UILabel (new CGRect (imgWrap.Frame.Right + 10, 0, ContentView.Frame.Width - imgWrap.Frame.Right - 100, 40));
			_name.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			_name.Lines = 2;
			_name.LineBreakMode = UILineBreakMode.WordWrap;
			//_name.BackgroundColor = UIColor.Red;
				
			ContentView.AddSubviews (_name);

			_address = new UILabel (new CGRect (
				imgWrap.Frame.Right + 10, 
				_name.Frame.Bottom + 2, 
				ContentView.Frame.Width - imgWrap.Frame.Right - 100, 
				ContentView.Frame.Height - _name.Frame.Bottom - 4));
			_address.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 10);
			_address.TextColor = UIColor.Clear.FromHexString ("#95a3a9");
			_address.LineBreakMode = UILineBreakMode.WordWrap;
			_address.Lines = 0;
			//_address.BackgroundColor = UIColor.Yellow;
			ContentView.AddSubviews (_address);

			_distance = new UILabel (new CGRect (_name.Frame.Right + 5, 
				(ContentView.Frame.Height - 20) / 2.0f, 90, 20));
			_distance.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			_distance.TextAlignment = UITextAlignment.Left;

			ContentView.AddSubviews (_distance);
		}

		private void InitializeBindings ()
		{
			this.DelayBind (() => {
				var set = this.CreateBindingSet<MerchantListRow, MerchantInfo> ();
				set.Bind (_name).To (vm => vm.MerchantName);
				set.Bind (_address).To (vm => vm.Address);
				set.Bind (_imageLoader).To (vm => vm.Image);
				set.Bind (_distance).To (vm => vm.DistanceText);

				set.Apply ();
			});
		}

		#endregion
	}
}

