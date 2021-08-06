using System;
using UIKit;
using Kunicardus.Core;
using System.Runtime.CompilerServices;
using Kunicardus.Core.ViewModels;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Kunicardus.Touch.Helpers.UI;
using Cirrious.MvvmCross.Binding.BindingContext;
using Accelerate;
using System.Collections.Generic;
using CoreImage;
using MessageUI;
using Foundation;
using CoreText;
using System.Runtime.InteropServices;

namespace Kunicardus.Touch
{
    public class OrganizationDetailsViewController : BaseMvxViewController
    {
        #region Props

        public new OrganizationDetailsViewModel ViewModel
        {
            get { return (OrganizationDetailsViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public bool DataPopulated
        {
            set
            {
                if (value)
                {
                    AddViews();
                    LabelTouchLogic();
                }
            }
            get { return false; }
        }

        #endregion

        #region Constructor

        public OrganizationDetailsViewController()
        {
            HideMenuIcon = true;
        }

        #endregion

        #region Variables

        private UIScrollView _mainScrollView;
        private UIButton _emailButton, _webpageButton, _fbButton;
        private nfloat _paddingTopRisesMenu = 60f;
        private nfloat tempPadding = 10f;
        private UIView _bottomView;
        private BindableUIWebView _descWebView;
        private UIImageView _orgImage;
        private nfloat _orgIconImageHeight = 60f;

        #endregion

        #region Overrides

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.NavigationBarHidden = false;
            View.BackgroundColor = UIColor.White;
            Title = ApplicationStrings.Partners;
            InitUI();
            var set = this.CreateBindingSet<OrganizationDetailsViewController, OrganizationDetailsViewModel>();
            set.Bind(this).For(v => v.DataPopulated).To(vmod => vmod.DataPopulated);
            set.Apply();
        }

        #endregion

        #region Methods

        private void InitUI()
        {
            _mainScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));

            _orgImage = new UIImageView(new CGRect(10f, tempPadding, View.Frame.Width - 20f, _orgIconImageHeight));
            _orgImage.ContentMode = UIViewContentMode.ScaleAspectFit;
            _orgImage.ClipsToBounds = true;

            tempPadding += _orgImage.Frame.Height + 10f;

            _mainScrollView.AddSubview(LineDivider(tempPadding));

            UILabel orgName = new UILabel(new CGRect(10f, tempPadding + (_paddingTopRisesMenu - 40f) / 2, 210f, 40));
            orgName.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize + 1f);
            orgName.Lines = 2;
            orgName.LineBreakMode = UILineBreakMode.WordWrap;

