using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using System.Linq;
using System.Threading.Tasks;
using Kunicardus.Core.Models;
using System.Text.RegularExpressions;
using Kunicardus.Core.ViewModels;
using System.Runtime.InteropServices;

namespace Kunicardus.Core
{
	public class SettingsViewModel : BaseViewModel
	{
		#region Private Variables

		private IUserService _userService;
		private ILocalDbProvider _localDBProvider;
		//private SettingsInfo _userSettings;

		#endregion

		#region Properties

		private UserInfo _userInfo;

		public UserInfo UserInfo {
			get { return _userInfo; }
			set{ _userInfo = value; }
		}

		#endregion

		#region Constructor Implementation

		public SettingsViewModel (IUserService userService, ILocalDbProvider localDbProvider)
		{
			
			_userService = userService;
			_localDBProvider = localDbProvider;
			UserInfo = _localDBProvider.Get<UserInfo> ().FirstOrDefault ();
			if (UserInfo != null && UserInfo.UserId == null)
				UserInfo.UserId = "0";
			//_userSettings = _localDBProvider.Get<SettingsInfo> ().Where (x => x.UserId == Convert.ToInt32 (UserInfo.UserId)).FirstOrDefault ();
		}

		#endregion

		#region Commands

		private ICommand _openChangePasswordPageCommand;

		public ICommand OpenChangePasswordPageCommand {
			get { 
				_openChangePasswordPageCommand = _openChangePasswordPageCommand ?? new MvxCommand (() => {
					ShowViewModel<ChangePasswordViewModel> ();
				});
				return _openChangePasswordPageCommand;
			}
		}

		private ICommand _openRemovePinViewModel;

		public ICommand OpenRemovePinViewModelCommand {
			get { 
				_openRemovePinViewModel = _openRemovePinViewModel ?? new MvxCommand (() => {
					ShowViewModel<iOldPinViewModel> (new {headerTitle = ApplicationStrings.RemovePin,
						pageTitle = ApplicationStrings.RemovePinPageTitle});
				});
				return _openRemovePinViewModel;
			}
		}

		private ICommand _openChangePinViewModelCommand;

		public ICommand OpenChangePinViewModelCommand {
			get { 
				_openChangePinViewModelCommand = _openChangePinViewModelCommand ?? new MvxCommand (() => {
					ShowViewModel<iOldPinViewModel> (new {headerTitle = ApplicationStrings.ChangePin,
						pageTitle = ApplicationStrings.ChangePinPageTitle});
				});
				return _openChangePinViewModelCommand;
			}
		}

		private ICommand _openSetPinViewModellCommand;

		public ICommand OpenSetPinViewModellCommand {
			get { 
				_openSetPinViewModellCommand = _openSetPinViewModellCommand ?? new MvxCommand (() => {
					ShowViewModel<iNewPinViewModel> (new {headerTitle = ApplicationStrings.set_pin,
						pageTitle = ApplicationStrings.enter_new_pin});
				});
				return _openSetPinViewModellCommand;
			}
		}

		#endregion

		#region Methods

		public bool ShouldShowChangePassword ()
		{
			var user = _localDBProvider.Get<UserInfo> ().FirstOrDefault ();
			return (user != null && !user.IsFacebookUser);
		}

		private string Validation (string oldPassword, string newPassword, string confirmNewpassword)
		{
			string result = "";
			string passValidation = PasswordValidation (newPassword);
			if (!string.IsNullOrWhiteSpace (passValidation)) {
				result = passValidation;
			} else if (oldPassword == newPassword)
				result = "ახალი და ძველი პაროლი ერთმანეთს ემთხვევა";
			else if (newPassword != confirmNewpassword)
				result = "პაროლი და პაროლი განმეორებით არ ემთხვევა ერთმანეთს";
			return result;
		}

		private string PasswordValidation (string newPassword)
		{
			string errorText = "";
			if (!string.IsNullOrWhiteSpace (newPassword)) {
				Match ifNumber = Regex.Match (newPassword, @"\d+");
				Match ifCharacter = Regex.Match (newPassword, @"[a-zA-Z]");
				if (newPassword.Length < 8)
					errorText = "პაროლის სიგრძე უნდა აღემატებოდეს 8 სიმბოლოს";
				else if (ifNumber.Value == "")
					errorText = "პაროლში აუცილებელია 1 რიცხვი მაინც";
				else if (ifCharacter.Value == "")
					errorText = "პაროლში აუცილებელია 1 ლათინური ასო მაინც";
			} else
				errorText = "პაროლი ვერ იქნება ცარიელი";
			return errorText;
		}



		public void ChangePassword (string oldPassword, string newPassword, string confirmPassword)
		{
			var validationStatus = Validation (oldPassword, newPassword, confirmPassword);
			if (string.IsNullOrWhiteSpace (validationStatus)) {
				Task.Run (async() => {
					InvokeOnMainThread (() => {
						_dialog.ShowProgressDialog (ApplicationStrings.Loading);
					});
					var response = await _userService.ChangePassword (oldPassword, newPassword, UserInfo.UserId);
					InvokeOnMainThread (() => {
						_dialog.DismissProgressDialog ();
						_dialog.ShowToast (response.DisplayMessage);
					});
				});
			} else
				InvokeOnMainThread (() => {
					_dialog.ShowToast (validationStatus);
				});
		}

		#endregion
	}
}

