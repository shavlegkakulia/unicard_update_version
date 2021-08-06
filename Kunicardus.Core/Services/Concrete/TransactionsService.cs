using System;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.DataTransferObjects;
using Kunicardus.Core.UnicardApiProvider;
using Kunicardus.Core.Helpers.AppSettings;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kunicardus.Core.Models.BusinessModels;
using Kunicardus.Core.Models.DataTransferObjects.Request;
using Kunicardus.Core.Models.DataTransferObjects.Response;

namespace Kunicardus.Core
{
	public class TransactionsService : ITransactionsService
	{
		private IUnicardApiProvider _apiProvider;
		private IAppSettings _appSettings;

		public TransactionsService (
			IUnicardApiProvider unicardApiProvider,
			IAppSettings appSettings
		)
		{
			_apiProvider = unicardApiProvider;
			_appSettings = appSettings;
		}

		public BaseActionResult<TransactionsModel> GetTransactions (string cardNumber)
		{
			BaseActionResult<TransactionsModel> result = new BaseActionResult<TransactionsModel> ();

			var request = new GetLastTransactionsRequest { 
				CardNumber = cardNumber
			};
			var url = string.Format ("{0}GetLastTransactions", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetLastTransactionsResponse> (url, null, json).Result; 
			result.Result = new TransactionsModel ();
			result.Success = response.Successful;
			result.Result.Merchants = response.Merchants;
			result.DisplayMessage = response.DisplayMessage;
			return result;
		}

		public BaseActionResult<CheckTransactionModel> CheckLastTransaction (string cardNumber, string merchantId, string amount, DateTime? transactionDate)
		{
			BaseActionResult<CheckTransactionModel> result = new BaseActionResult<CheckTransactionModel> ();

			var request = new CheckTransactionRequest { 
				UnicardNumber = cardNumber,
				MerchantId = merchantId,
				Amount = amount,
				TransactionDate = transactionDate
			};
			var url = string.Format ("{0}CheckLastTransaction", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings {
					NullValueHandling = NullValueHandling.Ignore,
					DateFormatString = Constants.ApiDateTimeFormat
				});
			var response = _apiProvider.Post<UnicardApiBaseResponse> (url, null, json).Result; 
			result.Result = new CheckTransactionModel ();
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;

			return result;
		}

		public async Task<BaseActionResult<AccountStatementModel>> GetAccountStatement (int userId, int? rowCount, int? rowIndex)
		{

			BaseActionResult<AccountStatementModel> result = new BaseActionResult<AccountStatementModel> ();

			var request = new GetAccountStatementRequest {
				UserId = userId,
				RowIndex = rowIndex,
				RowCount = rowCount
			};

			var url = string.Format ("{0}GetAccountStatement", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request,
				           Formatting.None,
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetAccountStatementResponse> (url, null, json).Result;
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new AccountStatementModel ();

			result.Result.Transactions = new List<TransactionModel> ();

			result.Result.TotalCount = response.TotalCount;
			if (response.Transactions != null && response.Transactions.Count > 0) {
				foreach (var item in response.Transactions) {
					var transactions = new TransactionModel {
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
					result.Result.Transactions.Add (transactions);
				}

			}

			return result;
		}
	}
}

