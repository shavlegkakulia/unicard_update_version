
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross;
using Kuni.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
//using MvvmCross.Binding.Droid.Views;

namespace Kunicardus.Droid.Fragments
{
	public class OrganisationDetailsFragment : MvxFragment
	{
		private int _organisationId;
		OrganizationDetailsViewModel _ViewModel;

		#region UI

		private LinearLayout _email;
		private LinearLayout _phone;
		private LinearLayout _webpage;
		private LinearLayout _fb_page;
		private LinearLayout _discription;
		private LinearLayout _workingHours;

		private View _linePhone;
		private View _lineMail;
		private View _linePage;
		private View _lineFB;
		private View _lineDesc;
		private View _lineHours;

		private bool _fromMerchant;
		ListView _phoneList;

		#endregion

		public OrganisationDetailsFragment (int organisationId, bool fromMerchant = false)
		{
			_organisationId = organisationId;
			_fromMerchant = fromMerchant;
            this.ViewModel = Mvx.IoCProvider.IoCConstruct<OrganizationDetailsViewModel>();
			_ViewModel = (OrganizationDetailsViewModel)this.ViewModel;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);					
			var View = this.BindingInflate (Resource.Layout.PartnerDetailsLayout, null);
		
//			var mainView = base.Activity;
			var backButton = View.FindViewById<ImageView> (Resource.Id.partner_details_back);
			var showMerchants = View.FindViewById<Kunicardus.Droid.BaseButton> (Resource.Id.object_list);
			showMerchants.Click += delegate {
				if (_fromMerchant) {
					((MerchantsView)Activity).OrgId = _organisationId;
					((MerchantsView)Activity).RequestLocationPermissions();
					this.Activity.SupportFragmentManager.BeginTransaction ().Remove (this).Commit ();

				} else {
					((MainView)this.Activity).GoToMerchants (_organisationId);
//					this.Activity.SupportFragmentManager.BeginTransaction ().Hide (this).Commit ();
				}
			};
			backButton.Click += delegate {
				if (_fromMerchant) {
					this.Activity.SupportFragmentManager.BeginTransaction ().Remove (this).Commit ();

				} else {
					((MainView)Activity).ClearBackStack ();
				}
			};				

			#region Open thierd party tool handling

			_email = View.FindViewById<LinearLayout> (Resource.Id.l_email);
			_fb_page = View.FindViewById<LinearLayout> (Resource.Id.l_fb);
			_webpage = View.FindViewById<LinearLayout> (Resource.Id.l_webpage);
			_phoneList = View.FindViewById<MvxListView> (Resource.Id.phoneList);
			_discription = View.FindViewById<LinearLayout> (Resource.Id.description);
			_workingHours = View.FindViewById<LinearLayout> (Resource.Id.workinghours);

			_linePhone = View.FindViewById<View> (Resource.Id.line_phone);
			_lineMail = View.FindViewById<View> (Resource.Id.line_mail);
			_linePage = View.FindViewById<View> (Resource.Id.line_page);
			_lineFB = View.FindViewById<View> (Resource.Id.line_fb);
			_lineDesc = View.FindViewById<View> (Resource.Id.line_Desc);
			_lineHours = View.FindViewById<View> (Resource.Id.line_hour);

			_email.Click += delegate {
				var email_text = View.FindViewById<TextView> (Resource.Id.email_text);
				if (!string.IsNullOrWhiteSpace (email_text.Text)) {
					try {
						var email = new Intent (Android.Content.Intent.ActionSendto, Android.Net.Uri.Parse (string.Format ("mailto:{0}", email_text.Text)));
						email.PutExtra (Android.Content.Intent.ExtraEmail,
							new string[]{ email_text.Text });
						//email.SetType ("message/rfc822");
						StartActivity (email);
					} catch {
						Toast.MakeText (Activity, Resource.String.error_occured, ToastLength.Long).Show ();
					}
				}
			};
			_fb_page.Click += delegate {
				var fb_text = View.FindViewById<TextView> (Resource.Id.fb_text);
				if (!string.IsNullOrWhiteSpace (fb_text.Text)) {
					OpenWebPage (fb_text.Text);
				}
			};

			_webpage.Click += delegate {
				var webpage_text = View.FindViewById<TextView> (Resource.Id.webpage_text);
				if (!string.IsNullOrWhiteSpace (webpage_text.Text)) {
					OpenWebPage (webpage_text.Text);
				}
			};

			#endregion

			var set = this.CreateBindingSet<OrganisationDetailsFragment, OrganizationDetailsViewModel> ();
			set.Bind (this).For (v => v.DisplayText).To (vmod => vmod.DataPopulated);
			set.Bind (this).For (v => v.NumberToCall).To (vmod => vmod.NumberToCall);
			set.Apply (); 

			return View;
		}

		public bool DisplayText {
			set {
				if (value) {
					HideLayouts ();
				}
			}
			get{ return false; }
		}

		public string NumberToCall {
			set {
				if (!string.IsNullOrEmpty (value)) {

					string phone = value.Replace ("(", "").Replace (")", "").Replace (" ", "").Trim ();
					if (!string.IsNullOrWhiteSpace (phone)) {
						try {
							var uri = Android.Net.Uri.Parse (string.Format ("tel:{0}", phone));
							var intent = new Intent (Intent.ActionDial, uri);
							StartActivity (intent);
						} catch {
							Toast.MakeText (Activity, Resource.String.error_occured, ToastLength.Long).Show ();
						}
					}
				}
			}
			get{ return string.Empty; }
		}

		void HideLayouts ()
		{
			Utils.SetSimpleListViewHeight (_phoneList);
			if (string.IsNullOrEmpty (_ViewModel.Description)) {
				_discription.Visibility = ViewStates.Gone;
				_lineDesc.Visibility = ViewStates.Gone;
			}
			if (string.IsNullOrEmpty (_ViewModel.FbLink)) {
				_fb_page.Visibility = ViewStates.Gone;
				_lineFB.Visibility = ViewStates.Gone;
			}
			if (string.IsNullOrEmpty (_ViewModel.Mail)) {
				_email.Visibility = ViewStates.Gone;
				_lineMail.Visibility = ViewStates.Gone;
			}
			if (string.IsNullOrEmpty (_ViewModel.Phone)) {
				_phone.Visibility = ViewStates.Gone;
				_linePhone.Visibility = ViewStates.Gone;
			}
			if (string.IsNullOrEmpty (_ViewModel.Website)) {
				_webpage.Visibility = ViewStates.Gone;
				_linePage.Visibility = ViewStates.Gone;
			}
			if (string.IsNullOrEmpty (_ViewModel.WorkingHours)) {
				_workingHours.Visibility = ViewStates.Gone;
				_lineHours.Visibility = ViewStates.Gone;
			}
		}

		public override void OnViewCreated (View View, Bundle savedInstanceState)
		{
			base.OnViewCreated (View, savedInstanceState);
			_ViewModel.InitData (_organisationId);
		}

		#region Methods

		private void OpenWebPage (string address)
		{
			try {
				if (!address.ToLower ().Contains ("http") && !address.ToLower ().Contains ("https")) {
					address = "http://" + address;
				}
				var uri = Android.Net.Uri.Parse (address);
				var intent = new Intent (Intent.ActionView, uri);
				StartActivity (intent);
			} catch {
				Toast.MakeText (Activity, Resource.String.error_occured, ToastLength.Long).Show ();
			}
		}

		#endregion
	}
}

