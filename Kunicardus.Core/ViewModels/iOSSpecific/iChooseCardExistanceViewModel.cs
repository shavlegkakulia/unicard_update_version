using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using Kunicardus.Core.Models.DataTransferObjects;

namespace Kunicardus.Core.ViewModels.iOSSpecific
{
	public class iChooseCardExistanceViewModel : BaseViewModel
	{
		private ICommand _unicardAvailableCommand;

		public ICommand UnicardAvailableCommand {
			get { 
				_unicardAvailableCommand = _unicardAvailableCommand ?? new MvxCommand (ShowUnicardNumberInputViewModel);
				return _unicardAvailableCommand;
			}
		}

		private ICommand _unicartNotAvaliableCommand;

		public ICommand UnicartNotAvailableCommand {
			get { 
				_unicartNotAvaliableCommand = _unicartNotAvaliableCommand ?? new MvxCommand (ShowEmailRegisterViewModel);
				return _unicartNotAvaliableCommand;
			}
		}

		private void ShowUnicardNumberInputViewModel ()
		{						
			ShowViewModel<iUnicardNumberInputViewModel> (FbUser);
		}

		private void ShowEmailRegisterViewModel ()
		{
			if (FbUser == null) {
				ShowViewModel<iRegistrationViewModel> (new {newCardRegistration = true});
			} else {
				FbUser.NewCardRegistration = true;
				ShowViewModel<iFacebookRegistrationViewModel> (FbUser);
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

