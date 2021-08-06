using System;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models.DB;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Kuni.Core
{
	public class iOldPinViewModel : BaseViewModel
	{
		#region Private Variables

		private SettingsInfo _userSettings;
		private ILocalDbProvider _dbProvider;
		private IGoogleAnalyticsService _gaService;

		#endregion

		#region Constructor Implementation


		public iOldPinViewModel (ILocalDbProvider dbProvider, IGoogleAnalyticsService gaService)
		{
			_dbProvider = dbProvider;
			_gaService = gaService;	
			Task.Run (() => {
				UserId = _dbProvider.Get<UserInfo> ().FirstOrDefault ().UserId;
				_userSettings = _dbProvider.Get<SettingsInfo> ().FirstOrDefault ();
			});
		}

		#endregion

		#region Init

		public void Init (string headerTitle, string pageTitle)
		{
			this.HeaderTitle = headerTitle;	
			this.PageTitle = pageTitle;
		}

		#endregion

		#region Properties

		public string UserId {
			get;
			set;
		}

		private bool _pinInputFinished;

		public bool PinInputFinished {
			get{ return _pinInputFinished; }
			set {
				if (value) {
					if (PinIsCorrect ()) {
						NavigationCommand<iNewPinViewModel> (new {headerTitle = ApplicationStrings.ChangePin,
							pageTitle = ApplicationStrings.enter_new_pin});
					} else
						_dialog.ShowToast (ApplicationStrings.incorrect_pin);
				}
				_pinInputFinished = value;
			}
		}

		private bool _pinRemoved;

		public bool PinRemoved {
			get{ return _pinRemoved; }
			set {
				_pinRemoved = value;
				RaisePropertyChanged (() => PinRemoved);
			}
		}

		private string _headerTitle;

		public string HeaderTitle {
			get{ return _headerTitle; }
			set {
				_headerTitle = value;
			}
		}

		private string _pageTitle;

		public string PageTitle {
			get{ return _pageTitle; }
			set {
				_pageTitle = value;
			}
		}

		private string _oldPin;

		public string OldPin {
			get { return _oldPin; }
			set { 
				_oldPin = value;
				RaisePropertyChanged (() => OldPin);
			}
		}

		#endregion

		#region Methods

		public bool PinIsCorrect ()
		{
			_userSettings = _dbProvider.Get<SettingsInfo> ().Where (x => x.UserId == Convert.ToInt32 (UserId)).FirstOrDefault ();
			var a = _dbProvider.Get<SettingsInfo> ().ToList ();
			if (_userSettings.Pin != null && _userSettings.Pin.Equals (_oldPin))
				return true;
			else
				return false;
		}

		public void RemovePin ()
		{	
			if (PinIsCorrect ()) {
				_dbProvider.Update<SettingsInfo> (new SettingsInfo () {
					UserId = Convert.ToInt32 (UserId),
					Pin = null,
					PinIsSet = false
				});
				this.PinRemoved = true;
				_gaService.TrackEvent (GAServiceHelper.From.Settings, GAServiceHelper.Events.RemovePin);
				_dialog.ShowToast (ApplicationStrings.removed_pin_successfully);
			} else {
				this.PinRemoved = false;
				_dialog.ShowToast (ApplicationStrings.incorrect_pin);
			}
		}

		#endregion
	}
}

