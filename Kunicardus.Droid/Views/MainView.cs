using System;

using System.Text;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using MvvmCross.ViewModels;
using Kuni.Core;
using Kuni.Core.Models.DB;
using Kuni.Core.ViewModels;
using Kunicardus.Droid.Adapters;
using Kunicardus.Droid.Fragments;
using Kunicardus.Droid.Helpers;
using Kunicardus.Droid.Plugins;
using Xamarin.Facebook.Login;
using MvvmCross.Droid.Support.V4;
using Android.Support.V4.Widget;

namespace Kunicardus.Droid
{
	[Activity (Label = "UNICARD",
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, 
		LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
		Name = "ge.unicard.unicardmobileapp.MainView")]
	[MvxViewFor (typeof(MainViewModel))]
	public class MainView : MvxFragmentActivity,  View.IOnTouchListener
	{
		#region Values and fields

		private FrameLayout _menuView;
		private CustomFrameLayout _contentView;
		private ValueAnimator _animator, _cardAnimator;
		private MainViewModel _mainViewModel;
		private RelativeLayout _parent_card_layout;

		private ImageView activeUnicard;

		private HomePagerAdapter adapter;
		private CustomViewPager pager;
		private const int animDuration = 300;

		public bool _pinIsOpened;
		private DateTime _lastDateTime;
		private int _homeTabindex = 0, _myPageIndex = 1, _catalogIndex = 3, _arounMeIndex = 4, _newsIndex = 6;
		private bool _isHomeSelected, _appWasInBackground;
		public bool _isInnerActivity;
		int _pagerIndex;
		public ProgressDialog _dialog;
		protected IOnBackPressedListener onBackPressedListener;

		public int? OrgId {
			get;
			set;
		}

		public ValueAnimator Animator {
			get {
				if (_animator == null) {
					_animator = ValueAnimator.OfInt (0, 0);
					_animator.Update += AnimatorUpdate;
					_animator.SetDuration (150);
					_animator.AnimationEnd += AnimatorAnimationEnd;
				}
				return _animator;
			}
		}

		public ValueAnimator CardAnimator {
			get {
				if (_cardAnimator == null) {
					_cardAnimator = ValueAnimator.OfInt (0, 0);
					_cardAnimator.Update += CardAnimatorUpdate;
					_cardAnimator.SetDuration (animDuration);
					_cardAnimator.AnimationEnd += CardAnimatorAnimationEnd;
				}
				return _cardAnimator;
			}
		}

		MerchantsFragment merchantsFragment;

		#endregion

		#region Override Methods

		public void SetOnBackPressedListener (IOnBackPressedListener onBackPressedListener)
		{
			this.onBackPressedListener = onBackPressedListener;
		}

		public override void OnBackPressed ()
		{
			if (IsCardActive) {
				CardAnimationClicked (null, null);
				return;
			}

			if (IsMenuShown) {
				ShowMenu ();
				return;
			}

			DrawerLayout _dLayout = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			LinearLayout _drawerListViewLayout = FindViewById<LinearLayout> (Resource.Id.right_drawer_layout);

			if (_dLayout != null && _drawerListViewLayout != null) {
				if (_dLayout.IsDrawerOpen (_drawerListViewLayout)) {
					_dLayout.CloseDrawer (_drawerListViewLayout);
					return;
				}
			}
//			var merchantFragment = (MerchantsFragment)adapter.GetFragment (4);
//			if (merchantFragment.CheckOrganisationFilter ()) {
//				var partnersDetailFragment = SupportFragmentManager.FindFragmentByTag ("partners_detail_fragment");
//				if (partnersDetailFragment != null && partnersDetailFragment.IsHidden) {
//					SupportFragmentManager.BeginTransaction ().Show (partnersDetailFragment).Commit ();
//				}
//
//				//GAService.GetGASInstance ().Track_App_Page (5);
//				pager.SetCurrentItem (5, false);
//
//				return;
//			}

			if (ClearBackStack ()) {
				return;
			}

			if (pager.CurrentItem != 0) {

				//GAService.GetGASInstance ().Track_App_Page (0);
				pager.SetCurrentItem (0, false);
				return;
			}

			if (pager.CurrentItem == 0) {
				AlertDialog alertDialog;
				AlertDialog.Builder builder = new AlertDialog.Builder (this);
				builder.SetMessage (Resources.GetString (Resource.String.closeMessage));
				builder.SetPositiveButton (Resources.GetString (Resource.String.yes), delegate {
					this.MoveTaskToBack (true);
				});
				builder.SetNegativeButton ("არა", delegate {
				});                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
				alertDialog = builder.Create ();
				alertDialog.SetCanceledOnTouchOutside (true);
				alertDialog.Show ();

				TextView message = alertDialog.FindViewById<TextView> (Android.Resource.Id.Message);
				message.Gravity = GravityFlags.Center;
				return;

			}  
		}

