using System;
using System.Collections.Generic;
using Kunicardus.Core.Models.DataTransferObjects;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kunicardus.Core.ViewModels.iOSSpecific
{
	public class iTransactionVerificationViewModel : BaseViewModel
	{
		ITransactionsService _transactionService;

		public iTransactionVerificationViewModel (ITransactionsService transactionService)
		{
			_transactionService = transactionService;
		}

		TransferUserModel FBUser;

		public void Init (iTransactionVerificationViewModelParams param)
		{
			UnicardNumber = param.UnicardNumber;
			Merchants = JsonConvert.DeserializeObject<List<Merchant>> (param.Merchants);
			FBUser = JsonConvert.DeserializeObject<TransferUserModel> (param.FBUser);
		}

		private List<Merchant> _merchants = new List<Merchant> ();

		public List<Merchant> Merchants {
			get { return _merchants; }
			set {
				_merchants = value;
				RaisePropertyChanged (() => Merchants);
			}
		}

		private Merchant _selectedItem;

		public Merchant SelectedMerchant {
			get { return _selectedItem; }
			set {
				_selectedItem = value;
				RaisePropertyChanged (() => SelectedMerchant);
			}
		}

		private string _price;

		public string Price {	
			get { return _price; }
			set {
				_price = value;
				RaisePropertyChanged (() => Price);
			}
		}

		private DateTime? _date;

		public DateTime? Date {	
			get { return _date; }
			set {
				_date = value;
				RaisePropertyChanged (() => Date);
			}
		}

		private string _unicardNumber;

		public string UnicardNumber {
			get { return _unicardNumber; }
			set { _unicardNumber = value; }
		}

		private ICommand _continueCommand;

		public ICommand ContinueCommand {
			get {
				_continueCommand = _continueCommand ?? new MvxCommand (Continue);
				return _continueCommand;
			}
		}

		private bool _lastTransactionStatus;

		public bool LastTransactionStatus {
			get { return _lastTransactionStatus; }
			set{ _lastTransactionStatus = value; }
		}

		private void Continue ()
		{
			ShouldValidateModel = true;
			if (_selectedItem == null)
				InvokeOnMainThread (() => {
					_dialog.ShowToast ("აირჩიეთ ბოლო ტრანზაქციის ადგილი");
				});
			else if (string.IsNullOrWhiteSpace (_price))
				InvokeOnMainThread (() => {
					_dialog.ShowToast ("შეიყვანეთ თანხა");
				});
			else if (!_date.HasValue)
				InvokeOnMainThread (() => {
					_dialog.ShowToast ("აირჩიეთ ბოლო ტრანზაქციის დრო");
				});
			else {
				InvokeOnMainThread (() => {
					_dialog.ShowProgressDialog ("");
				});
				Task.Run (() => {
					var response = _transactionService.CheckLastTransaction (
						               _unicardNumber,
						               _selectedItem.MerchantId,	
						               _price.Replace (",", "."),
						               _date);
					_lastTransactionStatus = response.Success;
					InvokeOnMainThread (() => {
						_dialog.DismissProgressDialog ();
					});
					if (response.Success) {
						if (FBUser == null) {
							ShowViewModel<iRegistrationViewModel> (new {newCardRegistration = false, unicardNumber = _unicardNumber});
						} else {
							FBUser.CardNumber = _unicardNumber;
							FBUser.NewCardRegistration = false;
							ShowViewModel<iFacebookRegistrationViewModel> (FBUser);
						}
					}
					_uiThread.InvokeUIThread (() => {
						if (!string.IsNullOrEmpty (response.DisplayMessage)) {
							_dialog.ShowToast (response.DisplayMessage);
						}
					});
				});
			}
		}
	}

	public class iTransactionVerificationViewModelParams
	{
		public string UnicardNumber {
			get;
			set;
		}

		public string Merchants {
			get;
			set;
		}

		public string FBUser {
			get;
			set;
		}
	}
}

