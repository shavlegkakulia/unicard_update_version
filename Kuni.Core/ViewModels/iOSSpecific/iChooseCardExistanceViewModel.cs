using System;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Models.DataTransferObjects;

namespace Kuni.Core.ViewModels.iOSSpecific
{
	public class iChooseCardExistanceViewModel : BaseViewModel
	{
		private ICommand _unicardAvailableCommand;

		public ICommand UnicardAvailableCommand {
			get { 
				_unicardAvailableCommand = _unicardAvailableCommand ?? new MvvmCross.Commands.MvxCommand (ShowUnicardNumberInputViewModel);
				return _unicardAvailableCommand;
			}
		}

		private ICommand _unicartNotAvaliableCommand;

		public ICommand UnicartNotAvailableCommand {
			get { 
				_unicartNotAvaliableCommand = _unicartNotAvaliableCommand ?? new MvvmCross.Commands.MvxCommand (ShowEmailRegisterViewModel);
				return _unicartNotAvaliableCommand;
			}
		}

		private void ShowUnicardNumberInputViewModel ()
		{						
			NavigationCommand<iUnicardNumberInputViewModel> (FbUser);
		}

		private void ShowEmailRegisterViewModel ()
		{
			if (FbUser == null) {
				NavigationCommand<iRegistrationViewModel> (new {newCardRegistration = true});
			} else {
				FbUser.NewCardRegistration = true;
				NavigationCommand<iFacebookRegistrationViewModel> (FbUser);
			}
		}

		public TransferUserModel FbUser {
			get;
			set;
		}

		public void Init (string fbUserName, string fbSurname, string fbEmail, string fbId)
		{
			if (!string.IsNullOrWhiteSpace (fbUserName)
			    &&
			    !string.IsNullOrWhiteSpace (fbSurname)
			    &&
			    !string.IsNullOrWhiteSpace (fbEmail)
			    &&
			    !string.IsNullOrWhiteSpace (fbId)) {
				FbUser = new TransferUserModel ();
				FbUser.Email = fbEmail;
				FbUser.Name = fbUserName;
				FbUser.Surname = fbSurname;		
				FbUser.FBId = fbId;
			}
		}
	}
}