		public bool ClearBackStack ()
		{
			if (onBackPressedListener != null) {
				onBackPressedListener.DoBack ();
				return true;
			}
			if (SupportFragmentManager.BackStackEntryCount >= 1) {
				SupportFragmentManager.PopBackStack ();
				return true;
			}
			return false;
		}

		protected override void OnResume ()
		{
			if (_appWasInBackground && !_isInnerActivity) {
				_appWasInBackground = false;

				var secondsDifference = DateTime.Now.Ticks / TimeSpan.TicksPerSecond - _lastDateTime.Ticks / TimeSpan.TicksPerSecond;
				if (secondsDifference > 60 && !_pinIsOpened) {
					var userSettings = _mainViewModel.GetUserSettings ();
					if (userSettings != null && userSettings.Pin != null) {
						OpenPinInputDialog (_mainViewModel.UserSettings.Pin);
						HideKeyboard ();
					}
				}
			}
			_isInnerActivity = false;
			base.OnResume ();
		}

		protected override void OnPause ()
		{
			_appWasInBackground = true;
			_lastDateTime = DateTime.Now;
			base.OnPause ();
		}

		protected	 override void OnViewModelSet ()
		{
			base.OnViewModelSet ();

			_contentView = FindViewById<CustomFrameLayout> (Resource.Id.main_fragment);
			_menuView = FindViewById<FrameLayout> (Resource.Id.menu_fragment);
			_mainViewModel = (ViewModel as MainViewModel);
			_parent_card_layout = FindViewById<RelativeLayout> (Resource.Id.parent_card_layout);

			InitViewAsync ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			SetContentView (Resource.Layout.MainView);

			_dialog = new ProgressDialog (this);
			_dialog.SetMessage (Resources.GetString (Resource.String.loading));
			try
			{
				_dialog.Show();
			}
			catch(Exception ex)
            {

            }

			_bigCardNumber = FindViewById<BaseTextView> (Resource.Id.bigCardNumber);
			FindViewById<RelativeLayout> (Resource.Id.bigCardNL).Animate ().Rotation (90);

			_widthPixels = Resources.DisplayMetrics.WidthPixels;

			base.OnCreate (bundle);
		}

		private async void InitViewAsync ()
		{
			MenuFragment menu = null;

			await Task.Run (() => {
				_rightY = Resources.DisplayMetrics.WidthPixels + Resources.DisplayMetrics.WidthPixels * _yFactor;

				activeUnicard = FindViewById<ImageView> (Resource.Id.uniCard);

				IsMenuShown = false;
				pager = FindViewById<CustomViewPager> (Resource.Id.pager);
				pager.OffscreenPageLimit = 9;
				pager.CurrentItem = 0;

				adapter = new HomePagerAdapter (SupportFragmentManager, this);
				RunOnUiThread (() => {
					pager.Adapter = adapter;
				});
				menu = new MenuFragment ();
				SupportFragmentManager.BeginTransaction ().Add (Resource.Id.menu_fragment, menu).Commit ();
			});

			_bigCardNumber.Text = _mainViewModel.CardNumber.FormatNumber ();

			var pageMargin = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, 4, Resources.DisplayMetrics);
			pager.PageMargin = pageMargin;
			pager.PageSelected += (o, e) => adapter.ActivateFragment (e.Position);
			_dialog.Dismiss ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			if (_contentView != null) {
				_contentView.SetOnTouchListener (this);
				_contentView.ScaleY = 1;
				_contentView.SetX (0);
				_contentView.SetPadding (0, 0, 0, 0);
			}

			height = Resources.DisplayMetrics.HeightPixels;
			IsMenuShown = false;
		}

