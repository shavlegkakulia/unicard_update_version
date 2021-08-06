using System;
using MvvmCross;
using Kuni.Core.Plugins.UIDialogPlugin;
using MvvmCross.ViewModels;
//using MvvmCross;

namespace Kuni.Core
{
	public class BaseRegisterViewModel : BaseViewModel
	{
		private IUIDialogPlugin _dialogPlugin;

		public BaseRegisterViewModel (IUIDialogPlugin dialogPlugin)
		{
			_dialogPlugin = dialogPlugin;
			if (ChooseCardExistanceViewModelProperty == null)
				ChooseCardExistanceViewModelProperty = Mvx.IoCConstruct<ChooseCardExistanceViewModel> ();
			if (RegistrationViewModelProperty == null)
				RegistrationViewModelProperty = Mvx.IoCConstruct<RegistrationViewModel> ();
			if (UnicardNumberInputViewModelProperty == null)
				UnicardNumberInputViewModelProperty = Mvx.IoCConstruct<UnicardNumberInputViewModel> ();
			if (CardVerificationViewModelProperty == null)
				CardVerificationViewModelProperty = Mvx.IoCConstruct<TransactionVerificationViewModel> ();
			if (FBRegisterViewModelProperty == null)
				FBRegisterViewModelProperty = Mvx.IoCConstruct<FBRegisterViewModel> ();
			if (SMSVerificationViewModelProperty == null)
				SMSVerificationViewModelProperty = Mvx.IoCConstruct<SMSVerificationViewModel> ();
			if (EmailRegistrationViewModelProperty == null)
				EmailRegistrationViewModelProperty = Mvx.IoCConstruct<EmailRegistrationViewModel> ();
		}

		public void Init (string fbUserName, string fbSurname, string fbEmail, string fbId)
		{
			_fbRegisterViewModel.FullName = fbUserName + " " + fbSurname;
			_fbRegisterViewModel.Name = fbUserName;
			_fbRegisterViewModel.SurName = fbSurname;
			_fbRegisterViewModel.Email = fbEmail;
			_fbRegisterViewModel.FBId = fbId;
		}

		private MvxViewModel _content;

		public MvxViewModel ContentView {
			get { return _content; }
			set {
				_content = value;
				RaisePropertyChanged (() => ContentView);
			}
		}

		private ChooseCardExistanceViewModel _chooseCardExistanceViewModel;

		public ChooseCardExistanceViewModel ChooseCardExistanceViewModelProperty {
			get { return _chooseCardExistanceViewModel; }
			set {
				_chooseCardExistanceViewModel = value;
				RaisePropertyChanged (() => ChooseCardExistanceViewModelProperty);
			}
		}

		private FBRegisterViewModel _fbRegisterViewModel;

		public FBRegisterViewModel FBRegisterViewModelProperty {
			get { return _fbRegisterViewModel; }
			set {
				_fbRegisterViewModel = value;
				RaisePropertyChanged (() => FBRegisterViewModelProperty);
			}
		}

		private EmailRegistrationViewModel _emailRegistrationViewModel;

		public EmailRegistrationViewModel EmailRegistrationViewModelProperty {
			get { return _emailRegistrationViewModel; }
			set {
				_emailRegistrationViewModel = value;
				RaisePropertyChanged (() => EmailRegistrationViewModelProperty);
			}
		}

		private RegistrationViewModel _registerViewModel;

		public RegistrationViewModel RegistrationViewModelProperty {
			get { return _registerViewModel; }
			set {
				_registerViewModel = value;
				RaisePropertyChanged (() => RegistrationViewModelProperty);
			}
		}

		private SMSVerificationViewModel _smsVerificationViewModel;

		public SMSVerificationViewModel SMSVerificationViewModelProperty {
			get { return _smsVerificationViewModel; }
			set {
				_smsVerificationViewModel = value;
				RaisePropertyChanged (() => SMSVerificationViewModelProperty);
			}
		}


		private UnicardNumberInputViewModel _unicardNumberInputView;

		public UnicardNumberInputViewModel UnicardNumberInputViewModelProperty {
			get { return _unicardNumberInputView; }
			set {
				_unicardNumberInputView = value;
				RaisePropertyChanged (() => UnicardNumberInputViewModelProperty);
			}
		}

		private TransactionVerificationViewModel _cardVeificationViewModel;

		public TransactionVerificationViewModel CardVerificationViewModelProperty {
			get { return _cardVeificationViewModel; }
			set {
				_cardVeificationViewModel = value;
				RaisePropertyChanged (() => CardVerificationViewModelProperty);
			}
		}

	}
}

