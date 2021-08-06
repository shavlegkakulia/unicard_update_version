using System;
using Cirrious.MvvmCross.Touch.Views;
using UIKit;
using Kunicardus.Core;
using Kunicardus.Core.ViewModels;
using Kunicardus.Touch.Helpers.UI;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreAnimation;
using Foundation;
using CoreGraphics;
using System.Net.NetworkInformation;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Kunicardus.Core.ViewModels.iOSSpecific;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.Touch.Target;
using Facebook.CoreKit;
using Kunicardus.Touch.Plugins.Connectivity;

namespace Kunicardus.Touch
{
    public class MainViewController : BaseMvxViewController
    {
        #region UI

        private UIButton _card;
        private PointsButton _pointsButton;
        private UILongPressGestureRecognizer _longPressGesture;
        private GAService _gaService;

        #endregion

        #region Props

        public UIRefreshControl RefreshControl
        {
            get;
            set;
        }

        public static bool DialogShowed
        {
            get;
            set;
        }

        public new MainViewModel ViewModel
        {
            get { return (MainViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        private string _cardNumber;

        public string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;
                if (_card != null)
                {
                    _card.SetTitle(FormatCardNumber(CardNumber), UIControlState.Normal);
                    //					if (app.OpenCard == true) {
                    //						app.OpenCard = null;
                    //						OpenCard ();
                    //					}
                    if (APP.LaunchedShortcutItem != null)
                    {
                        APP.LaunchedShortcutItem = null;
                        UIApplication.SharedApplication.InvokeOnMainThread(() =>
                        {
                            OpenCard();
                        });
                    }
                }
            }
        }

        private string _points;

        public string Points
        {
            get { return _points; }
            set
            {
                _points = value;
                if (_pointsButton != null)
                {
                    _pointsButton.Button.SetTitle(value.Replace(",", "."), UIControlState.Normal);
                }
            }
        }

        private int _newsCount;

        public int NewsCount
        {
            get { return _newsCount; }
            set
            {
                _newsCount = value;
                if (_newsCounter == null)
                {
                    _newsCounter = new KuniBadgeBarButtonItem("alert", null);
                }
                _newsCounter.BadgeCount = value;
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    _newsBarButton.Image = ImageFromView(_newsCounter).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                });
            }
        }

        private bool _dataPopulated;

        public bool DataPopulated
        {
            get { return _dataPopulated; }
            set
            {
                _dataPopulated = value;
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                RefreshControl.EndRefreshing();
            }
        }

        #endregion

        #region Overrides

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.Translucent = false;

