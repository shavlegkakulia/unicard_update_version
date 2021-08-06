using System;
using Kuni.Core.Services.Abstract;
using System.Text.RegularExpressions;
using System.Windows.Input;
using MvvmCross.ViewModels;
using System.Threading.Tasks;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models.DB;
using System.Linq;
using MvvmCross;
//using MvvmCross;

namespace Kuni.Core.ViewModels
{
	public class ChangePasswordViewModel : BaseViewModel
	{
		#region Private Variables

		private IUserService _userService;
		private ILocalDbProvider _localDBProvider;

		#endregion

		#region Constructor Implementation

		public ChangePasswordViewModel (IUserService userService, ILocalDbProvider localDbProvider)
		{
			_userService = userService;
			_localDBProvider = localDbProvider;
		}

		#endregion

		#region Validation Methods

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
				if (newPassword.Length < 8 || ifNumber.Value == "" || ifCharacter.Value == "")
					errorText = "პაროლის სიგრძე უნდა აღემატებოდეს 8 სიმბოლოს \n" +
					"პაროლში აუცილებელია 1 რიცხვი მაინც \n" +
					"პაროლში აუცილებელია 1 ლათინური ასო მაინც\n";
			} else
				errorText = "გთხოვთ შეავსოთ ყველა ველი";
			return errorText;
		}

		#endregion

		#region Properties

		private bool _passwordChanged;

		public bool PasswordChanged {
			get{ return _passwordChanged; }
			set {
				_passwordChanged = value;
				RaisePropertyChanged (() => PasswordChanged);
			}
		}

		private string _oldPassword;

		public string OldPassword {
			get { return _oldPassword; }
			set {
				_oldPassword = value;
				RaisePropertyChanged (() => OldPassword);
			}
		}

		private string _newPassword;

		public string NewPassword {
			get { return _newPassword; }
			set {
				_newPassword = value;
				RaisePropertyChanged (() => NewPassword);
			}
		}

		private string _confirmNewPassword;

		public string ConfirmNewPassword {
			get { return _confirmNewPassword; }
			set {
				_confirmNewPassword = value;
				RaisePropertyChanged (() => ConfirmNewPassword);
			}
		}

		#endregion

		#region Commands

		private ICommand _changePasswordCommand;

		public ICommand ChangePasswordCommand {
			get { 
				_changePasswordCommand = _changePasswordCommand ?? new MvvmCross.Commands.MvxCommand (ChangePassword);
				return _changePasswordCommand;
			}
		}

		#endregion

		#region Methods

		public void ChangePassword ()
		{
			var validationStatus = Validation (_oldPassword, _newPassword, _confirmNewPassword);
			if (string.IsNullOrWhiteSpace (validationStatus)) {
				var userInfo = _localDBProvider.Get<UserInfo> ();
				var userId = userInfo.FirstOrDefault ().UserId;
				Task.Run (async() => {
					InvokeOnMainThread (() => {
						_dialog.ShowProgressDialog (ApplicationStrings.Loading);
					});
					var response = await _userService.ChangePassword (_oldPassword, _newPassword, userId);
					InvokeOnMainThread (() => {
						_dialog.DismissProgressDialog ();
						_dialog.ShowToast (response.DisplayMessage);
					});
				
					if (response.Success) {
						var securityProvider = Mvx.IoCProvider.Resolve<ICustomSecurityProvider> ();
						var info = securityProvider.GetCredentials ();
						securityProvider.SaveCredentials (info.UserId, info.UserName, NewPassword, info.SessionId, info.fbToken);
						this.OldPassword = "";
						this.NewPassword = "";
						this.ConfirmNewPassword = "";
						this.PasswordChanged = true;

					}

				});
			} else
				InvokeOnMainThread (() => {
					_dialog.ShowToast (validationStatus);
				});
			
		}

		#endregion
	}
}