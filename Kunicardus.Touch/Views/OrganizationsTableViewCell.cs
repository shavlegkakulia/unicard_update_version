using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using System.Drawing;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core.Models;

namespace Kunicardus.Touch
{
    [Register("OrganizationsTableViewCell")]
    public class OrganizationsTableViewCell : MvxTableViewCell
    {
        public OrganizationsTableViewCell(IntPtr handle) : base(handle)
        {
            CreateLayout();
            InitializeBindings();
        }

        private UILabel _title;
        private UILabel _point, _pointIndicator;
        private UILabel _price;
        private UIImageView _image;
        private MvxImageViewLoader _loader;

        private void CreateLayout()
        {
            const int offsetStart = 10;
            const float imageWidth = 65;
            const float imageHeight = 65;
            const int padding = 8;


            Accessory = UITableViewCellAccessory.None;
            _title = new UILabel(new RectangleF(offsetStart + imageWidth + 5,
                15,
                (float)ContentView.Frame.Width - offsetStart - imageWidth - 100, 40));
            _title.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 6);
            _title.LineBreakMode = UILineBreakMode.WordWrap;
            _title.Lines = 0;
            //_title.BackgroundColor = UIColor.Yellow;

            _point = new UILabel(new CGRect(_title.Frame.Right + 2, 14, ContentView.Frame.Width - _title.Frame.Right - 42, 20));
            _point.TextAlignment = UITextAlignment.Right;
            _point.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 3);
            _point.TextColor = UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen);
            //_point.BackgroundColor = UIColor.Red;

            _pointIndicator = new UILabel(new RectangleF((float)_point.Frame.Right + 3f, 14, 36, 20));
            _pointIndicator.TextAlignment = UITextAlignment.Left;
            _pointIndicator.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 4);
            _pointIndicator.TextColor = UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen);
            _pointIndicator.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f);
            _pointIndicator.Text = ApplicationStrings.Score;

            _price = new UILabel(new RectangleF((float)_pointIndicator.Frame.Left - 70f, 40, 100, 20));
            _price.TextAlignment = UITextAlignment.Right;
            _price.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 6);
            _price.TextColor = UIColor.LightGray;

            UIView imgWrap = new UIView(new CGRect(offsetStart, padding, imageWidth, imageHeight));
            imgWrap.BackgroundColor = UIColor.Clear;
            imgWrap.Layer.CornerRadius = (float)(imageWidth / 2.0);
            imgWrap.Layer.MasksToBounds = false;
            imgWrap.ClipsToBounds = true;
            imgWrap.Layer.BorderColor = UIColor.Clear.FromHexString(Styles.Colors.LightGray).CGColor;
            imgWrap.Layer.BorderWidth = 2;

            _image = new UIImageView(new CGRect(10, 10, imageWidth - 20, imageHeight - 20));
            //double min = Math.Min (imageWidth, imageWidth);
            //			_image.Layer.CornerRadius = (float)(min / 2.0);
            //			_image.Layer.MasksToBounds = false;
            //			_image.Layer.BorderColor = UIColor.Clear.FromHexString (Styles.Colors.LightGray).CGColor;
            //			_image.Layer.BorderWidth = 2;
            _image.ClipsToBounds = true;
            _image.ContentMode = UIViewContentMode.ScaleAspectFit;
            imgWrap.AddSubview(_image);
            _loader = new MvxImageViewLoader(() => _image);

            ContentView.AddSubviews(_title, _point, _pointIndicator, _price, imgWrap);

        }

        void InitializeBindings()
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<OrganizationsTableViewCell, OrganizationModel>();
                set.Bind(_title).To(vm => vm.Name);
                set.Bind(_point).To(vm => vm.UnitScore);
                set.Bind(_price).To(vm => vm.UnitDescription);
                set.Bind(_loader).To(vm => vm.Image);
                set.Apply();
            });
        }

        public UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }
    }
}

