using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Kuni.Core;
using Android.Graphics;


namespace Kunicardus.Droid
{
	public class ChooseCardExistanceViewFragment : MvxFragment,IDialogInterfaceOnClickListener
	{
		public void OnClick (IDialogInterface dialog, int which)
		{
			if (which == -2)
				_activity.OnBackPressed ();
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			_activity = ((BaseRegisterView)base.Activity);
		}


		private TextView _unicardNotAvailable, _unicardAvailable;
		private BaseRegisterView _activity;

		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.ChooseCardExistanceView, null);

			var backToolbar = View.FindViewById<RelativeLayout> (Resource.Id.backbuttonToolbar);
			var backLayout = backToolbar.FindViewById<LinearLayout> (Resource.Id.back_button_layout);

			var toolbarTitle = backToolbar.FindViewById<BaseTextView> (Resource.Id.toolbar_title);
			toolbarTitle.Text = Resources.GetString (Resource.String.registration);

			backLayout.Click += (sender, e) => {
				this.Activity.OnBackPressed ();
			};
			var mainRegisterViewModel = (this.Activity as BaseRegisterView).ViewModel as BaseRegisterViewModel;
			if (!string.IsNullOrEmpty (mainRegisterViewModel.FBRegisterViewModelProperty.FBId)) {
				AlertDialog.Builder builder = new AlertDialog.Builder (this.Activity);
				builder.SetTitle (Resources.GetString (Resource.String.fbRegistration));
				builder.SetCancelable (false);
				builder.SetMessage (Resources.GetString (Resource.String.fb_not_registered));
				builder.SetPositiveButton ("დიახ", this);
				builder.SetNegativeButton ("არა", this);
				AlertDialog alert = builder.Create ();
				alert.Show ();
			}

			_unicardNotAvailable = View.FindViewById<BaseTextView> (Resource.Id.unicard_not_available);
			_unicardNotAvailable.Click += (sender, e) => {
				_unicardNotAvailable.SetTextColor (Color.ParseColor ("#ffd158"));
				_unicardAvailable.SetTextColor (Color.ParseColor ("#ffffff"));
				//var mainRegisterViewModel = (this.Activity as BaseRegisterView).ViewModel as BaseRegisterViewModel;
				(_activity).NewCardRegistration = (this.ViewModel as ChooseCardExistanceViewModel).NewCardRegistration;
				if (!string.IsNullOrEmpty (mainRegisterViewModel.FBRegisterViewModelProperty.FBId)) {
					(_activity).OpenFbRegisterFragment (null);
				} else {
					(_activity).OpenRegistrationFragment (null);
				}

			};

			_unicardAvailable = View.FindViewById<BaseTextView> (Resource.Id.unicard_available);
			_unicardAvailable.Click += (sender, e) => {
				_unicardAvailable.SetTextColor (Color.ParseColor ("#ffd158"));
				_unicardNotAvailable.SetTextColor (Color.ParseColor ("#ffffff"));
				(_activity).NewCardRegistration = (this.ViewModel as ChooseCardExistanceViewModel).NewCardRegistration;
				(_activity).OpenUnicardInputFragment ();

			};

			return View;
		}
	}
}

