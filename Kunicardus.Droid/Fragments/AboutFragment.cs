using System;
using Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Android.Widget;
using Android.Content;
using MvvmCross;
using Kuni.Core.ViewModels;
using System.Threading;

namespace Kunicardus.Droid
{
	public class AboutFragment : BaseMvxFragment
	{
		#region implemented abstract members of BaseMvxFragment

		private LinearLayout _email, _webpage, _fb_page;

		#region implemented abstract members of BaseMvxFragment

		public override void OnActivate ()
		{
			if (this.ViewModel == null) {
				this.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCConstruct<AboutViewModel>();	
			}
			StartWorker();
		}

		public void StartWorker()
		{
			new Thread(() =>
			{
				Thread.CurrentThread.IsBackground = true;
				//if (string.IsNullOrEmpty (((AboutViewModel)ViewModel).Mail))
				((AboutViewModel)ViewModel).PopulateData ();
			}).Start();
		}

		#endregion

		#endregion

		public override View OnCreateView (Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.AboutUsView, null);

			ImageButton	menu = View.FindViewById<ImageButton> (Resource.Id.menuImg);
			menu.Click += (o, e) => ((MainView)base.Activity).ShowMenu ();
			var title = View.FindViewById<TextView> (Resource.Id.pageTitle);
			title.Text = Resources.GetString (Resource.String.aboutus);
			View.FindViewById<ImageView> (Resource.Id.alert).Visibility = ViewStates.Invisible;
			View.FindViewById<ImageView> (Resource.Id.merchants).Visibility = ViewStates.Gone;

			#region Open thierd party tool handling

			_email = View.FindViewById<LinearLayout> (Resource.Id.l_email);
			_fb_page = View.FindViewById<LinearLayout> (Resource.Id.l_fb);
			_webpage = View.FindViewById<LinearLayout> (Resource.Id.l_webpage);

			LinearLayout phone = View.FindViewById<LinearLayout> (Resource.Id.l_phone);

			var share = View.FindViewById<LinearLayout> (Resource.Id.share);
			var rate = View.FindViewById<LinearLayout> (Resource.Id.rate);


			rate.Click += (object sender, EventArgs e) => {
				Android.Net.Uri uri = Android.Net.Uri.Parse ("market://details?id=" + this.Activity.PackageName);
				Intent myAppLinkToMarket = new Intent (Intent.ActionView, uri);
				try {
					StartActivity (myAppLinkToMarket);
				} catch (ActivityNotFoundException ex) {
					Toast.MakeText (this.Activity, "Unable To Find Market App", ToastLength.Long).Show ();
				}
			};

			share.Click += (object sender, EventArgs e) => {
				try {
					Intent sendIntent = new Intent ();
					sendIntent.SetAction (Intent.ActionSend);
					sendIntent.PutExtra (Intent.ExtraText, string.Format ("https://play.google.com/store/apps/details?id={0}", this.Activity.PackageName));
					sendIntent.SetType ("text/plain");
					StartActivity (Intent.CreateChooser (sendIntent, "აირჩიეთ აპლიკაცია"));
				} catch {
					Toast.MakeText (Activity, Resource.String.error_occured, ToastLength.Long).Show ();
				}
			};

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
			phone.Click += delegate {              
				var phone_text = View.FindViewById<TextView> (Resource.Id.phone_text);
				if (!string.IsNullOrWhiteSpace (phone_text.Text)) {
					string phonet = phone_text.Text.Replace ("(", "").Replace (")", "").Replace (" ", "").Trim ();
					if (!string.IsNullOrWhiteSpace (phonet)) {
						try {
							var uri = Android.Net.Uri.Parse (string.Format ("tel:{0}", phonet));
							var intent = new Intent (Intent.ActionDial, uri);
							StartActivity (intent);
						} catch {
							Toast.MakeText (Activity, Resource.String.error_occured, ToastLength.Long).Show ();
						}
					}
				}
			};
			_webpage.Click += delegate {
				var webpage_text = View.FindViewById<TextView> (Resource.Id.webpage_text);
				if (!string.IsNullOrWhiteSpace (webpage_text.Text)) {
					OpenWebPage (webpage_text.Text);
				}
			};

			#endregion

			return View;
		}

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
	}
}