		protected override void OnRestart ()
		{
			base.OnRestart ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			_contentView.SetOnTouchListener (null);

			if (_animator != null) {
				_animator.Update -= AnimatorUpdate;
				_animator.AnimationEnd -= AnimatorAnimationEnd;
				_animator = null;
			}
		}

		#endregion

		#region Child Fragment Control

		public void RefreshSettings (SettingsPinPages settingsPages)
		{
			SupportFragmentManager.PopBackStack ();

			var removePinLayout = this.FindViewById<RelativeLayout> (Resource.Id.remove_pin_layout);
			var setPinLayout = this.FindViewById<RelativeLayout> (Resource.Id.set_pin_layout);
			var changePinLayout = this.FindViewById<RelativeLayout> (Resource.Id.change_pin_layout);
//			RunOnUiThread (() => {
			switch (settingsPages) {
			case SettingsPinPages.ConfirmPin:
				removePinLayout.Visibility = ViewStates.Visible;
				changePinLayout.Visibility = ViewStates.Visible;
				setPinLayout.Visibility = ViewStates.Gone;
				break;
			case SettingsPinPages.RemovePin:
				removePinLayout.Visibility = ViewStates.Gone;
				changePinLayout.Visibility = ViewStates.Gone;
				setPinLayout.Visibility = ViewStates.Visible;
				break;
			}
//			});
		}

		public void BalanceClick ()
		{
			//GAService.GetGASInstance ().Track_App_Page (_myPageIndex);
			pager.SetCurrentItem (_myPageIndex, false);
		}

		public void GoToMerchants (int organisationId)
		{
			OrgId = organisationId;
			//GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.MapClicked, GAServiceHelper.From.FromPartnersDetails);
			//GAService.GetGASInstance ().Track_App_Page (_arounMeIndex);
			Intent intent = new Intent (this, typeof(MerchantsView));
			intent.PutExtra ("OrgId", organisationId);

			_isInnerActivity = true;
			StartActivity (intent);
//			pager.CurrentItem = _arounMeIndex;
		}

		public void CatalogClick ()
		{
			//GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.CatalogClicked, GAServiceHelper.From.FromHomePage);
			//GAService.GetGASInstance ().Track_App_Page (_catalogIndex);
			pager.SetCurrentItem (_catalogIndex, false);
		}

		public void AlertClick ()
		{
			//GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.NewsClicked, GAServiceHelper.From.FromNotifications);
			//GAService.GetGASInstance ().Track_App_Page (_newsIndex);
			pager.SetCurrentItem (_newsIndex, false);
		}

		public void LocationClick ()
		{
			//GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.MapClicked, GAServiceHelper.From.FromHomePage);
			//GAService.GetGASInstance ().Track_App_Page (4);
//			pager.SetCurrentItem (4, false);
			_isInnerActivity = true;
			StartActivity (typeof(MerchantsView));
		}

		public void HideTabs ()
		{
			_contentView.SetBackgroundResource (Resource.Color.green);
		}

		public void ShowTabs ()
		{
			_contentView.SetBackgroundResource (Resource.Color.orange);
		}

		#endregion

		#region Menu Controls

		public bool IsMenuShown { get; set; }

		public float OffSetForY  { get { return -10.0f; } }

		public float OffSetForX {
			get { 
				Display display = WindowManager.DefaultDisplay;
				Point size = new Point ();
				display.GetSize (size);
				int width = size.X;
				return width * 0.75f;
			}
		}

		#endregion

		#region Menu

		private float _widthPixels;
		private static readonly float _yFactor = 0.8f;
		private static float _rightY;

		float _ViewX;
		float prevX;
		bool moveRight;

		bool _isMoving;

		int _contentLayoutBackgoundResourceId = -1;

		public void ShowMenu ()
		{
			if (IsMenuShown)
				RootAnimate ((int)(_widthPixels * _yFactor), 0);
			else {
				RootAnimate (0, (int)(_widthPixels * _yFactor));
			}
		}

		public void MenuClick (int index)
		{
			if (IsAnimInProgress)
				return;
			_pagerIndex = index;
			RunOnUiThread (() => {
				ShowMenu ();
			});
			ChangeFragment ();
		}

