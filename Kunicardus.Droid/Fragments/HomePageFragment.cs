using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
//using MvvmCross.Binding.Droid.Views;
using Kuni.Core;
using Kuni.Core.ViewModels;
using Kunicardus.Droid;
using Kunicardus.Droid.Adapters;
using Kunicardus.Droid.Plugins;
using Kunicardus.Droid.Plugins.Connectivity;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using System.Threading;

namespace Kunicardus.Droid
{
    public class HomePageFragment : BaseMvxFragment, Android.Widget.AdapterView.IOnItemClickListener, View.IOnTouchListener, Animation.IAnimationListener
    {
        public bool AlertsUpdated
        {
            set
            {
                if (value)
                {
                    if (alertCount == null)
                    {
                        return;
                    }
                    _mainView.RunOnUiThread(() =>
                    {
                        if (_currentViewModel.NewsCount > 0)
                        {
                            alertCount.Visibility = ViewStates.Visible;
                        }
                        else {
                            alertCount.Visibility = ViewStates.Gone;
                        }
                    });
                }
            }
            get { return false; }
        }

        #region Private Variables

        private MainView _mainView;
        private ImageButton _menu;
        private RelativeLayout bottom;
        private RelativeLayout main, homePageToolbar;
        private float height;
        private DisplayMetrics display;
        private TextView txtClickUnicard, cardNumber;
        private ImageView homePageDivider, barcode, backLine;
        private BaseTextView txtPoints, txtTotalPoints;
        private View uniCard;
        private ImageButton closeCard;
        private ISharedPreferences prefs;
        private HomePageViewModel _currentViewModel;
        private TextView alertCount;
        private bool pinWasOpened;
        private int appOpenCases;
        private bool isConfirming;
        private MvxGridView _gridView;

        #endregion

        #region Properties

        public bool IsCardActive { get; set; }

        public bool IsAnimInProgress { get; set; }

        public bool DataPopulated
        {
            get { return true; }
            set
            {
                //if (value) {
                //	if (_homeContentView != null) {
                //		_homeContentView.ScrollTo (0, 0);
                //	}
                //}
            }
        }

        #endregion

        #region Construtor Implementation

        public HomePageFragment()
        {
            ViewModel = Mvx.IoCProvider.IoCConstruct<HomePageViewModel>();
        }

        #endregion

        #region Native Methods


        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutBoolean("app_from_background", true);
        }

        public void alertInitialization()
        {
            SetupAnimation();

            var location = View.FindViewById<ImageButton>(Resource.Id.merchants);
            location.Click += OnLocationClick;

            var alert = View.FindViewById<ImageButton>(Resource.Id.alert);
            alertCount = View.FindViewById<TextView>(Resource.Id.alertCount);
            alert.Click += OnAlertClick;
            alertCount.Click += OnAlertClick;

            HideKeyboard();

            var set = this.CreateBindingSet<HomePageFragment, HomePageViewModel>();
            set.Bind(this).For(v => v.AlertsUpdated).To(vmod => vmod.AlertsUpdated);
            set.Bind(this).For(v => v.DataPopulated).To(vmod => vmod.DataPopulated);
            set.Apply();

            CheckAlerts();
        }

