using System;
using System.Collections.Generic;
using MvvmCross.ViewModels;
using Kuni.Core.Models.Helpers;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Models.DB;
using System.Threading.Tasks;
using Kuni.Core.Providers.LocalDBProvider;
using System.Linq;
using Kuni.Core.Models.BusinessModels;
using Kuni.Core.Models;
using Kuni.Core.Plugins.UIDialogPlugin;
using SQLitePCL;

namespace Kuni.Core.ViewModels.iOSSpecific
{
	public class iMyPageViewModel : BaseViewModel
	{
		#region Variables

		private ITransactionsService _transactionService;
		private ILocalDbProvider _dbProvider;
		private IUserService _userService;

		private bool _transactionsPopulated;
		private bool _balancePopulated;

		#endregion

		#region Constructor implementation

		public iMyPageViewModel (ITransactionsService transactionService, ILocalDbProvider dbProvider, IUserService userService)
		{
			_dbProvider = dbProvider;
			_transactionService = transactionService;
			_userService = userService;

			InvokeOnMainThread (() => {
				_dialog.ShowProgressDialog (ApplicationStrings.Loading);
			});
			GetData ();

		}

		#endregion

		#region Properties

		private bool _dataPopulated;

		public bool DataPopulated {
			get { return _dataPopulated; }
			set {
				_dataPopulated = value;
				RaisePropertyChanged (() => DataPopulated);
			}
		}

		private decimal _point = 0;

		public decimal Point {
			get { return _point; }
			set {
				_point = value;
				RaisePropertyChanged (() => Point);
				RaisePropertyChanged (() => PointsText);
			}
		}

		public string PointsText {
			get { 
				return Math.Round (_point, 1).ToString ().Replace (",", ".");
			}
		}

		private decimal _blocked = 0;

		public decimal Blocked {
			get { return _blocked; }
			set {
				_blocked = value;
				RaisePropertyChanged (() => Blocked);
				RaisePropertyChanged (() => BlockedText);
			}
		}

		public string BlockedText {
			get { 
				return Math.Round (_blocked, 1).ToString ().Replace (",", ".");
			}
		}

		private decimal _spent = 0;

		public decimal Spent {
			get { return _spent; }
			set {
				_spent = value;
				RaisePropertyChanged (() => Spent);
				RaisePropertyChanged (() => SpentText);
			}
		}

		public string SpentText {
			get { 
				return Math.Round (_spent, 1).ToString ().Replace (",", ".");
			}
		}

		private decimal _total = 0;

		public decimal Total {
			get { return _total; }
			set {
				_total = value;
				RaisePropertyChanged (() => Total);
				RaisePropertyChanged (() => TotalText);
			}
		}

		public string TotalText {
			get { 
				return Math.Round (_total, 1).ToString ().Replace (",", ".");
			}
		}

		private string _cardNumber = "";

		public string CardNumber {
			get { return _cardNumber; }
			set {
				_cardNumber = value;
				RaisePropertyChanged (() => CardNumber);
			}
		}

		private List<TransactionInfo> _transactions;

		public List<TransactionInfo> Transactions {
			get { return _transactions; }
			set {
				_transactions = value;
				RaisePropertyChanged (() => Transactions);
			}
		}

		#endregion

		public void GetData ()
		{
			_balancePopulated = false;
			_transactionsPopulated = false;
			Task.Run (() => {
				var user = _dbProvider.Get<UserInfo> ().FirstOrDefault ();
				Total = user.Balance_AccumulatedPoint;
				Point = user.Balance_AvailablePoints;
				Blocked = user.Balance_BlockedPoints;
				Spent = user.Balance_SpentPoints;

				var transactions = _dbProvider.Get<TransactionInfo> ();
				Transactions = transactions;

				var balance = _userService.GetUserBalance (user.UserId);				
				if (balance != null && balance.Success) {
					user.Balance_AccumulatedPoint = Total = balance.Result.AccumulatedPoint;
					user.Balance_AvailablePoints = Point = balance.Result.AvailablePoints;
					user.Balance_BlockedPoints = Blocked = balance.Result.BlockedPoints;
					user.Balance_SpentPoints = Spent = balance.Result.SpentPoints;
					_dbProvider.Update<UserInfo> (user);
				} 
				_balancePopulated = true;

				DataPopulated = true;
				BaseActionResult<AccountStatementModel> response = _transactionService.GetAccountStatement (Convert.ToInt32 (user.UserId), null, null).Result;
				if (response.Success && response.Result != null) {
					transactions.Clear ();
					foreach (var item in response.Result.Transactions) {
						var transaction = new TransactionInfo {
							OrganizationId = item.OrganizationId,
							Address = item.Address,
							CardNumber = item.CardNumber,
							Date = item.Date,
							OrganizationName = item.OrganizationName,
							PaymentAmount = item.PaymentAmount,
							Score = item.Score,
							Status = item.Status,
							Type = item.Type
						};
						transactions.Add (transaction);
					}
					if (_dbProvider.Get<TransactionInfo> ().Count == 0)
						_dbProvider.Insert<TransactionInfo> (transactions);
					else
						_dbProvider.Update<TransactionInfo> (transactions);
				} 

				if (transactions != null && transactions.Count > 0) {
					transactions.First ().First = true;
					transactions.Last ().Last = true;
					Transactions = transactions;
				}
				_transactionsPopulated = true;
				InvokeOnMainThread (() => {
					if (!string.IsNullOrEmpty (response.DisplayMessage)) {
						_dialog.ShowToast (response.DisplayMessage);
					}
					_dialog.DismissProgressDialog ();
				});
				PublishMessage ();
			});
		}

		private void PublishMessage ()
		{
			if (_balancePopulated && _transactionsPopulated) {
				DataPopulated = true;
			}
		}
	}
}