		private void MapCleared (object sender, EventArgs e)
		{
			try {
				Intent intent = new Intent (this, typeof(LoginView));
				intent.SetFlags (ActivityFlags.ClearTop | ActivityFlags.NewTask);
				StartActivity (intent);
				merchantsFragment.MapCleared -= MapCleared;
             
				this.Finish ();
			} catch {
				Toast.MakeText (this, Resource.String.error_occured, ToastLength.Long).Show ();
			}
		}

		private void RootAnimate (int left, int right)
		{
			if (!Animator.IsStarted) {
				Animator.SetIntValues (left, right);
				Animator.Start ();
			}
		}

		void AnimatorAnimationEnd (object sender, EventArgs e)
		{
			_isMoving = false;
			if (_contentView.GetX () == 0) {
				IsMenuShown = false;
				//ChangeFragment ();
			} else
				IsMenuShown = true;

//			IsMenuShown = !IsMenuShown;
			if (!IsMenuShown) {
				_contentView.DisableChildTouch (false);

				_contentView.SetPadding (0, 0, 0, 0);
				_contentLayoutBackgoundResourceId = -1;
				_contentView.setRounded (false);
			} else {
				_contentView.DisableChildTouch (true);
			}
		}

		void ChangeFragment ()
		{
			OrgId = null;

			////GAService.GetGASInstance ().Track_App_Page (_pagerIndex);
			ClearBackStack ();
			switch (_pagerIndex) {
			case 0:
				pager.SetCurrentItem (0, false);
				break;
			case 1:
				pager.SetCurrentItem (1, false);
				break;
			case 2:
				////GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.CatalogClicked, GAServiceHelper.From.FromMenu);
				pager.SetCurrentItem (3, false);
				break;
			case 3:
				////GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.MapClicked, GAServiceHelper.From.FromMenu);
//				pager.SetCurrentItem (4, false);
				_isInnerActivity = true;
				StartActivity (typeof(MerchantsView));
				break;
			case 4:
				pager.SetCurrentItem (5, false);
				break;
			case 5:
				////GAService.GetGASInstance ().Track_App_Event ("News Clicked", "from menu");
				pager.SetCurrentItem (6, false);
				break;
			case 6:
				pager.SetCurrentItem (7, false);
				break;
			case 7:
				pager.SetCurrentItem (8, false);
				break;
			case 8:
				try {
					((MainViewModel)ViewModel).Logout ();
					if (LoginManager.Instance != null) {
						LoginManager.Instance.LogOut ();
					}
					Intent intent = new Intent (this, typeof(LoginView));
					intent.SetFlags (ActivityFlags.ClearTop | ActivityFlags.NewTask);
					this.Finish ();
					StartActivity (intent);
				} catch {
					Toast.MakeText (this, Resource.String.error_occured, ToastLength.Long).Show ();
				}
				break;
			default:
				break;
			}
		}

		private void AnimatorUpdate (object sender, ValueAnimator.AnimatorUpdateEventArgs e)
		{
			int left = (int)e.Animation.AnimatedValue;
			var right = (int)(left + _contentView.Width);
			_contentView.SetX (left);
			_contentView.ScaleY = (_widthPixels - left * 0.15f) / _widthPixels;

			_menuView.ScaleY = (_widthPixels - (_rightY - right) * 0.15f) / _widthPixels;
			_menuView.Alpha = 1 - (_rightY - right) / _rightY * 3;
			_menuView.Layout ((int)((_rightY - right) * 0.15), _menuView.Top, (int)((_rightY - right) * 0.15 + _menuView.Width), _menuView.Bottom);

			if (_contentLayoutBackgoundResourceId == -1) {
				_contentView.setRounded (true);
			}
		}

