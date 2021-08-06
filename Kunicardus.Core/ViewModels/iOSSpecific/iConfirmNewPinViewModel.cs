using System;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.Services.Concrete;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kunicardus.Core
{
	public class iConfirmNewPinViewModel : BaseViewModel
	{
		#region Private Variables

		private ILocalDbProvider _dbProvider;
		private SettingsInfo _userSettings;
		private IGoogleAnalyticsService _gaService;

		#endregion

		#region Initilise

		public void Init (bool fromSetPin, string newPin, string headerTitle, string pageTitle)
		{
			FromSetPin = fromSetPin;
			_newPin = newPin;
			_headerTitle = headerTitle;
			_pageTitle = pageTitle;
		}

		#endregion

		#region Constructor Implementation

		public iConfirmNewPinViewModel (ILocalDbProvider dbProvider, IGoogleAnalyticsService gaService)
		{
			_dbProvider = dbProvider;
			_gaService = gaService;
			_userSettings = _dbProvider.Get<SettingsInfo> ().FirstOrDefault ();
			Task.Run (() => {
				UserId = _dbProvider.Get<UserInfo> ().FirstOrDefault ().UserId;
				_userSettings = _dbProvider.Get<SettingsInfo> ().FirstOrDefault ();
			});
		}

		#endregion

		#region Properties

		private bool _fromSetPin;

		public bool FromSetPin {
			get{ return _fromSetPin; }
			set { _fromSetPin = value; }
		}

		private string _newPin;

		public string NewPin {
			get{ return _newPin; }
			set {
				_newPin = value;
				RaisePropertyChanged (() => NewPin);
			}
		}

		private string _userId;

		public string UserId {
			get{ return _userId; }
			set {
				_userId = value;
			}
		}

		private string _confirmNewPin;

		public string ConfirmNewPin {
			get{ return _confirmNewPin; }
			set {
				_confirmNewPin = value;
				RaisePropertyChanged (() => ConfirmNewPin);
			}
		}

		private string _headerTitle;

		public string HeaderTitle {
			get{ return _headerTitle; }
			set {
				_headerTitle = value;
			}
		}

		private bool _pinChaged;

		public bool PinChanged {
			get{ return _pinChaged; }
			set { 
				_pinChaged = value;
				RaisePropertyChanged (() => PinChanged);
			}
		}

		private string _pageTitle;

		public string PageTitle {
			get{ return _pageTitle; }
			set {
				_pageTitle = value;
			}
		}

		private bool _pinInputFinished;

		public bool PinInputFinished {
			get{ return _pinInputFinished; }
			set {
				if (value) {
					if (PinsAreCorrect ()) {
						if (!FromSetPin) {
							ChangePin ();
							_dialog.ShowToast (ApplicationStrings.pin_changed_successfully);
							PinChanged = true;
							_gaService.TrackEvent (GAServiceHelper.From.Settings, GAServiceHelper.Events.ChangePin);
						} else {
							SetPin ();
							_dialog.ShowToast (ApplicationStrings.pin_set_successfully);
							PinChanged = true;
							_gaService.TrackEvent (GAServiceHelper.From.Settings, GAServiceHelper.Events.SetPin);
						}
					} else {
						_dialog.ShowToast (ApplicationStrings.repeated_pin_incorrect);
					}
				}
				_pinInputFinished = value;
			}
		}

		#endregion

		#region Methods

		public bool PinsAreCorrect ()
		{
			return _newPin == _confirmNewPin;
		}

		private void ChangePin ()
		{
			_dbProvider.Update<SettingsInfo> (new SettingsInfo () {
				UserId = Convert.ToInt32 (UserId),
				Pin = _newPin,
				PinIsSet = true
			});
		}

		private void SetPin ()
		{
			_dbProvider.Update<SettingsInfo> (new SettingsInfo () {
				UserId = Convert.ToInt32 (UserId),
				Pin = _newPin,
				PinIsSet = true
			});
		}

		#endregion
	}
}

