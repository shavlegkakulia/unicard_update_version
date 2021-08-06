using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.BusinessModels;
using Kunicardus.Core.Helpers.Device;

namespace Kunicardus.Core.ViewModels
{
	public class AboutViewModel : BaseViewModel
	{
        
		#region Variables

		private IContactService _contactService;

		#endregion

		#region Constructor Implementation

		public AboutViewModel (IContactService contactService, IDevice device)
		{			
			_contactService = contactService;
			if (device.Platform == "ios") {
				InvokeOnMainThread (() => {
					_dialog.ShowProgressDialog (ApplicationStrings.Loading);
				});
				PopulateData ();
				InvokeOnMainThread (() => {
					_dialog.DismissProgressDialog ();
				});
			}
		}

		#endregion

		#region Properties

		private string _workingHours;

		public string WorkingHours {
			get {
				return _workingHours;
			}
			set {
				_workingHours = value;
				RaisePropertyChanged (() => WorkingHours);
			}
		}

		private string _phone;

		public string Phone {
			get {
				return _phone;
			}
			set {
				_phone = value;
				RaisePropertyChanged (() => Phone);
			}
		}

		private string _mail;

		public string Mail {
			get {
				return _mail;
			}
			set {
				_mail = value;
				RaisePropertyChanged (() => Mail);
			}
		}

		private string _facebook;

		public string Facebook {
			get {
				return _facebook;
			}
			set {
				_facebook = value;
				RaisePropertyChanged (() => Facebook);
			}
		}

		private string _webPage;

		public string WebPage {
			get {
				return _webPage;
			}
			set {
				_webPage = value;
				RaisePropertyChanged (() => WebPage);
			}
		}

		#endregion

		#region Methods

		public async void PopulateData ()
		{
			var data = await _contactService.GetContactInfo ();
			InvokeOnMainThread (() => _dialog.DismissProgressDialog ());
			if (data == null) {
				return;
			}
			if (!string.IsNullOrWhiteSpace (data.DisplayMessage)) {
				InvokeOnMainThread (() => _dialog.ShowToast (data.DisplayMessage));
			}
			if (data.Success) {
				InvokeOnMainThread (() => BindDataToUIFields (data.Result));
			}
		}

		private void BindDataToUIFields (ContactInfoModel model)
		{
			this.Facebook = model.Facebook;
			this.Mail = model.Email;
			this.Phone = model.PhoneNumber;
			this.WorkingHours = model.WorkHours;
			this.WebPage = model.WebPage;
		}

		#endregion
	}
}
