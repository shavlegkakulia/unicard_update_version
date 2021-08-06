//using System;
//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Views;
//using Android.Widget;
//
//using Android.Animation;
//using Cirrious.MvvmCross.Droid.Fragging;
//using Android.Graphics;
//
//namespace Kunicardus.Droid
//{
//	[Activity (Label = "BaseView")]			
//	public class BaseView : MvxFragmentActivity
//	{
//		private FrameLayout _menuView;
//		private CustomFrameLayout _contentView;
//		private ValueAnimator _animator;
//		MenuFragment menu;
//
//		public ValueAnimator Animator {
//			get {
//				if (_animator == null) {
//					_animator = ValueAnimator.OfInt (0, 0);
//					_animator.Update += AnimatorUpdate;
//					_animator.SetDuration (150);
//					_animator.AnimationEnd += AnimatorAnimationEnd;
//				}
//				return _animator;
//			}
//		}
//
//		public bool IsCardActive { get; set; }
//
//		public bool IsAnimInProgress { get; set; }
//
//		public bool IsReplaced { get; set; }
//
//		protected override void OnCreate (Bundle bundle)
//		{
//			base.OnCreate (bundle);
//
//			SetContentView (Resource.Layout.MainView);
//		}
//
//		protected override void OnViewModelSet ()
//		{
//			base.OnViewModelSet ();
//
//			menu = new MenuFragment ();
//			SupportFragmentManager.BeginTransaction ().Add (Resource.Id.menu_fragment, menu).Commit ();
//		}
//
//		#region Menu Controls
//
//		public bool IsMenuShown { get; set; }
//
//		public float OffSetForY  { get { return -10.0f; } }
//
//		public float OffSetForX {
//			get { 
//				Display display = WindowManager.DefaultDisplay;
//				var size = new Point ();
//				display.GetSize (size);
//				int width = size.X;
//				return width * 0.75f;
//			}
//		}
//
//		#endregion
//
//		#region Menu
//
//		private float _widthPixels;
//		private static readonly float _yFactor = 0.8f;
//		private static float _rightY;
//
//		float _ViewX;
//		float prevX;
//		bool moveRight;
//
//		bool _isMoving;
//
//		int _contentLayoutBackgoundResourceId = -1;
//
//		public void ShowMenu ()
//		{
//			if (IsMenuShown)
//				RootAnimate ((int)(_widthPixels * _yFactor), 0);
//			else {
//				RootAnimate (0, (int)(_widthPixels * _yFactor));
//			}
//		}
//
//		public void MenuClick (int index)
//		{
//
//			//	_pagerIndex = index;
//			ChangeFragment ();
//			RunOnUiThread (() => {
//				ShowMenu ();
//			});
//		}
//
//		//		private void MapCleared (object sender, EventArgs e)
//		//		{
//		//			try {
//		//				Intent intent = new Intent (this, typeof(LoginView));
//		//				intent.SetFlags (ActivityFlags.ClearTop | ActivityFlags.NewTask);
//		//				StartActivity (intent);
//		//				merchantsFragment.MapCleared -= MapCleared;
//		//
//		//				this.Finish ();
//		//			} catch {
//		//				Toast.MakeText (this, Resource.String.error_occured, ToastLength.Long).Show ();
//		//			}
//		//		}
//
//		private void RootAnimate (int left, int right)
//		{
//			if (!Animator.IsStarted) {
//				Animator.SetIntValues (left, right);
//				Animator.Start ();
//			}
//		}
//
//		void AnimatorAnimationEnd (object sender, EventArgs e)
//		{
//			_isMoving = false;
//			if (_contentView.GetX () == 0) {
//				IsMenuShown = false;
//				//ChangeFragment ();
//			} else
//				IsMenuShown = true;
//
//			//			IsMenuShown = !IsMenuShown;
//			if (!IsMenuShown) {
//				_contentView.DisableChildTouch (false);
//
//				_contentView.SetPadding (0, 0, 0, 0);
//				_contentLayoutBackgoundResourceId = -1;
//				_contentView.setRounded (false);
//			} else {
//				_contentView.DisableChildTouch (true);
//			}
//		}
//
//		void ChangeFragment ()
//		{
//			OrgId = null;
//
//			GAService.GetGASInstance ().Track_App_Page (_pagerIndex);
//			ClearBackStack ();
//			switch (_pagerIndex) {
//			case 0:
//				pager.SetCurrentItem (0, false);
//				break;
//			case 1:
//				pager.SetCurrentItem (1, false);
//				break;
//			case 2:
//				GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.CatalogClicked, GAServiceHelper.From.FromMenu);
//				pager.SetCurrentItem (3, false);
//				break;
//			case 3:
//				GAService.GetGASInstance ().Track_App_Event (GAServiceHelper.Events.MapClicked, GAServiceHelper.From.FromMenu);
//				pager.SetCurrentItem (4, false);
//				break;
//			case 4:
//				pager.SetCurrentItem (5, false);
//				break;
//			case 5:
//				GAService.GetGASInstance ().Track_App_Event ("News Clicked", "from menu");
//				pager.SetCurrentItem (6, false);
//				break;
//			case 6:
//				pager.SetCurrentItem (7, false);
//				break;
//			case 7:
//				pager.SetCurrentItem (8, false);
//				break;
//			case 8:
//				try {
//					((MainViewModel)ViewModel).Logout ();
//					if (LoginManager.Instance != null) {
//						LoginManager.Instance.LogOut ();
//					}
//					Intent intent = new Intent (this, typeof(LoginView));
//					intent.SetFlags (ActivityFlags.ClearTop | ActivityFlags.NewTask);
//					this.Finish ();
//					StartActivity (intent);
//				} catch {
//					Toast.MakeText (this, Resource.String.error_occured, ToastLength.Long).Show ();
//				}
//				break;
//			default:
//				break;
//			}
//		}
//
//		private void AnimatorUpdate (object sender, ValueAnimator.AnimatorUpdateEventArgs e)
//		{
//			int left = (int)e.Animation.AnimatedValue;
//			var right = (int)(left + _contentView.Width);
//			_contentView.SetX (left);
//			_contentView.ScaleY = (_widthPixels - left * 0.15f) / _widthPixels;
//
//			_menuView.ScaleY = (_widthPixels - (_rightY - right) * 0.15f) / _widthPixels;
//			_menuView.Alpha = 1 - (_rightY - right) / _rightY * 3;
//			_menuView.Layout ((int)((_rightY - right) * 0.15), _menuView.Top, (int)((_rightY - right) * 0.15 + _menuView.Width), _menuView.Bottom);
//
//			if (_contentLayoutBackgoundResourceId == -1) {
//				_contentView.setRounded (true);
//			}
//		}
//
//		public bool OnTouch (View v, MotionEvent e)
//		{
//			switch (v.Id) {
//			case Resource.Id.main_fragment:
//				switch (e.Action) {
//				case MotionEventActions.Down:
//					_ViewX = e.GetX ();
//					prevX = e.RawX;
//					_isMoving = false;
//					break;
//				case MotionEventActions.Move:
//					var left = (int)(e.RawX - _ViewX);
//					var right = (int)(left + v.Width);
//					if (left >= _widthPixels * _yFactor || left <= 0)
//						break;
//					_isMoving = true;
//					v.SetX (left);
//					v.ScaleY = (_widthPixels - left * 0.15f) / _widthPixels;
//					_menuView.ScaleY = (_widthPixels - (_rightY - right) * 0.15f) / _widthPixels;
//					_menuView.Layout ((int)((_rightY - right) * 0.15), _menuView.Top, (int)((_rightY - right) * 0.15 + _menuView.Width), _menuView.Bottom);
//					_menuView.Alpha = 1 - (_rightY - right) / _rightY * 3;
//					if (_contentLayoutBackgoundResourceId == -1 && left > 10) {
//						_contentView.setRounded (true);
//					} else if (left <= 10) {
//						//_contentLayoutBackgoundResourceId = -1;
//						//_contentFrameLayout.Background = null;
//						//_contentFrameLayout.SetPadding(0, 0, 0, 0);
//					}
//					if (e.RawX > prevX)
//						moveRight = true;
//					else if (e.RawX < prevX)
//						moveRight = false;
//					prevX = e.RawX;
//					break;
//				case MotionEventActions.Up:
//					left = (int)(e.RawX - _ViewX);
//					if (!_isMoving && IsMenuShown) {
//						ShowMenu ();
//						break;
//					}
//					if (left < 0)
//						left = 0;
//					if (left > _widthPixels * _yFactor) {
//						left = (int)(_widthPixels * _yFactor);
//					}
//					if (!moveRight)
//						RootAnimate (left, 0);
//					else
//						RootAnimate (left, (int)(_widthPixels * _yFactor));
//					break;
//				default:
//					break;
//				}
//				break;
//			}
//			return true;
//		}
//
//		#endregion
//	}
//}
//