		public bool OnTouch (View v, MotionEvent e)
		{
			switch (v.Id) {
			case Resource.Id.main_fragment:
				switch (e.Action) {
				case MotionEventActions.Down:
					_ViewX = e.GetX ();
					prevX = e.RawX;
					_isMoving = false;
					break;
				case MotionEventActions.Move:
					var left = (int)(e.RawX - _ViewX);
					var right = (int)(left + v.Width);
					if (left >= _widthPixels * _yFactor || left <= 0)
						break;
					_isMoving = true;
					v.SetX (left);
					v.ScaleY = (_widthPixels - left * 0.15f) / _widthPixels;
					_menuView.ScaleY = (_widthPixels - (_rightY - right) * 0.15f) / _widthPixels;
					_menuView.Layout ((int)((_rightY - right) * 0.15), _menuView.Top, (int)((_rightY - right) * 0.15 + _menuView.Width), _menuView.Bottom);
					_menuView.Alpha = 1 - (_rightY - right) / _rightY * 3;
					if (_contentLayoutBackgoundResourceId == -1 && left > 10) {
						_contentView.setRounded (true);
					} else if (left <= 10) {
						//_contentLayoutBackgoundResourceId = -1;
						//_contentFrameLayout.Background = null;
						//_contentFrameLayout.SetPadding(0, 0, 0, 0);
					}
					if (e.RawX > prevX)
						moveRight = true;
					else if (e.RawX < prevX)
						moveRight = false;
					prevX = e.RawX;
					break;
				case MotionEventActions.Up:
					left = (int)(e.RawX - _ViewX);
					if (!_isMoving && IsMenuShown) {
						ShowMenu ();
						break;
					}
					if (left < 0)
						left = 0;
					if (left > _widthPixels * _yFactor) {
						left = (int)(_widthPixels * _yFactor);
					}
					if (!moveRight)
						RootAnimate (left, 0);
					else
						RootAnimate (left, (int)(_widthPixels * _yFactor));
					break;
				default:
					break;
				}
				break;
			}
			return true;
		}

		#endregion

		#region Pin Methods

