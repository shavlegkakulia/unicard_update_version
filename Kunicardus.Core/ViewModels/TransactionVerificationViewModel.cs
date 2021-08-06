using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Collections.Generic;
using Kunicardus.Core.Models.DataTransferObjects;
using Kunicardus.Core.Models;
using MvvmCross;
using System.Threading.Tasks;
using MvvmCross.Platform;

namespace Kunicardus.Core
{
	public class TransactionVerificationViewModel : BaseViewModel
	{
		ITransactionsService _transactionService;

		public TransactionVerificationViewModel (ITransactionsService transactionService)
		{
			_transactionService = transactionService;
			if (_merchants.Count > 0)
				_merchants.Clear ();
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

		private bool _dataPopulated;

		public bool DataPopulated {	
			get { return _dataPopulated; }
			set {
				_dataPopulated = value;
				RaisePropertyChanged (() => DataPopulated);
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
			_dialog.ShowProgressDialog (ApplicationStrings.Loading);
			Task.Run (() => {
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
					var response = _transactionService.CheckLastTransaction (
						               _unicardNumber,
						               _selectedItem.MerchantId,	
						               _price.ToString (),
						               _date);
					_lastTransactionStatus = response.Success;

					if (response.Success) {
						Mvx.IocConstruct<RegistrationViewModel> ();
					}
					InvokeOnMainThread (() => {
						if (!string.IsNullOrEmpty (response.DisplayMessage)) {
							_dialog.ShowToast (response.DisplayMessage);
						}
					});
				}
				InvokeOnMainThread (() => {
					_dialog.DismissProgressDialog ();	
				});
				DataPopulated = true;
			});
		}
	}
}

