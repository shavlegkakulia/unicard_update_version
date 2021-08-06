
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using Kuni.Core;
using Android.Telephony;
using Android.Text;
using MvvmCross.Binding.BindingContext;
using Kuni.Core.Models;
using Kunicardus.Droid.Helpers;
using Android.Support.V4.App;
using MvvmCross.Platforms.Android.Binding.Views;

namespace Kunicardus.Droid
{
	public class BuyProductFragment : MvxFragment, IOnBackPressedListener
	{
		MainView _context;

		private BuyProductViewModel _vm;

		MvxFragment fragment = null;

		public BuyProductFragment (BuyProductViewModel Viewmodel)
		{
			this.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Viewmodel;	
		
		}

		public string DisplayText {
			set {
				if (_vm.OperationCompleted) {
					ShowCompleteDialog ();
				} else {
					if (!string.IsNullOrEmpty (value)) {
						Toast.MakeText (_context, value, ToastLength.Long).Show ();
					}
				}
			}
			get{ return ""; }
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.BuyProductView, null);
			_vm = (BuyProductViewModel)this.ViewModel;
			_context = (this.Activity as MainView);
			_context.SetOnBackPressedListener (this);

			InitContentView (_vm);

			var backButton = View.FindViewById<ImageButton> (Resource.Id.backbtn);
			backButton.Click += BackPressed;

			var set = this.CreateBindingSet<BuyProductFragment, BuyProductViewModel> ();
			set.Bind (this).For (v => v.DisplayText).To (vmod => vmod.DisplayText);
			set.Apply (); 
			return View;
		}

		private void ShowCompleteDialog ()
		{
			var dialog = new Dialog (_context,
				             Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
			dialog.SetContentView (Resource.Layout.BuyProductComplete);
			dialog.SetCancelable (false);

			dialog.FindViewById<BaseButton> (Resource.Id.btnContinue).Click += (o, e) => {
				DoBack ();
				dialog.Dismiss ();
			};
			dialog.Show ();
		}

		public override void OnSaveInstanceState (Bundle outState)
		{
			//base.OnSaveInstanceState (outState);
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
		}


		public  void DoBack ()
		{
			BackPressed (null, null);
		}

		private void BackPressed (object sender, EventArgs e)
		{

			ChildFragmentManager.PopBackStack ();
			var transactions = _context.SupportFragmentManager.BeginTransaction ();
			transactions.Remove (this).Commit ();
			_context.SetOnBackPressedListener (null);
			_context.HideKeyboard ();
		}


		private void InitContentView (BuyProductViewModel vm)
		{
			
			switch (vm.DeliveryMethodId) {
			//ადგილზე მიტანა - 3
			case 3:
				fragment = new FetchFragment (vm);
				break;
		
			//პარტნიორი ორგანიზაციიდან გატანა 2
			//სერვის ცენტრიდან 1
			//შოუ რუმიდან 5
			case 2:
			case 1:
			case 5:
				fragment = new ServiceCenterFragment (vm);
				break;

			//მომენტალური მიღება 4
			//მომენტალური გადახდა 10
			case 10:
			case 4:
				if (vm.ProductTypeID == 5) {
					TelephonyManager tMgr = (TelephonyManager)_context.GetSystemService (Context.TelephonyService);
					String mPhoneNumber = tMgr.Line1Number;
					vm.OnlinePaymentIdentifier = mPhoneNumber;
					fragment = new MobilePaymentFragment (vm);
				} else {
					fragment = new OnlinePaymentFragment (vm);
				}
				break;
			default:
				break;
			}
			if (fragment != null) {
				ChildFragmentManager.BeginTransaction ().Add (Resource.Id.contentFrame, fragment).Commit ();
				if (fragment.View != null) {
					fragment.View.KeyPress += (o, e) => {
						if (e.KeyCode == Keycode.Back) {
							BackPressed (null, null);
						}
					};
				} 
			} 

			if (_context._dialog != null) {
				_context._dialog.Dismiss ();
			}
		}

		public override void OnViewCreated (View View, Bundle savedInstanceState)
		{
			var _disList = this.View.FindViewById<MvxListView> (Resource.Id.discountList);
			if (_disList != null) {
				Utils.SetListViewHeightBasedOnChildren (_disList);
			}

		}
	}