		public  void OpenPinInputDialog (string pinFromDB)
		{
			_pinIsOpened = true;
			PinInputDialogFragment d = new PinInputDialogFragment (pinFromDB);
			d.Show (FragmentManager, "");
			d.SetStyle (DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
			d.Cancelable = false;
		}

		public void HideKeyboard ()
		{
			View View = CurrentFocus;
			if (View != null) {
				InputMethodManager inputManager = (InputMethodManager)GetSystemService (Context.InputMethodService);
				inputManager.HideSoftInputFromWindow (View.WindowToken, HideSoftInputFlags.None);
			}
		}

		public void ClearDigits (TextView _firstDigit,
		                         TextView _secondDigit,
		                         TextView _thirdDigit,
		                         TextView _fourthDigit)
		{
			_firstDigit.Text = "─";
			_secondDigit.Text = "─";
			_thirdDigit.Text = "─";
			_fourthDigit.Text = "─";
		}

		#endregion

		#region CardAnimation

		public bool IsCardActive { get; set; }

		public bool IsAnimInProgress { get; set; }

		public bool IsReplaced { get; set; }

		ImageView barcode;
		BaseTextView _bigCardNumber;
		ImageButton closeCard;
		RelativeLayout cardLayout;
		float height;

		public void CardAnimationClicked (object o, EventArgs e)
		{
			_isHomeSelected = pager.CurrentItem == _homeTabindex;

			if (_isHomeSelected) {
				_homeContentView = FindViewById<ScrollView> (Resource.Id.homeContentView);
			}
				
			closeCard = FindViewById<ImageButton> (Resource.Id.close_card);
			
			if (cardLayout == null || barcode == null) {
				cardLayout = FindViewById<RelativeLayout> (Resource.Id.mainCardRealativeLayout);
				barcode = FindViewById<ImageView> (Resource.Id.mainBarcode);

				if (!string.IsNullOrEmpty (_mainViewModel.CardNumber)) {
					using (Bitmap bitmap = BarcodeGenerator.Generate (_mainViewModel.CardNumber, activeUnicard.Width * 0.30f, activeUnicard.Height * 0.52f)) {
						Matrix matrix = new Matrix ();
						matrix.PostRotate (90);
						Bitmap rotatedBitmap = Bitmap.CreateBitmap (bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

						barcode.SetImageBitmap (rotatedBitmap);
					}
				}
				closeCard.Click += CardAnimationClicked;
				closeCard.Clickable = true;
			}

			if (!CardAnimator.IsStarted) {
				CardAnimator.SetIntValues ();
				CardAnimator.StartDelay = -100;
				CardAnimator.Start ();
			}
		}

		private void CardAnimatorUpdate (object sender, ValueAnimator.AnimatorUpdateEventArgs e)
		{
			if (IsAnimInProgress)
				return;
			IsAnimInProgress = true;
			if (IsCardActive) {
				HideCard (null, null);
			} else {
				ShowCard ();
			} 
		}

		void CardAnimatorAnimationEnd (object sender, EventArgs e)
		{
			if (!IsCardActive) {
				_menuView.Visibility = ViewStates.Visible;
				_parent_card_layout.Visibility = ViewStates.Invisible;
				if (_isHomeSelected) {
					_homeContentView.Visibility = ViewStates.Visible;
				}
			} else {
				_menuView.Visibility = ViewStates.Invisible;

			}
			IsAnimInProgress = false;
		}

		#region HomeCard properties

		private RelativeLayout _homeBottomLayout;

		public RelativeLayout HomeBottomLayout {
			get { 
				if (_homeBottomLayout == null) {
					_homeBottomLayout = FindViewById<RelativeLayout> (Resource.Id.toSeeRelativeLayout);
				}
				return _homeBottomLayout;
			}
		}

		private ImageView _homePageDivider;

		public ImageView HomePageDivider {
			get { 
				if (_homePageDivider == null) {
					_homePageDivider = FindViewById<ImageView> (Resource.Id.home_page_divider);
				}
				return _homePageDivider;
			}
		}

		private RelativeLayout _homeCardRealativeLayout;

		public RelativeLayout HomeCardRealativeLayout {
			get { 
				if (_homeCardRealativeLayout == null) {
					_homeCardRealativeLayout = FindViewById<RelativeLayout> (Resource.Id.cardRealativeLayout);
				}
				return _homeCardRealativeLayout;
			}
		}

		private ImageView _homeBarCode;

		public ImageView HomeBarCode {
			get { 
				if (_homeBarCode == null) {
					_homeBarCode = FindViewById<ImageView> (Resource.Id.barcode);
				}
				return _homeBarCode;
			}
		}

		private ImageView _backLine;

		public ImageView BackLine {
			get { 
				if (_backLine == null) {
					_backLine = FindViewById<ImageView> (Resource.Id.back_line);
				}
				return _backLine;
			}
		}

		private TextView _cardNumber;

		public TextView CardNumber {
			get { 
				if (_cardNumber == null) {
					_cardNumber = FindViewById<TextView> (Resource.Id.cardNumber);
				}
				return _cardNumber;
			}
		}

		private BasePointsTextView _txtTotalPoints;

		public BasePointsTextView TxtTotalPoints {
			get { 
				if (_txtTotalPoints == null) {
					_txtTotalPoints = FindViewById<BasePointsTextView> (Resource.Id.total_points);

				}
				return _txtTotalPoints;
			}
		}

		//		private BaseTextView _txtPoints;
		//
		//		public BaseTextView TxtPoints {
		//			get {
		//				if (_txtPoints == null) {
		//					_txtPoints = FindViewById<BaseTextView> (Resource.Id.txtPoints);
		//
		//				}
		//				return _txtPoints;
		//			}
		//		}

		private BaseTextView _txtClickUnicard;

		public BaseTextView TxtClickUnicard {
			get { 
				if (_txtClickUnicard == null) {
					_txtClickUnicard = FindViewById<BaseTextView> (Resource.Id.txt_click_Unicard);

				}
				return _txtClickUnicard;
			}
		}

		private RelativeLayout _home_page_toolbar;

		public RelativeLayout Home_page_toolbar {
			get { 
				if (_home_page_toolbar == null) {
					_home_page_toolbar = FindViewById<RelativeLayout> (Resource.Id.home_page_toolbar);

				}
				return _home_page_toolbar;
			}
		}

		private ScrollView _homeContentView;

		#endregion

		private void ShowCard ()
		{
			IsCardActive = true;
			_contentView.Animate ().TranslationY (height);
			_parent_card_layout.Visibility = ViewStates.Visible;
		}

		private string ConvertCardNumber (string value)
		{
			var builder = new StringBuilder ();
			int count = 0;
			foreach (var c in value) {
				builder.Append (c);
				if ((++count % 4) == 0) {
					builder.Append (' ');
				}
			}
			return builder.ToString ();
		}

		public void HideCard (object o, EventArgs e)
		{

//			_contentView.Visibility = ViewStates.Visible;
			_contentView.Animate ().TranslationY (0);
			if (_isHomeSelected) {
				Home_page_toolbar.Visibility = ViewStates.Visible;
				_homeContentView.ScrollTo (0, 0);
			}
			IsCardActive = false;
		}

		#endregion
	}
}