        public void StartWorker()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                alertInitialization();
            }).Start();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var View = this.BindingInflate(Resource.Layout.HomePageView, null);
            homePageToolbar = View.FindViewById<RelativeLayout>(Resource.Id.home_page_toolbar);
            var title = (TextView)homePageToolbar.GetChildAt(1);
            title.Text = GetString(Resource.String.homePage);

            View.FindViewById<View>(Resource.Id.transparentV).Touch += (o, e) =>
            {
                e.Handled = true;
            };
            _mainView = ((MainView)base.Activity);

            var conn = new DroidConnectivityProviderPlugin();
            if (conn.IsNetworkReachable || conn.IsNetworkReachable)
            {
                _currentViewModel.UpdatePoints();
            }

            _gridView = View.FindViewById<MvxGridView>(Resource.Id.catalogGridView);
            _gridView.Adapter = new CatalogListAdapter(this.Activity, (IMvxAndroidBindingContext)BindingContext);
            _gridView.OnItemClickListener = this;

            backLine = View.FindViewById<ImageView>(Resource.Id.back_line);
            txtClickUnicard = View.FindViewById<BaseTextView>(Resource.Id.txt_click_Unicard);

            cardNumber = View.FindViewById<BaseTextView>(Resource.Id.cardNumber);
            bottom = View.FindViewById<RelativeLayout>(Resource.Id.toSeeRelativeLayout);
            main = View.FindViewById<RelativeLayout>(Resource.Id.cardRealativeLayout);
            barcode = View.FindViewById<ImageView>(Resource.Id.barcode);
            homePageDivider = View.FindViewById<ImageView>(Resource.Id.home_page_divider);

            var refresher = View.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            refresher.Refresh += OnRefresh;

            display = Resources.DisplayMetrics;
            height = display.HeightPixels;
            #region View Initialization

            var catalogLabel = View.FindViewById<TextView>(Resource.Id.txt_catalog);
            catalogLabel.Click += delegate
            {
                _mainView.CatalogClick();
            };

            #region Opening Pin
            prefs = Activity.GetSharedPreferences("pref", FileCreationMode.Private);
            appOpenCases = prefs.GetInt((this.ViewModel as HomePageViewModel).UserId, 0);
            bool appFromBackground = false;
            if (savedInstanceState != null)
            {
                appFromBackground = savedInstanceState.GetBoolean("app_from_background", false);
            }
            if (!appFromBackground && !pinWasOpened)
            {
                if (appOpenCases == 1 && !(_mainView.ViewModel as MainViewModel).UserAuthed)
                {
                    OpenPinInputDialog();
                }
                else if (appOpenCases == 0)
                {
                    OpenDialog();
                }
                else {
                }
                pinWasOpened = true;
            }
            #endregion

            txtPoints = View.FindViewById<BaseTextView>(Resource.Id.txtPoints);
            txtPoints.Click += (o, e) =>
            {
                _mainView.BalanceClick();
            };
            txtTotalPoints = View.FindViewById<BaseTextView>(Resource.Id.total_points);
            txtTotalPoints.Click += (o, e) =>
            {
                _mainView.BalanceClick();
            };
            rotateLayout = View.FindViewById<RelativeLayout>(Resource.Id.rotateLayout);
            uniCard = View.FindViewById<View>(Resource.Id.uniCard);
            uniCard.Click += UniCard_Click;
            uniCard.LongClick += UniCard_LongClick;
            uniCard.SetLayerType(LayerType.Software, null);
            handler = new Handler();

            _menu = View.FindViewById<ImageButton>(Resource.Id.menuImg);
            _menu.Click += (o, e) => _mainView.ShowMenu();
            #endregion
            StartWorker();
            return View;
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            ((SwipeRefreshLayout)sender).Refreshing = false;
            _currentViewModel.GetNewsList();
            _currentViewModel.UpdatePoints();
            _currentViewModel.GetProducts(null, 0);

        }

        private void UniCard_LongClick(object sender, View.LongClickEventArgs e)
        {
            rotateLayout.StartAnimation(anim);
        }

        private void UniCard_Click(object sender, EventArgs e)
        {
            if (!anim.HasStarted || anim.HasEnded)
            {
                _mainView.CardAnimationClicked(null, null);
            }
        }

        public override void OnViewModelSet()
        {
            base.OnViewModelSet();
            _currentViewModel = this.ViewModel as HomePageViewModel;
        }

        #endregion

        #region Methods

        private void OpenPinInputDialog()
        {
            var mainViewModel = (_mainView.ViewModel as MainViewModel);
            if (mainViewModel.UserSettings != null)
            {
                if (mainViewModel.UserSettings.PinIsSet.HasValue && mainViewModel.UserSettings.PinIsSet.Value == true)
                {
                    _mainView.OpenPinInputDialog(mainViewModel.UserSettings.Pin.ToString());
                }
            }
        }

        private void OpenDialog()
        {
            var choosePinFragment = new ChoosePinSettingsDialogFragment(_currentViewModel);
            choosePinFragment.Show(_mainView.FragmentManager, "");
            choosePinFragment.SetStyle(DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
            choosePinFragment.Cancelable = false;
        }

        #endregion

        #region Events

        public override void OnPause()
        {
            if (IsCardActive)
            {
                ZoomOut();
            }
            base.OnPause();
        }

        void OnAlertClick(object sender, EventArgs e)
        {
            _mainView.AlertClick();
        }

        void OnLocationClick(object sender, EventArgs e)
        {
            _mainView.LocationClick();
        }

        public override void OnActivate()
        {
            //if (_homeContentView != null) {
            //	//_homeContentView.ScrollTo (0, 0);	
            //}
            //new Thread(() =>
            //{
            //    Thread.CurrentThread.IsBackground = true;
            //    CheckAlerts();
            //}).Start();
        }
        //		override setmenu

        void CheckAlerts()
        {
            if (alertCount != null)
            {
                ((HomePageViewModel)ViewModel).GetAlerts();

                if (((HomePageViewModel)ViewModel).NewsCount > 0)
                {
                    alertCount.Visibility = ViewStates.Visible;
                }
                else {
                    alertCount.Visibility = ViewStates.Gone;
                }
            }
        }

        public void CardClicked(object sender, EventArgs e)
        {
            ////GAService.GetGASInstance().Track_App_Event("Card Clicked", "from homePage");
            _mainView.CardAnimationClicked(null, null);
        }

        #endregion

        #region Retrieving Messages

        public void OnItemClick(AdapterView parent, View View, int position, long id)
        {
            _mainView._dialog.Show();
            Task.Run(() =>
            {
                var netwService = new DroidConnectivityProviderPlugin();
                if (!(netwService.IsWifiReachable || netwService.IsNetworkReachable))
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        Toast.MakeText(Activity, Resource.String.no_network, ToastLength.Short).Show();
                        _mainView._dialog.Dismiss();
                    });
                    return;
                }

                var pid = _currentViewModel.Products[position].ProductID;
                CatalogDetailFragment _catalogDetailFragment = new CatalogDetailFragment(pid);

                ////GAService.GetGASInstance().Track_App_Event(GAServiceHelper.Events.CatalogClicked, GAServiceHelper.From.FromHomePage);
                ////GAService.GetGASInstance().Track_App_Page(GAServiceHelper.Page.CatalogDetail);

                var fragmentTransaction = _mainView.SupportFragmentManager.BeginTransaction();
                fragmentTransaction.SetCustomAnimations(Resource.Animation.slide_in, Resource.Animation.slide_out);
                fragmentTransaction.AddToBackStack(null);
                fragmentTransaction.Add(Resource.Id.main_fragment, _catalogDetailFragment).Commit();
            });
        }

        public override void OnResume()
        {
            _gridView.Clickable = true;
            base.OnResume();
        }

        public override void SetMenuVisibility(bool menuVisible)
        {
            base.SetMenuVisibility(menuVisible);
            if (menuVisible)
            {
                ((HomePageViewModel)ViewModel).UpdatePoints();
                ((HomePageViewModel)ViewModel).GetProducts(null, 0);
            }
        }


        #endregion

        #region Animation Control

        public void Animate()
        {
            if (!IsAnimInProgress)
            {
                if (!IsCardActive)
                {
                    ZoomIn();
                }
                else {
                    ZoomOut();
                }
            }
        }

        public void ZoomIn()
        {
            IsCardActive = true;
            IsAnimInProgress = true;
            closeCard.Clickable = true;
            using (Bitmap bitmap = BarcodeGenerator.Generate(((HomePageViewModel)ViewModel).CardNumber, barcode.Height, barcode.Width))
            {
                barcode.SetImageBitmap(bitmap);
            }
            bottom.Animate().TranslationYBy(height);
            homePageDivider.Animate().TranslationYBy(height);
            main.Animate().RotationBy(90).SetDuration(300);
            main.Animate().ScaleX((float)(3.5 / 2.0)).ScaleY((float)(3.5 / 2.0));
            main.Animate().TranslationYBy(main.Height / 1.3f);
            main.Animate().TranslationX(-main.Height / 6);
            Task.Run(() =>
            {
                return EndAnim(true);
            });
            _mainView.HideTabs();
            //hide some Views
            backLine.Visibility = ViewStates.Gone;
            cardNumber.Visibility = ViewStates.Invisible;
            barcode.Visibility = ViewStates.Visible;
            closeCard.Visibility = ViewStates.Visible;
            txtTotalPoints.Visibility = ViewStates.Invisible;
            txtPoints.Visibility = ViewStates.Invisible;
            txtClickUnicard.Visibility = ViewStates.Invisible;
            homePageToolbar.Visibility = ViewStates.Invisible;
        }

        public void ZoomOut()
        {
            IsAnimInProgress = true;
            closeCard.Clickable = false;
            bottom.Animate().TranslationYBy(-height);
            homePageDivider.Animate().TranslationYBy(-height);
            main.Animate().RotationBy(-90).SetDuration(300);
            main.Animate().ScaleX(2 / 2.0f).ScaleY(2 / 2.0f);
            main.Animate().TranslationYBy(-main.Height / 1.3f);
            main.Animate().TranslationX((main.Height / 8) / 6);

            Task.Run(() =>
            {
                return EndAnim(false);
            });
            ((MainView)base.Activity).ShowTabs();
            IsCardActive = false;

            barcode.Visibility = ViewStates.Gone;
            backLine.Visibility = ViewStates.Visible;
            cardNumber.Visibility = ViewStates.Visible;
            closeCard.Visibility = ViewStates.Invisible;
            txtTotalPoints.Visibility = ViewStates.Visible;
            txtPoints.Visibility = ViewStates.Visible;
            txtClickUnicard.Visibility = ViewStates.Visible;
            homePageToolbar.Visibility = ViewStates.Visible;
        }

        async Task<bool> EndAnim(bool isOpening)
        {
            if (isOpening)
            {
                _mainView.HideTabs();
            }
            else {
                _mainView.ShowTabs();
            }
            IsAnimInProgress = false;
            return IsAnimInProgress;
        }

        public void HideKeyboard()
        {
            View View = _mainView.CurrentFocus;
            if (View != null)
            {
                InputMethodManager inputManager = (InputMethodManager)_mainView.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(View.WindowToken, HideSoftInputFlags.NotAlways);
            }
        }

        void SetupAnimation()
        {
            anim = new RotateAnimation(0f, 1079f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            anim.Duration = 1300;
            anim.SetInterpolator(Activity, Android.Resource.Animation.LinearInterpolator);
            anim.RepeatCount = 0;
            anim.FillAfter = false;
            anim.SetAnimationListener(this);

            _startAnimation = delegate
            {
                rotateLayout.StartAnimation(anim);
                //uniCard.StartAnimation(anim);
                //cardNumber.StartAnimation(anim);
            };
        }

        RelativeLayout rotateLayout;
        RotateAnimation anim;
        Action _startAnimation;
        Handler handler;

        //private Rect rect;
        private int oldValue = 0;

        public bool OnTouch(View v, MotionEvent e)
        {
            Log.Debug("MotionEvent", e.Action.ToString());

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    oldValue = (int)(e.GetX() + e.GetY());

                    rotateLayout.Alpha = 0.4f;
                    handler.PostDelayed(_startAnimation, 1500);
                    break;
                case MotionEventActions.Move:
                    int newValue = (int)(e.GetX() + e.GetY());
                    if (Math.Abs(oldValue - newValue) > 5)
                    {
                        rotateLayout.Alpha = 1f;
                        handler.RemoveCallbacks(_startAnimation);
                    }
                    break;
                case MotionEventActions.Up:
                    rotateLayout.Alpha = 1f;
                    handler.RemoveCallbacks(_startAnimation);
                    if (!anim.HasStarted || anim.HasEnded)
                    {
                        _mainView.CardAnimationClicked(null, null);
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        public void OnAnimationEnd(Animation animation)
        {
            rotateLayout.Alpha = 1;
        }

        public void OnAnimationRepeat(Animation animation)
        {
            // throw new NotImplementedException();
        }

        public void OnAnimationStart(Animation animation)
        {
            rotateLayout.Alpha = 1f;
        }

        #endregion

    }
}
