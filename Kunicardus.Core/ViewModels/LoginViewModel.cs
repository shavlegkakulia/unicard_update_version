using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core.Helpers.AppSettings;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.Models;
using System.Threading.Tasks;
using Kunicardus.Core.ViewModels.iOSSpecific;
using Kunicardus.Core.Helpers.Device;
using Kunicardus.Core.Plugins.Connectivity;
using MvvmCross.Binding;
using MvvmCross.Core.ViewModels;

namespace Kunicardus.Core
{
	public class LoginViewModel : BaseViewModel
	{
		#region Variables

		private IAuthService _authService;
		private ILocalDbProvider _dbProvider;
		private IPaymentService _paymentService;
		private IUserService _userService;
		private ICustomSecurityProvider _securityProvider;
		private IConnectivityPlugin _connectivity;
		private IGoogleAnalyticsService _iGoogleAnalyticsService;

		#endregion

		#region Constructor implementation

		public LoginViewModel (IAuthService authService, 
		                       IConnectivityPlugin connectivity,
		                       IPaymentService paymentService, 
		                       IUserService userService, ILocalDbProvider dbProvider, 
		                       ICustomSecurityProvider securityProvider,
		                       IGoogleAnalyticsService iGoogleAnalyticsService)
		{
			_connectivity = connectivity;
			_userService = userService;
			_authService = authService;
			_dbProvider = dbProvider;
			_paymentService = paymentService;
			_securityProvider = securityProvider;					
			_iGoogleAnalyticsService = iGoogleAnalyticsService;
        }

		#endregion

		#region Properties

		private string _userId;
		private string _userName;

		public string UserName {
			get	{ return _userName; }
			set {
				_userName = value;
				RaisePropertyChanged (() => UserName);
			}
		}

		private bool _completed;

		public bool Completed {
			get	{ return _completed; }
			set {
				_completed = value;
			}
		}

		private string _password;

		public string Password {
			get{ return _password; }
			set {
				_password = value;
				RaisePropertyChanged (() => Password);
			}
		}

		#endregion

		#region Commands

		private ICommand _testCommand;

		public ICommand TestCommand {
			get {
				_testCommand = _testCommand ?? new MvxCommand (() => NavigationCommand<RootViewModel> (null, true));
				return _testCommand;
			}
		}

		private ICommand _authCommand;

		public ICommand AuthCommand {
			get {
				_authCommand = _authCommand ?? new MvxCommand (() => ShowViewModel<LoginAuthViewModel> ());
				return _authCommand;
			}
		}

		private ICommand _register;

		public ICommand RegisterCommand {
			get {
				_register = _register ?? new MvxCommand (RegisterUser);
				return _register;
			}
		}

		#endregion

		#region Methods

		public void DismisDialog ()
		{
			_dialog.DismissProgressDialog ();
		}

		public bool IfNewtork ()
		{
			if (_connectivity.IsNetworkReachable)
				return true;
			else if (_connectivity.IsWifiReachable)
				return true;
			return false;
		}

		private void SaveLoggedInUserInfoForFB ()
		{
			var users = _dbProvider.Get<UserInfo> ();
			foreach (var item in users) {
				_dbProvider.Delete<UserInfo> (item);
			}

			UserInfo newUser = new UserInfo () { UserId = _userId, Username = _userName };

			var userFromUnicard = _userService.GetUserInfoByUserId (_userId);
			if (userFromUnicard != null && userFromUnicard.Success) {
				newUser.FirstName = userFromUnicard.Result.FirstName;
				newUser.LastName = userFromUnicard.Result.LastName;
				newUser.Address = userFromUnicard.Result.Address;
				newUser.Username = UserName;
				newUser.FullAddress = userFromUnicard.Result.FullAddress;
				string phone = userFromUnicard.Result.Phone;
				if (phone.Length >= 12) {
					phone = phone.Substring (3, phone.Length - 3);
				}
				newUser.Phone = phone;
				newUser.PersonalId = userFromUnicard.Result.PersonalNumber;
			}

			var balance = _userService.GetUserBalance (_userId);
			if (balance != null && balance.Success) {
				newUser.Balance_AccumulatedPoint = balance.Result.AccumulatedPoint;
				newUser.Balance_AvailablePoints = balance.Result.AvailablePoints;
				newUser.Balance_BlockedPoints = balance.Result.BlockedPoints;
				newUser.Balance_SpentPoints = balance.Result.SpentPoints;
			}

			var virtualCardNumber = _userService.GetVirtualCard (_userId);
			if (virtualCardNumber != null && virtualCardNumber.Success) {
				newUser.VirtualCardNumber = virtualCardNumber.Result.CardNumber;
			}

			newUser.IsFacebookUser = true;
			_dbProvider.Insert<UserInfo> (newUser);
		}

		public void FacebookConnect (string name, string surname, string email, string fbId)
		{		
			UserName = email;
			InvokeOnMainThread (() => _dialog.ShowProgressDialog (ApplicationStrings.Loading));	
			Task.Run (async () => {	
				var response = _authService.Auth (email, null, fbId);
				if (response != null) {					
					if (response.Success) {
						_userId = response.Result.UserId;
						_securityProvider.SaveCredentials (_userId, email, null, response.Result.SessionId, fbId);

						SaveLoggedInUserInfoForFB ();
						FillDeliveryTypes ();
						if (_device.Platform == "ios") {
							InvokeOnMainThread (() => {
								_dialog.DismissProgressDialog ();
							});
							NavigationCommand<RootViewModel> (null, true);
						} else {
							ShowViewModel<MainViewModel> ();
						}

						if (_iGoogleAnalyticsService != null) {
							_iGoogleAnalyticsService.TrackEvent (GAServiceHelper.Events.FBAuthorization, GAServiceHelper.Events.Authorization);
						}
					} else {
						if (_device.Platform == "ios") {
							InvokeOnMainThread (() => {
								_dialog.DismissProgressDialog ();
							});
							ShowViewModel<iChooseCardExistanceViewModel> (new
								{	
									fbUserName = name,
									fbSurname = surname,
									fbEmail = email,
									fbId = fbId
								});							
						} else {
							// ANDROID
							ShowViewModel<BaseRegisterViewModel> (new
								{	
									fbUserName = name,
									fbSurname = surname,
									fbEmail = email,
									fbId = fbId
								});
							InvokeOnMainThread (() => {
								_dialog.DismissProgressDialog ();
							});
						}
					}
				} 
			});	
		}

		public void DismissDialog ()
		{
			if (_dialog != null) {
				_dialog.DismissProgressDialog ();
			}

		}

		//		private void SaveLoggedInUserInfo ()
		//		{
		//			var users = _dbProvider.Get<UserInfo> ();
		//			foreach (var item in users) {
		//				_dbProvider.Delete<UserInfo> (item);
		//			}
		//			_dbProvider.Insert<UserInfo> (new UserInfo (){ UserId = _userId, Username = _userName });
		//		}


		private void RegisterUser ()
		{
			if (_device.Platform == "ios") {
				ShowViewModel<iChooseCardExistanceViewModel> ();
			} else {
				ShowViewModel<BaseRegisterViewModel> (new {fbId = ""});
			}
		}

		private void FillDeliveryTypes ()
		{
			var data = _dbProvider.Get<DeliveryMethod> ();
			if (data.Count == 0) {
				var methods = _paymentService.GetDeliveryMethods ();
				if (methods != null && methods.Result != null) {
					_dbProvider.Insert<DeliveryMethod> (methods.Result.Result);
				}
			}

		}

		#endregion
	}
}