            nfloat pointHeight = 24f, pointIndicatorHeight = 10f;
            UILabel point = new UILabel(new CGRect(View.Frame.Width - 100f, tempPadding + (_paddingTopRisesMenu - (pointHeight + pointIndicatorHeight)) / 2, 65f, pointHeight));
            point.TextColor = UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen);
            point.TextAlignment = UITextAlignment.Right;
            point.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize + 6f);

            //point.BackgroundColor = UIColor.Yellow;
            UILabel pointIndicator = new UILabel(new CGRect(point.Frame.Right, tempPadding + (_paddingTopRisesMenu - (pointHeight + pointIndicatorHeight)) / 2 + 12f, 30f, pointIndicatorHeight));
            pointIndicator.TextColor = UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen);
            pointIndicator.Text = ApplicationStrings.Score;
            pointIndicator.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);

            UILabel price = new UILabel(new CGRect(pointIndicator.Frame.Left - 70f, point.Frame.Bottom, 100f, 20f));
            price.TextAlignment = UITextAlignment.Right;
            price.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 4f);
            price.TextColor = UIColor.LightGray;

            var loader = new MvxImageViewLoader(() => _orgImage);

            _mainScrollView.AddSubviews(point, pointIndicator,
                orgName, _orgImage,
                price);

            View.AddSubview(_mainScrollView);

            //bindings
            var set = this.CreateBindingSet<OrganizationDetailsViewController, OrganizationDetailsViewModel>();
            set.Bind(point).To(vm => vm.UnitScore);
            set.Bind(price).To(vm => vm.UnitDescription);
            set.Bind(orgName).To(vm => vm.Name);
            set.Bind(loader).To(vm => vm.ImageUrl);
            set.Apply();
        }

        private UIView LineDivider(nfloat top)
        {
            UIView view = new UIView(new CGRect(0, top, View.Frame.Width, 1f));
            view.BackgroundColor = UIColor.Clear.FromHexString("#e2e3e3");
            return view;
        }

        private void AddViews()
        {
            if (!string.IsNullOrWhiteSpace(ViewModel.Description))
            {
                tempPadding += _paddingTopRisesMenu;
                _mainScrollView.AddSubview(LineDivider(tempPadding - 2f));
                _descWebView = new BindableUIWebView(new CGRect(5f, tempPadding, View.Frame.Width - 15f, 80f));
                _descWebView.ScrollView.ScrollEnabled = false;
                _descWebView.ScrollView.AlwaysBounceHorizontal = false;
                _descWebView.ScrollView.AlwaysBounceVertical = false;
                _descWebView.ScrollView.ShowsHorizontalScrollIndicator = false;
                _descWebView.ScrollView.ShowsVerticalScrollIndicator = false;
                this.CreateBinding(_descWebView).For("Text").To((OrganizationDetailsViewModel vm) => vm.Description).Apply();
                this.AddBindings(new Dictionary<object, string>() { { _descWebView, "Text Description" } });
                _descWebView.BackgroundColor = UIColor.White;
                _descWebView.ScalesPageToFit = false;
                _mainScrollView.AddSubview(_descWebView);
                _descWebView.LoadFinished += HtmlLoaded;
            }

            if (!string.IsNullOrWhiteSpace(ViewModel.Description))
            {
                _bottomView = new UIView(new CGRect(0, tempPadding, View.Frame.Width, 60f));
                tempPadding += _paddingTopRisesMenu;
            }
            else {
                _bottomView = new UIView(new CGRect(0, tempPadding + _paddingTopRisesMenu, View.Frame.Width, tempPadding));
            }

            tempPadding = 0f;
            if (!string.IsNullOrWhiteSpace(ViewModel.WorkingHours))
            {
                var hoursButton = new UIButton(UIButtonType.System);
                hoursButton.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
                hoursButton.TitleLabel.Lines = 0;
                hoursButton.SetTitleColor(UIColor.Clear.FromHexString("#707070"), UIControlState.Normal);
                this.CreateBinding(hoursButton).For("Title").To((OrganizationDetailsViewModel vm) => vm.WorkingHours).Apply();
                this.AddBindings(new Dictionary<object, string>() { { hoursButton, "Title WorkingHours" } });
                AddSubView("clock_green", this.ViewModel.WorkingHours, hoursButton);
                tempPadding += _paddingTopRisesMenu;
            }
            if (ViewModel.Phones != null && ViewModel.Phones.Count > 0)
            {
                AddPhoneSubView();
                tempPadding += _paddingTopRisesMenu;
            }
            if (!string.IsNullOrWhiteSpace(ViewModel.Mail))
            {
                _emailButton = new UIButton(UIButtonType.System);
                this.CreateBinding(_emailButton).For("Title").To((OrganizationDetailsViewModel vm) => vm.Mail).Apply();
                this.AddBindings(new Dictionary<object, string>() { { _emailButton, "Title Mail" } });
                AddSubView("email_green", this.ViewModel.Mail, _emailButton);
                tempPadding += _paddingTopRisesMenu;
            }
            if (!string.IsNullOrWhiteSpace(ViewModel.Website))
            {
                _webpageButton = new UIButton(UIButtonType.System);
                this.CreateBinding(_webpageButton).For("Title").To((OrganizationDetailsViewModel vm) => vm.Website).Apply();
                this.AddBindings(new Dictionary<object, string>() { { _webpageButton, "Title Website" } });
                AddSubView("webpage_green", this.ViewModel.Website, _webpageButton);
                tempPadding += _paddingTopRisesMenu;
            }
            if (!string.IsNullOrWhiteSpace(ViewModel.FbLink))
            {
                _fbButton = new UIButton(UIButtonType.System);
                this.CreateBinding(_fbButton).For("Title").To((OrganizationDetailsViewModel vm) => vm.FbLink).Apply();
                this.AddBindings(new Dictionary<object, string>() { { _fbButton, "Title FbLink" } });
                AddSubView("fb_green", this.ViewModel.FbLink, _fbButton);
                tempPadding += _paddingTopRisesMenu;
            }

            InitObjectListButton();

            _mainScrollView.AddSubview(_bottomView);
        }

        private void InitObjectListButton()
        {
            nfloat buttonHeight = _paddingTopRisesMenu - 20f;
            UIButton button = new UIButton(UIButtonType.System);
            button.Frame = new CGRect(30, tempPadding, View.Frame.Width - 60, buttonHeight);
            button.BackgroundColor = UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen);
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
            button.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 20);
            button.TintColor = UIColor.White;
            button.DropShadowDependingOnBGColor();
            button.SetTitle(ApplicationStrings.ObjectList, UIControlState.Normal);
            button.Layer.CornerRadius = button.Frame.Height / 2.0f;
            button.TouchUpInside += ShowObjects;
            _bottomView.AddSubview(button);

            ChangeBottomViewHeight(_paddingTopRisesMenu);

            _mainScrollView.ContentSize = new CGSize(View.Frame.Width, _bottomView.Frame.Bottom);
        }

        private void AddPhoneSubView()
        {
            _bottomView.AddSubview(LineDivider(tempPadding));
            var count = ViewModel.Phones.Count;
            UIImageView imageView = new UIImageView(UIImage.FromBundle("phone_green"));

            var imageWidth = imageView.Image.CGImage.Width;
            var imageHeight = imageView.Image.CGImage.Height;
            float proportion;
            nint imageSize = 28;
            if (imageWidth > imageHeight)
            {
                proportion = (float)imageSize / imageWidth;
                imageWidth = imageSize;
                imageHeight = Convert.ToInt32(((float)imageHeight * proportion));

            }
            else {
                proportion = (float)imageSize / imageHeight;
                imageHeight = imageSize;
                imageWidth = Convert.ToInt32(((float)imageWidth * proportion));
            }

            UIScrollView phoneScrollView = new UIScrollView(new CGRect(imageWidth + 30f, tempPadding, View.Frame.Width - imageWidth, _paddingTopRisesMenu));
            phoneScrollView.AlwaysBounceVertical = false;
            phoneScrollView.ShowsVerticalScrollIndicator = false;
            phoneScrollView.AlwaysBounceHorizontal = true;
            phoneScrollView.ShowsHorizontalScrollIndicator = false;
            imageView.Frame = new CGRect(18f, tempPadding + (_paddingTopRisesMenu - 30) / 2, imageWidth, imageHeight);
            _bottomView.AddSubview(imageView);
            float leftpadding = 0f;
            for (int i = 0; i < count; i++)
            {
                UIButton phoneItem = new UIButton(UIButtonType.System);
                phoneItem.Frame = new CGRect(0f, 0f, 100f, 100f);
                phoneItem.SetTitle(ViewModel.Phones[i], UIControlState.Normal);
                phoneItem.TintColor = UIColor.Clear.FromHexString("#8DBD3B");
                phoneItem.SetTitleColor(UIColor.Clear.FromHexString("#8DBD3B"), UIControlState.Normal);
                phoneItem.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 4);
                phoneItem.SizeToFit();
                phoneItem.Frame = new CGRect(leftpadding, (_paddingTopRisesMenu - 30) / 2, phoneItem.Frame.Width + 5f, phoneItem.Frame.Height);
                leftpadding += (float)phoneItem.Frame.Width + 5f;
                phoneScrollView.AddSubview(phoneItem);
                phoneItem.TouchUpInside += (sender, e) =>
                {
                    var phoneNumber = phoneItem.Title(UIControlState.Normal).Replace("(", "").Replace(";", "").Replace(")", "").Replace(" ", "").Trim();
                    var url = new NSUrl("tel:" + phoneNumber);
                    UIApplication.SharedApplication.OpenUrl(url);
                    if (!UIApplication.SharedApplication.OpenUrl(url))
                    {
                        var av = new UIAlertView("Not supported",
                                     "Scheme 'tel:' is not supported on this device",
                                     null,
                                     "OK",
                                     null);
                        av.Show();
                    }
                };
                if (count >= 2 && i < count - 1)
                {
                    phoneItem.SetTitle(phoneItem.Title(UIControlState.Normal) + ";", UIControlState.Normal);
                }
                if (i == count - 1)
                {
                    phoneScrollView.ContentSize = new CGSize(phoneItem.Frame.Right, _paddingTopRisesMenu);
                }
            }
            _bottomView.AddSubview(phoneScrollView);
        }

        private void AddSubView(string imageSource, string textSource, UIButton button)
        {
            ChangeBottomViewHeight(_paddingTopRisesMenu);
            UIImageView imageView = new UIImageView(UIImage.FromBundle(imageSource));
            var imageWidth = imageView.Image.CGImage.Width;
            var imageHeight = imageView.Image.CGImage.Height;
            float proportion;
            nint imageSize = 28;
            if (imageWidth > imageHeight)
            {
                proportion = (float)imageSize / imageWidth;
                imageWidth = imageSize;
                imageHeight = Convert.ToInt32(((float)imageHeight * proportion));

            }
            else {
                proportion = (float)imageSize / imageHeight;
                imageHeight = imageSize;
                imageWidth = Convert.ToInt32(((float)imageWidth * proportion));
            }

            button.Frame = new CGRect(0, tempPadding + (_paddingTopRisesMenu - 40) / 2, View.Frame.Width - 20f, 40);
            button.SetTitleColor(UIColor.Clear.FromHexString("#8DBD3B"), UIControlState.Normal);
            button.SetImage(ImageHelper.MaxResizeImage(UIImage.FromBundle(imageSource), imageWidth, imageHeight), UIControlState.Normal);
            button.TintColor = UIColor.Clear.FromHexString("#8DBD3B");
            button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            if (imageSource == "phone_green")
            {
                button.ImageEdgeInsets = new UIEdgeInsets(0.0f, 18.0f, 8.0f, 0.0f);
                button.TitleEdgeInsets = new UIEdgeInsets(0.0f, 27.0f, 10.0f, 0f);
            }
            else {
                button.ImageEdgeInsets = new UIEdgeInsets(0.0f, 13.0f, 7.0f, 0.0f);
                button.TitleEdgeInsets = new UIEdgeInsets(0.0f, 22.0f, 10.0f, 0f);
            }

            button.TitleLabel.TextAlignment = UITextAlignment.Left;
            if (imageSource == "clock_green")
                button.SetTitleColor(UIColor.Clear.FromHexString("#707070"), UIControlState.Normal);
            button.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 4);

            button.TitleLabel.SizeToFit();
            _bottomView.AddSubview(button);

            _bottomView.AddSubview(LineDivider(tempPadding));

            var buttonWidth = button.TitleLabel.Frame.Width;
            if (buttonWidth / View.Frame.Width >= 1 && imageSource == "clock_green")
            {
                var value = (buttonWidth / View.Frame.Width) * button.TitleLabel.Frame.Height;
                tempPadding += value;
                var frame = button.Frame;
                frame.Y += value / 2;
                button.Frame = frame;

                var bFrame = _bottomView.Frame;
                bFrame.Height += value;
                _bottomView.Frame = bFrame;
            }
        }

        private void ChangeBottomViewHeight(nfloat size)
        {
            var frame = _bottomView.Frame;
            frame.Height += size;
            _bottomView.Frame = frame;
        }

        private void LabelTouchLogic()
        {
            //mail click
            if (_emailButton != null)
                _emailButton.TouchUpInside += (sender, e) =>
                {
                    MFMailComposeViewController mailController;
                    if (MFMailComposeViewController.CanSendMail)
                    {
                        mailController = new MFMailComposeViewController();
                        mailController.SetToRecipients(new string[] { this.ViewModel.Mail });
                        mailController.NavigationBar.TintColor = UIColor.White;
                        this.PresentViewController(mailController, true, null);
                        mailController.Finished += (object s, MFComposeResultEventArgs args) =>
                        {
                            args.Controller.DismissViewController(true, null);
                        };
                    }
                };

            //webpage click
            if (_webpageButton != null)
                _webpageButton.TouchUpInside += (sender, e) =>
                {
                    OpenWebPage(this.ViewModel.Website);
                };

            //fb Click
            if (_fbButton != null)
                _fbButton.TouchUpInside += (sender, e) =>
                {
                    OpenWebPage(this.ViewModel.FbLink);
                };

        }

        private void OpenWebPage(string webAddress)
        {
            string address;
            if (!this.ViewModel.Website.ToLower().Contains("http") && !this.ViewModel.Website.ToLower().Contains("https"))
            {
                address = "http://" + webAddress;
            }
            else
                address = webAddress;
            UIApplication.SharedApplication.OpenUrl(new NSUrl(address));
        }

        #endregion

        #region Events

        private void HtmlLoaded(object sender, EventArgs e)
        {
            var js = String.Format("document.getElementsByTagName('p')[0].style.fontSize= 'small'");

            var offset = "document.getElementsByTagName('p')[0].offsetHeight;";
            _descWebView.EvaluateJavascript(js);
            var offsetHeight = _descWebView.EvaluateJavascript(offset);
            var frame = _descWebView.Frame;
            if ((nfloat)Convert.ToDouble(offsetHeight) < _paddingTopRisesMenu)
            {
                frame.Height = _paddingTopRisesMenu;
            }
            else {
                frame.Height = (nfloat)Convert.ToDouble(offsetHeight);
                frame.Height += 10f;
            }
            _descWebView.Frame = frame;

            var bottomFrame = _bottomView.Frame;
            bottomFrame.Y = _descWebView.Frame.Bottom;
            _bottomView.Frame = bottomFrame;
            _mainScrollView.ContentSize = new CGSize(View.Frame.Width, _bottomView.Frame.Bottom);
        }

        void ShowObjects(object sender, EventArgs e)
        {
            var gaService = new GAService();
            gaService.TrackEvent(GAServiceHelper.From.FromPartnersDetails, GAServiceHelper.Events.MapClicked);
            gaService.TrackScreen(GAServiceHelper.Pagenames.AroundMe);
            ViewModel.ShowObjects();
        }

        #endregion
    }
}

