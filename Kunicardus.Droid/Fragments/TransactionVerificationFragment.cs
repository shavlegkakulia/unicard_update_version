using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Kuni.Core;
using Android.Views.InputMethods;
using MvvmCross.Binding.BindingContext;

namespace Kunicardus.Droid
{
	public class TransactionVerificationFragment : MvxFragment
	{
		private BaseRegisterView _activity;
		private BaseEditText _dateEditText;
		private TransactionVerificationViewModel _currentViewModel;
		private string _unicardNumber;
		private bool _dataPopulated;

		public bool DataPopulated {	
			get { return _dataPopulated; }
			set {
				_dataPopulated = value;
				if (_dataPopulated) {
					if (_currentViewModel.LastTransactionStatus) {
						Bundle b = new Bundle ();
						b.PutString ("unicard_number", _unicardNumber);
						var parentViewModel = (this.Activity as BaseRegisterView).ViewModel as BaseRegisterViewModel;
						if (string.IsNullOrEmpty (parentViewModel.FBRegisterViewModelProperty.FBId))
							(this.Activity as BaseRegisterView).OpenRegistrationFragment (b);
						else
							(this.Activity as BaseRegisterView).OpenFbRegisterFragment (b);
					}
				}
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.TransactionVerificationView, null);

			var layout = View.FindViewById<RelativeLayout> (Resource.Id.regRelativeLayout);
			layout.Click += HideKeyboard;
			_activity = ((BaseRegisterView)base.Activity);
			var backToolbar = View.FindViewById<RelativeLayout> (Resource.Id.backbuttonToolbar);
			var backLayout = backToolbar.FindViewById<LinearLayout> (Resource.Id.back_button_layout);
			backLayout.Click += BackClick;
			var toolbarTitle = backToolbar.FindViewById<BaseTextView> (Resource.Id.toolbar_title);
			toolbarTitle.Text = Resources.GetString (Resource.String.registration);

			_currentViewModel = (this.ViewModel as TransactionVerificationViewModel);
			_unicardNumber = Arguments.GetString ("unicard_number");

			if (!string.IsNullOrWhiteSpace (_unicardNumber))
				_currentViewModel.UnicardNumber = _unicardNumber;
			
			DatePickerDialog d = new DatePickerDialog (
				                     this.Activity,
				                     (object sender2, DatePickerDialog.DateSetEventArgs e2) => {
					_currentViewModel.Date = e2.Date;
				},
				                     DateTime.Now.Year,
				                     DateTime.Now.Month,
				                     DateTime.Now.Day);


			_dateEditText = View.FindViewById<BaseEditText> (Resource.Id.txt_verify_card_date);
			_dateEditText.FocusChange += (object sender, View.FocusChangeEventArgs e) => {
				if (_dateEditText.IsFocused)
					this.Activity.RunOnUiThread (() => {
						d.Show ();
					});
			};
			_dateEditText.Click += (object sender, EventArgs e) => {
				if (!d.IsShowing)
					d.Show ();
			};

			_currentViewModel.DataPopulated = false;
			var set = this.CreateBindingSet<TransactionVerificationFragment, TransactionVerificationViewModel> ();
			set.Bind (this).For (v => v.DataPopulated).To (vmod => vmod.DataPopulated);
			set.Apply (); 

			return View;
		}

		private void HideKeyboard (object sender, EventArgs e)
		{
			View View = base.Activity.CurrentFocus;
			if (View != null) {
				InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService (Context.InputMethodService);
				inputManager.HideSoftInputFromWindow (View.WindowToken, HideSoftInputFlags.NotAlways);
			}
		}

		void BackClick (object sender, EventArgs e)
		{
			HideKeyboard (this, null);
			_activity.BackToPreviousFragment ();
		}
	}
}