	public class FetchFragment : BaseContentFragment
	{
		public FetchFragment (BuyProductViewModel Viewmodel) : base (Viewmodel)
		{
			this.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Viewmodel;
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.DeliveryView, null);
			var layout = View.FindViewById<LinearLayout> (Resource.Id.delivery_view_layout);
			layout.Click += (sender, e) => {
				(this.Activity as MainView).HideKeyboard ();
			};
			return View;
		}
	}

	public class ServiceCenterFragment : BaseContentFragment
	{
		public ServiceCenterFragment (BuyProductViewModel Viewmodel) : base (Viewmodel)
		{
			ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Viewmodel;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.ServiceCenterView, null);
			var layout = View.FindViewById<LinearLayout> (Resource.Id.delivery_view_layout);
			layout.Click += (sender, e) => {
				(this.Activity as MainView).HideKeyboard ();
			};
			return View;
		}
	}

	public class PartnetCompFragment : BaseContentFragment
	{

		public PartnetCompFragment (BuyProductViewModel Viewmodel) : base (Viewmodel)
		{
			ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Viewmodel;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.PartnerCompView, null);
			var layout = View.FindViewById<LinearLayout> (Resource.Id.delivery_view_layout);
			layout.Click += (sender, e) => {
				(this.Activity as MainView).HideKeyboard ();
			};
			return View;
		}
	}

	public class MobilePaymentFragment : BaseContentFragment
	{
		BaseEditText _txtMobileNumber;

		public MobilePaymentFragment (BuyProductViewModel Viewmodel) : base (Viewmodel)
		{
			ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Viewmodel;
		
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.MobilePaymentView, null);
			_txtMobileNumber = View.FindViewById<BaseEditText> (Resource.Id.txtMobileNumber);
			_txtMobileNumber.AddTextChangedListener (new PhoneNumberFormattingTextWatcher ());
			var layout = View.FindViewById<LinearLayout> (Resource.Id.delivery_view_layout);
			layout.Click += (sender, e) => {
				(this.Activity as MainView).HideKeyboard ();
			};
			return View;
		}
	}

	public class OnlinePaymentFragment : BaseContentFragment
	{
		MvxListView _infoList;
		public bool _changed;

		public bool DataChanged {
			get{ return _changed; }
			set { 
				Utils.SetListViewHeightBasedOnChildren (_infoList);
			}
		}

		public bool CheckClicked {
			get{ return true; }
			set { 
				if (value) {
					ActivityHelpers.HideSoftKeyboard (this.Activity);
				}
			}
		}


		public OnlinePaymentFragment (BuyProductViewModel Viewmodel) : base (Viewmodel)
		{
			ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Viewmodel;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.OnlinePaymentView, null);
	
			_infoList = View.FindViewById<MvxListView> (Resource.Id.infolistView);
			var set = this.CreateBindingSet<OnlinePaymentFragment, BuyProductViewModel> ();
			set.Bind (this).For (v => v.DataChanged).To (vm => vm.DataChanged);
			set.Bind (this).For (v => v.CheckClicked).To (vm => vm.CheckClicked);
			set.Apply ();

			var layout = View.FindViewById<LinearLayout> (Resource.Id.delivery_view_layout);
			layout.Click += (sender, e) => {
				(this.Activity as MainView).HideKeyboard ();
			};
			return View;
		}

	}

	public class BaseContentFragment : MvxFragment, MvxListView.IOnItemClickListener
	{
		MvxListView _disList;
		BuyProductViewModel vm;

		public BaseContentFragment (BuyProductViewModel Viewmodel)
		{
			vm = Viewmodel;
		}

		public override void OnViewCreated (View View, Bundle savedInstanceState)
		{
			_disList = View.FindViewById<MvxListView> (Resource.Id.discountList);
			var disheader = View.FindViewById<RelativeLayout> (Resource.Id.discountheader);
			if (disheader != null) {
				disheader.Visibility = vm.HasDiscount ? ViewStates.Visible : ViewStates.Gone;	
			}
			if (_disList != null) {
				_disList.OnItemClickListener = this;
				_disList.SetScrollContainer (false);
				Utils.SetListViewHeightBasedOnChildren (_disList);
			}

			BaseTextView txtdesc = View.FindViewById<BaseTextView> (Resource.Id.txtDescription);
			if (txtdesc != null) {
				txtdesc.TextFormatted = Html.FromHtml (((BuyProductViewModel)this.ViewModel).Note);
				txtdesc.TextSize = 10;
			}
		}

		public void OnItemClick (AdapterView parent, View View, int position, long id)
		{
			//delesect old item
			if (vm.SelecteDPostion >= 0) {
				var oldView = _disList.GetViewByPosition (vm.SelecteDPostion);
				if (oldView != null) {
					var imgv = oldView.FindViewById<ImageView> (Resource.Id.imgSelected);
					imgv.SetImageResource (Resource.Drawable.radioIcon);
				}
			}
			//select new discoutnt
			if (vm.SelecteDPostion != position) {
				var discobj = vm.UserDiscounts [position];
				vm.SelectedDiscount = discobj;
				vm.SelecteDPostion = position;
				View.FindViewById<ImageView> (Resource.Id.imgSelected).SetImageResource (Resource.Drawable.radioIconActive);
			} else {
				vm.SelecteDPostion = -1;
				vm.SelectedDiscount = null;
			}
		}
	}
}

