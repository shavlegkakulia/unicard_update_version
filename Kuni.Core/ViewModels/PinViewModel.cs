using System;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models.DB;
using System.Linq;

namespace Kuni.Core
{
	public class PinViewModel : BaseViewModel
	{
		#region private Variables

		private ILocalDbProvider _dbProvider;
		private SettingsInfo _userSettings;

		#endregion

		#region Constructor Implementation

		public PinViewModel (ILocalDbProvider dbProvider)
		{
			_dbProvider = dbProvider;
			_userSettings = dbProvider.Get<SettingsInfo> ().FirstOrDefault ();
			UserId = dbProvider.Get<UserInfo> ().FirstOrDefault ().UserId;
		}

		#endregion

		#region Properties

		public string UserId { get; private set; }

		#endregion

		#region Methods

		public bool PinIsCorrect (string pin)
		{
			if (_userSettings.Pin != null && _userSettings.Pin.Equals (pin))
				return true;
			return false;
		}


		public void UpdatePin (string pin)
		{
			_dbProvider.Update<SettingsInfo> (new SettingsInfo () {
				UserId = Convert.ToInt32 (UserId),
				Pin = pin,
				PinIsSet = true
			});
		}

		public void ChangePin (string pin)
		{
			_dbProvider.Update<SettingsInfo> (new SettingsInfo () {
				UserId = Convert.ToInt32 (UserId),
				Pin = pin,
				PinIsSet = true
			});
			
		}

		public bool RemovePin (string pin)
		{	
			if (_userSettings.Pin != null && _userSettings.Pin == pin) {
				_dbProvider.Update<SettingsInfo> (new SettingsInfo () {
					UserId = Convert.ToInt32 (UserId),
					Pin = null,
					PinIsSet = false
				});
				return true;
			} else
				return false;
		}

		#endregion
	}
}