            RefreshControl = new UIRefreshControl();
            RefreshControl.BackgroundColor = UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen);

            View.BackgroundColor = UIColor.White;
            Title = ApplicationStrings.MainPage;
            _gaService = GAService.GetGAServiceInstance();
            InitUI();
            InitPinUI();
            Console.WriteLine("Main View Loaded");
            APP.OpenCard += delegate
            {
                APP.LaunchedShortcutItem = null;
                OpenCard();
            };

            this.CreateBinding(this)
                .For(view => view.CardNumber)
                .To<MainViewModel>(vm => vm.CardNumber)
                .Apply();
            this.CreateBinding(this)
                .For(view => view.Points)
                .To<MainViewModel>(vm => vm.Points)
                .Apply();
            this.CreateBinding(this)
                .For(x => x.DataPopulated)
                .To((iMyPageViewModel vm) => vm.DataPopulated)
                .Apply();

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        #endregion

        #region Methods

        #region Pin Logic

        private float _pinPosition = 80f;
        private PinWrapper _pinWrapper;
        private ChoosePinSetup _choosePinSetup;
        private EnterPin _enterPin;
        AppDelegate app;

        private void InitPinUI()
        {

            app = UIApplication.SharedApplication.Delegate as AppDelegate;

            if (!DialogShowed)
            {
                var pinStatus = PinHelper.GetPinStatus(ViewModel.UserId);

                switch (pinStatus)
                {
                    case PinStatus.FirstInit:
                        {
                            if (_pinWrapper == null)
                            {
                                _pinWrapper = new PinWrapper(app.Window.Frame);

                                nfloat width = View.Frame.Width - 40f;
                                nfloat height = 180f;

                                if (_choosePinSetup == null)
                                {
                                    _choosePinSetup =
                                new ChoosePinSetup(
                                        new CoreGraphics.CGRect(
                                            (View.Frame.Width - width) / 2.0f,
                                            (app.Window.Frame.Height - height) / 2.0f,
                                            width,
                                            height));
                                }
                                _choosePinSetup.WithPin.TouchUpInside += WithPinCLicked;
                                _choosePinSetup.WithoutPin.TouchUpInside += WithoutPinCLicked;
                                _pinWrapper.AddSubview(_choosePinSetup);

                                app.Window.AddSubview(_pinWrapper);
                            }
                            break;
                        }
                    case PinStatus.ShouldEnterPin:
                        {
                            if (!ViewModel.UserAuthed)
                            {
                                if (_pinWrapper == null)
                                {
                                    nfloat width = View.Frame.Width - 60f;
                                    nfloat height = 160f;

                                    if (!Screen.IsTall)
                                    {
                                        height = 150;
                                        width = View.Frame.Width - 60f;
                                    }

                                    _pinWrapper = new PinWrapper(app.Window.Frame);
                                    app.Window.AddSubview(_pinWrapper);
                                    _enterPin = new EnterPin(
                                        new CoreGraphics.CGRect(
                                            (View.Frame.Width - width) / 2.0f,
                                            (app.Window.Frame.Height - height) / 2.0f - _pinPosition,
                                            width,
                                            height), PinStatus.ShouldEnterPin);
                                    app.Window.AddSubview(_enterPin);

                                    _enterPin.SetPinFinished += (pinSender, pinE) =>
                                    {
                                        var pinIsCorrect = ViewModel.PinIsCorrect(pinE);
                                        if (ViewModel.UserSettings.Pin != null)
                                        {
                                            PinHelper.CorrectPin = ViewModel.UserSettings.Pin;
                                            PinHelper.UserId = ViewModel.UserId;
                                        }
                                        if (pinIsCorrect)
                                        {
                                            _pinWrapper.RemoveFromSuperview();
                                            _enterPin.RemoveFromSuperview();
                                        }
                                        else
                                            _enterPin.ClearDigits();
                                    };
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
                DialogShowed = true;
            }

        }

        void WithPinCLicked(object sender, EventArgs e)
        {
            nfloat width = View.Frame.Width - 40f;
            nfloat height = 180f;
            _choosePinSetup.RemoveFromSuperview();
            _choosePinSetup.Dispose();
            _choosePinSetup = null;

            if (!Screen.IsTall)
            {
                height = 150;
                width = View.Frame.Width - 60f;
            }
            if (_enterPin == null)
            {
                UIView.Animate(0.4f, () =>
                {
                    _enterPin = new EnterPin(
                        new CoreGraphics.CGRect(
                            (View.Frame.Width - width) / 2.0f,
                            (app.Window.Frame.Height - height) / 2.0f - _pinPosition,
                            width,
                            height), PinStatus.FirstInit);
                }, () =>
                {
                });
            }
            app.Window.AddSubview(_enterPin);
            _enterPin.SetPinFinished += (pinSender, pinE) =>
            {
                PinHelper.SetPinStatus(ViewModel.UserId, PinStatus.ShouldEnterPin);
                PinHelper.CorrectPin = pinE;
                PinHelper.UserId = ViewModel.UserId;
                ViewModel.InsertSettingsInfo(true, true, true, pinE);
                _pinWrapper.RemoveFromSuperview();
                _enterPin.RemoveFromSuperview();
            };
            _enterPin.ConfirmWasIncorrect += (pinSender, pinE) =>
            {
                if (pinE)
                {
                    ViewModel.PinIncorrectToast();
                    _enterPin.ClearDigits();
                }
            };

        }

        void WithoutPinCLicked(object sender, EventArgs e)
        {
            PinHelper.SetPinStatus(ViewModel.UserId, PinStatus.NoPin);
            ViewModel.InsertSettingsInfo(true, true, false, null);

            UIView.Animate(0.3f, () =>
            {
                _pinWrapper.Alpha = 0f;
            }, () =>
            {
                _choosePinSetup.RemoveFromSuperview();
                _pinWrapper.RemoveFromSuperview();
                _choosePinSetup.Dispose();
                _choosePinSetup = null;
                _pinWrapper.Dispose();
                _pinWrapper = null;
            });
        }

        #endregion

        private void InitRefreshControl()
        {
            RefreshControl.ValueChanged += delegate
            {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
                ViewModel.GetProducts(null, null);
            };
        }

        KuniBadgeBarButtonItem _newsCounter;
        UIBarButtonItem _newsBarButton;

        private void InitUI()
        {
            UIScrollView _scroll = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));

            _scroll.DelaysContentTouches = false;
            nfloat statusBarHeight = GetStatusBarHeight();
            _scroll.BackgroundColor = UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen);
            _scroll.AddSubview(RefreshControl);
            InitRefreshControl();

            var conn = new TouchConnectivityPlugin();
            if (conn.IsNetworkReachable || conn.IsWifiReachable)
            {
                ViewModel.UpdatePoints();
            }

            #region Toolbar

            _newsCounter = new KuniBadgeBarButtonItem("alert", null);
            _newsBarButton = new UIBarButtonItem(ImageFromView(_newsCounter).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIBarButtonItemStyle.Plain, delegate
            {
                APP.SidebarController.ChangeContentView(CreateViewForList(Mvx.IocConstruct<NewsListViewModel>(), false));
                _gaService.TrackEvent(GAServiceHelper.From.FromHomePage, GAServiceHelper.Events.NewsClicked);
                _gaService.TrackScreen(GAServiceHelper.Pagenames.News);
            });

            var locationButton = new UIBarButtonItem(UIImage.FromBundle("location_pin2"), UIBarButtonItemStyle.Plain, delegate
            {
                APP.SidebarController.ChangeContentView(CreateViewFor(Mvx.IocConstruct<iMerchantsAroundMeViewModel>(), false));
                _gaService.TrackEvent(GAServiceHelper.From.FromHomePage, GAServiceHelper.Events.MapClicked);
                _gaService.TrackScreen(GAServiceHelper.Pagenames.AroundMe);
            });

            NavigationItem.RightBarButtonItems = new UIBarButtonItem[] {
                _newsBarButton,
                locationButton
            };

            this.CreateBinding(this)
                .For(view => view.NewsCount)
                .To<MainViewModel>(vm => vm.NewsCount)
                .Apply();

            //			alert.Clicked += delegate {
            //				kuniAlert.UpdateBadgeCount (kuniAlert.BadgeCount - 1);
            //			};

            //			var goToMerchants = new KuniBadgeBarButtonItem ("location_pin", () => {				
            //				// TODO: Go to Merchants
            //			}, 0);		

            #endregion

            #region Top View
            UIView topView = new UIView();
            topView.Frame = new CoreGraphics.CGRect(0, 0, View.Frame.Width, View.Frame.Height / 2.1f - statusBarHeight);
            topView.BackgroundColor = UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen);

            UIImageView cardBackLine = new UIImageView(ImageHelper.MaxResizeImage(UIImage.FromBundle("back_line_green"), View.Frame.Width, 0));
            cardBackLine.SizeToFit();
            cardBackLine.Frame =
                new CoreGraphics.CGRect(0, (topView.Frame.Height - cardBackLine.Frame.Height) / 2.0f, cardBackLine.Frame.Width, cardBackLine.Frame.Height);
            topView.AddSubview(cardBackLine);

            _card = new UIButton(UIButtonType.RoundedRect);
            _card.SetBackgroundImage(GetCardImage(), UIControlState.Normal);
            _card.SizeToFit();
            _card.SetTitleColor(UIColor.Black, UIControlState.Normal);
            _card.TintColor = UIColor.White;
            if (!Screen.IsTall)
            {
                _card.Font = UIFont.SystemFontOfSize(14);
            }
            _card.TitleEdgeInsets = new UIEdgeInsets(_card.Frame.Height / 2.0f - 5, 14, 50, 40);
            _card.BackgroundColor = UIColor.Clear;
            _card.Frame =
                new CoreGraphics.CGRect((View.Frame.Width - _card.Frame.Width) / 2.0f, (topView.Frame.Height - _card.Frame.Height) / 2.0f, _card.Frame.Width, _card.Frame.Height);
            if (!string.IsNullOrWhiteSpace(CardNumber))
            {
                _card.SetTitle(FormatCardNumber(CardNumber), UIControlState.Normal);
            }
            _card.TouchUpInside += delegate
            {
                OpenCard();
            };
            topView.AddSubview(_card);
            _longPressGesture = new UILongPressGestureRecognizer(() =>
            {
                if (_longPressGesture.State == UIGestureRecognizerState.Began)
                {
                    _longPressGesture.State = UIGestureRecognizerState.Ended;
                    Rotate(_card, 0.4f, 3);
                }
            })
            { MinimumPressDuration = 2 };
            _card.AddGestureRecognizer(_longPressGesture);

            //scrollview bottom color
            UIView bottomColorView = new UIView(new CGRect(0, topView.Frame.Bottom, View.Frame.Width, View.Frame.Height + 250f));
            bottomColorView.BackgroundColor = UIColor.White;
            _scroll.AddSubview(bottomColorView);

            UIImageView devider = new UIImageView(ImageHelper.MaxResizeImage(UIImage.FromBundle("devider_white_green"), View.Frame.Width, 0));
            devider.SizeToFit();
            devider.Frame = new CoreGraphics.CGRect(0, topView.Frame.Bottom, devider.Frame.Width, devider.Frame.Height);
            _scroll.AddSubview(devider);
            _scroll.AddSubview(topView);

            // Point round button initialisation
            nfloat pointsWidth = 80f;
            if (!Screen.IsTall)
            {
                pointsWidth = 70;
            }
            _pointsButton = new PointsButton(
                new CoreGraphics.CGRect(10, topView.Frame.Height - pointsWidth - (Screen.IsTall ? 5 : 2), pointsWidth, pointsWidth));
            topView.AddSubview(_pointsButton);
            _pointsButton.Button.TouchUpInside += delegate
            {
                AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
                app.SidebarController.ChangeContentView(CreateViewFor(Mvx.IocConstruct<iMyPageViewModel>(), false));
                _gaService.TrackScreen(GAServiceHelper.Pagenames.MyPage);
            };
            if (!string.IsNullOrWhiteSpace(Points))
            {
                _pointsButton.Button.SetTitle(Points.Replace(",", "."), UIControlState.Normal);
            }

            #endregion

            #region Products

            UILabel whereToSpend = new UILabel();
            whereToSpend.TextColor = UIColor.Black;
            whereToSpend.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 14);
            whereToSpend.Text = ApplicationStrings.WhereToSpend;
            whereToSpend.SizeToFit();
            whereToSpend.Frame = new CGRect(5, devider.Frame.Bottom + 10, whereToSpend.Frame.Width, whereToSpend.Frame.Height);
            _scroll.AddSubview(whereToSpend);

            UIButton catalogue = new UIButton(UIButtonType.RoundedRect);
            catalogue.SetTitleColor(UIColor.Clear.FromHexString(Styles.Colors.HeaderGreen), UIControlState.Normal);
            catalogue.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 14);
            catalogue.SetTitle(string.Format("{0} >", ApplicationStrings.Catalogue), UIControlState.Normal);
            catalogue.SizeToFit();
            catalogue.Frame = new CGRect(View.Frame.Width - catalogue.Frame.Width - 5,
                devider.Frame.Bottom + 3, catalogue.Frame.Width, catalogue.Frame.Height);
            catalogue.TouchUpInside += delegate
            {
                APP.SidebarController.ChangeContentView(CreateViewFor(Mvx.IocConstruct<iCatalogListViewModel>(), false));
                _gaService.TrackEvent(GAServiceHelper.From.FromHomePage, GAServiceHelper.Events.CatalogClicked);
                _gaService.TrackScreen(GAServiceHelper.Pagenames.Catalog);
            };
            _scroll.AddSubview(catalogue);

            UICollectionView CollectionView = new UICollectionView(
                                                  new CGRect(
                                                      0, whereToSpend.Frame.Bottom + 5, View.Frame.Width,

                                                      (((BaseMvxViewController.APP.Window.Frame.Width / 2.0f) + 40) * 2) + 4),
                                                  new UICollectionViewFlowLayout()
                                                  {
                                                      ItemSize =
                        new CGSize(
                        ((BaseMvxViewController.APP.Window.Frame.Width / 2.0f) - 2),
                        ((BaseMvxViewController.APP.Window.Frame.Width / 2.0f) + 40)),
                                                      ScrollDirection = UICollectionViewScrollDirection.Vertical,
                                                      SectionInset = new UIEdgeInsets(0, 2, 0, 2),
                                                      MinimumLineSpacing = 4,
                                                      MinimumInteritemSpacing = 0

                                                  });

            CollectionView.BackgroundColor = UIColor.White;
            CollectionView.RegisterClassForCell(typeof(ProductCell), ProductCell.Key);
            CollectionView.AllowsSelection = true;

            var source = new MvxCollectionViewSource(CollectionView, ProductCell.Key);
            CollectionView.Source = source;

            this.CreateBinding(source).For(x => x.SelectionChangedCommand).To((MainViewModel vm) => vm.ItemClick).Apply();
            this.CreateBinding(source).To((MainViewModel vm) => vm.Products).Apply();

            CollectionView.ReloadData();
            _scroll.AddSubview(CollectionView);

            _scroll.ContentSize = new CGSize(View.Frame.Width, CollectionView.Frame.Bottom + GetStatusBarHeight() + 10);
            View.AddSubview(_scroll);
            #endregion
        }

        private void OpenCard()
        {
            var vc = new CardViewController(CardNumber);
            _gaService.TrackEvent(GAServiceHelper.From.FromHomePage, GAServiceHelper.Events.CardClicked);
            AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
            app.Window.RootViewController.PresentModalViewController(vc, true);
        }

        private UINavigationController CreateViewForList(IMvxViewModel viewModel, bool navBarHidden)
        {
            var controller = new UINavigationController();
            var screen = this.CreateViewControllerFor(viewModel) as UITableViewController;
            controller.PushViewController(screen, false);
            controller.NavigationBar.BarStyle = UIBarStyle.Black;
            controller.NavigationBarHidden = navBarHidden;
            return controller;
        }

        private UIViewController CreateViewFor(IMvxViewModel viewModel, bool navBarHidden)
        {
            var controller = new UINavigationController();
            var screen = this.CreateViewControllerFor(viewModel) as UIViewController;
            controller.PushViewController(screen, false);
            controller.NavigationBar.BarStyle = UIBarStyle.Black;
            controller.NavigationBarHidden = navBarHidden;
            return controller;
        }

        private UIImage GetCardImage()
        {

            UIImage card = Screen.IsTall ? UIImage.FromBundle("card_horizontal_big") : UIImage.FromBundle("card_horizontal");
            UIImageView cardImageView = new UIImageView(card);
            cardImageView.SizeToFit();



            UILabel getpositive = new UILabel();
            getpositive.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 10);
            getpositive.TextColor = UIColor.White;
            getpositive.Text = " " + ApplicationStrings.GetPositive;
            getpositive.BackgroundColor = UIColor.Clear.FromHexString("#f28e2e");
            getpositive.Layer.CornerRadius = 10;
            getpositive.SizeToFit();
            getpositive.Frame = new CoreGraphics.CGRect(card.Size.Width - getpositive.Frame.Width - 30f,
                card.Size.Height - card.Size.Height / 3.7f, getpositive.Frame.Width + 4, getpositive.Frame.Height);
            cardImageView.AddSubview(getpositive);



            return ImageFromView(cardImageView);
        }

        private UIImage ImageFromView(UIView view)
        {
            UIGraphics.BeginImageContextWithOptions(view.Frame.Size, view.Opaque, 0.0f);
            view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            UIImage img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return img;
        }

        private string FormatCardNumber(string number)
        {
            if (!string.IsNullOrWhiteSpace(number))
            {
                return number.Insert(4, " ").Insert(9, " ").Insert(14, " ");
            }
            else {
                return number;
            }
        }

        private void Rotate(UIView view, nfloat duration, float repeat)
        {
            CABasicAnimation rotationAnimation;
            rotationAnimation = CABasicAnimation.FromKeyPath("transform.rotation.z");
            rotationAnimation.To = new NSNumber(Math.PI * 2);// [NSNumber numberWithFloat: M_PI * 2.0 /* full rotation*/ * rotations * duration ];
            rotationAnimation.Duration = duration;
            rotationAnimation.Cumulative = true;
            rotationAnimation.RepeatCount = repeat;
            view.Layer.AddAnimation(rotationAnimation, "rotationAnimation");
        }

        #endregion
    }
}

