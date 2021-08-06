using System;
using System.Collections.Generic;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Models.DB;
using System.Threading.Tasks;
using Kuni.Core.Providers.LocalDBProvider;
using System.Linq;
using Kuni.Core.Models.BusinessModels;
using Kuni.Core.Models;
using MvvmCross;
//using MvvmCross;

namespace Kuni.Core
{
	public class MyPageViewModel : BaseViewModel
	{
		#region Variables

		private ITransactionsService _transactionService;
		private IUserService _userService;

		#endregion

		#region Constructor implementation

		public MyPageViewModel (ITransactionsService transactionService, IUserService userService)
		{
			_transactionService = transactionService;
			_userService = userService;
			FillTransactionFromDB ();
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

		private bool _transactionsUpdated;

		public bool TransactionsUpdated {
			get { return _transactionsUpdated; }
			set {
				_transactionsUpdated = value;
				RaisePropertyChanged (() => TransactionsUpdated);
			}
		}

		private decimal _point = 0;

		public decimal Point {
			get { return _point; }
			set {
				_point = value;
				RaisePropertyChanged (() => Point);
			}
		}

		private decimal _blocked = 0;

		public decimal Blocked {
			get { return _blocked; }
			set {
				_blocked = value;
				RaisePropertyChanged (() => Blocked);
			}
		}

		private decimal _spent = 0;

		public decimal Spent {
			get { return _spent; }
			set {
				_spent = value;
				RaisePropertyChanged (() => Spent);
			}
		}

		private decimal _total = 0;

		public decimal Total {
			get { return _total; }
			set {
				_total = value;
				RaisePropertyChanged (() => Total);
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

		public void FillTransactionFromDB ()
		{
			Task.Run (() => {
				var transactions = new List<TransactionInfo> ();
				using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider> ()) {
					transactions = dbProvider.Get<TransactionInfo> ();
				}
				Transactions = transactions;
				TransactionsUpdated = true;
			});
		
		}

		public void GetData ()
		{
			Task.Run (() => {
				GetTransactions ();
				GetUserBalance ();
			});
		}

		public void GetTransactions ()
		{
			_transactionsUpdated = false;
			var transactions = new List<TransactionInfo> ();
			using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider> ()) {
				var user = dbProvider.Get<UserInfo> ().FirstOrDefault ();
				BaseActionResult<AccountStatementModel> response = _transactionService.GetAccountStatement (Convert.ToInt32 (user.UserId), null, null).Result;

				if (response.Success && response.Result != null) {
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
					dbProvider.Execute ("Delete from TransactionInfo");
					dbProvider.Insert<TransactionInfo> (transactions);
					TransactionsUpdated = true;
					Transactions = transactions;

				} else if (Transactions.Count == 0) {
					Transactions = dbProvider.Get<TransactionInfo> ();
					TransactionsUpdated = false;
				}
				InvokeOnMainThread (() => {
					DataPopulated = true;

					if (!string.IsNullOrEmpty (response.DisplayMessage)) {
						_dialog.ShowToast (response.DisplayMessage);
					}
				});
			}
		}

		public void GetUserBalance ()
		{
			using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider> ()) {
				var user = dbProvider.Get<UserInfo> ().FirstOrDefault ();
				var balance = _userService.GetUserBalance (user.UserId);
				InvokeOnMainThread (() => {
					if (balance != null && balance.Success) {
						user.Balance_AccumulatedPoint = balance.Result.AccumulatedPoint;
						Total = Math.Round (balance.Result.AccumulatedPoint, 1);
						user.Balance_AvailablePoints = balance.Result.AvailablePoints;
						Point = Math.Round (balance.Result.AvailablePoints, 1);
						user.Balance_BlockedPoints = balance.Result.BlockedPoints;
						Blocked = Math.Round (balance.Result.BlockedPoints, 1);
						user.Balance_SpentPoints = balance.Result.SpentPoints;
						Spent = Math.Round (balance.Result.SpentPoints, 1);
						dbProvider.Update<UserInfo> (user);
					} else {
						Total = user.Balance_AccumulatedPoint;
						Point = user.Balance_AvailablePoints;
						Blocked = user.Balance_BlockedPoints;
						Spent = user.Balance_SpentPoints;
					}
					DataPopulated = true;
				});
			}
		}
	}
}