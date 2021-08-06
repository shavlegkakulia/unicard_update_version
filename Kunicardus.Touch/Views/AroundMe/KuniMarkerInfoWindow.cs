using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Core.Models.DB;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
    public class KuniMarkerInfoWindow : UIView
    {
        private readonly nfloat width = 265f;
        private readonly nfloat height = 130f;

        public KuniMarkerInfoWindow(MerchantInfo merchant) : base(new CGRect(0, 0, 265, 140))
        {
            this.BackgroundColor = UIColor.Clear;
            this.Layer.CornerRadius = 10;
            UIView contentView = new UIView(new CGRect(0, 0, width, height - 40));
            contentView.BackgroundColor = UIColor.White;
            this.AddSubview(contentView);

            // Info three dot button
            nfloat infoDotsPadding = 6f;
            nfloat infoDotsHeight = height - contentView.Frame.Height - (2 * infoDotsPadding);
            UIButton infoDots = new UIButton(new CGRect(this.Frame.Width - infoDotsHeight - infoDotsPadding, contentView.Frame.Height + infoDotsPadding, infoDotsHeight, infoDotsHeight));
            infoDots.Layer.CornerRadius = infoDotsHeight / 2.0f;
            infoDots.Layer.BorderColor = UIColor.White.CGColor;
            infoDots.Layer.BorderWidth = 2f;
            infoDots.SetTitleColor(UIColor.White, UIControlState.Normal);
            infoDots.Font = UIFont.SystemFontOfSize(20);
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                infoDots.Font = UIFont.SystemFontOfSize(30);
                infoDots.TitleEdgeInsets = new UIEdgeInsets(-9.0f, 0, 9.0f, 0);
            }
            infoDots.SetTitle("•••", UIControlState.Normal);
            this.AddSubview(infoDots);

            // Points label
            UILabel points = new UILabel(new CGRect(10, contentView.Frame.Height, infoDots.Frame.Left - 20, height - contentView.Frame.Height));
            points.TextColor = UIColor.White;
            points.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 14);
            points.Text = string.Format("{0} - {1} ქულა", merchant.UnitDescription, merchant.UnitScore);
            this.AddSubview(points);

            UIView imgWrap = new UIView(new CGRect(10, 10, contentView.Frame.Height - 20, contentView.Frame.Height - 20));
            imgWrap.BackgroundColor = UIColor.Clear;
            imgWrap.Layer.CornerRadius = (float)((contentView.Frame.Height - 20) / 2.0);
            imgWrap.Layer.MasksToBounds = false;
            imgWrap.ClipsToBounds = true;
            imgWrap.Layer.BorderColor = UIColor.Clear.FromHexString(Styles.Colors.LightGray).CGColor;
            imgWrap.Layer.BorderWidth = 2;
            contentView.AddSubview(imgWrap);
            // Image
            UIImageView img = new UIImageView(new CGRect(10, 10, imgWrap.Frame.Height - 20, imgWrap.Frame.Height - 20));
            img.ContentMode = UIViewContentMode.ScaleAspectFit;
            img.ClipsToBounds = true;
            //			img.Layer.CornerRadius = (float)(img.Frame.Width / 2.0f);
            //			img.Layer.MasksToBounds = true;
            //			img.Layer.BorderColor = UIColor.Clear.FromHexString (Styles.Colors.LightGray).CGColor;
            //			img.Layer.BorderWidth = 2;
            try
            {
                img.Image = ImageHelper.FromUrl(merchant.Image.Replace(@"\", "/"));
            }
            catch
            {
            }
            imgWrap.AddSubview(img);

            UILabel title = new UILabel(new CGRect(imgWrap.Frame.Right + 10, 5, contentView.Frame.Width - imgWrap.Frame.Right - 10, 40));
            title.TextColor = UIColor.Black;
            title.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 14);
            title.Text = merchant.MerchantName;
            title.Lines = 2;
            title.LineBreakMode = UILineBreakMode.WordWrap;
            contentView.AddSubview(title);


            UIImageView locationImg = new UIImageView(UIImage.FromBundle("location_pin2"));
            locationImg.Frame = new CGRect(imgWrap.Frame.Right + 10, title.Frame.Bottom + 5, locationImg.Frame.Width, locationImg.Frame.Height);
            contentView.AddSubview(locationImg);

            UILabel _address = new UILabel(new CGRect(
                                   locationImg.Frame.Right + 5,
                                   locationImg.Frame.Top,
                                   contentView.Frame.Width - locationImg.Frame.Right - 15,
                                   contentView.Frame.Height - locationImg.Frame.Top - 10));
            _address.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 10);
            _address.TextColor = UIColor.Clear.FromHexString("#95a3a9");
            _address.LineBreakMode = UILineBreakMode.WordWrap;
            _address.Lines = 0;
            _address.Text = merchant.Address;
            _address.SizeToFit();
            if (_address.Frame.Height > contentView.Frame.Height - locationImg.Frame.Top - 10)
            {
                _address.Frame = new CGRect(
                    locationImg.Frame.Right + 5,
                    locationImg.Frame.Top,
                    contentView.Frame.Width - locationImg.Frame.Right - 15,
                    contentView.Frame.Height - locationImg.Frame.Top - 10);
            }
            contentView.AddSubview(_address);
        }

        public override void Draw(CGRect rect)
        {
            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                g.BeginPath();

                g.MoveTo(0, 0);
                g.AddLineToPoint(width, 0);
                g.AddLineToPoint(width, height);
                g.AddLineToPoint(width / 2f + 10, height);
                g.AddLineToPoint(width / 2f, height + 10);
                g.AddLineToPoint(width / 2f - 10, height);
                g.AddLineToPoint(0, height);
                g.AddLineToPoint(0, 0);

                g.ClosePath();
                g.SetFillColor(UIColor.Clear.FromHexString("#f79521").CGColor);
                g.FillPath();
            }
        }
    }
}

