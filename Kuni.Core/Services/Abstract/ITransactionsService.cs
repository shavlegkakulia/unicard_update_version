using System;
using Kuni.Core.Models;
using Kuni.Core.Models.BusinessModels;
using System.Threading.Tasks;

namespace Kuni.Core
{
	public interface ITransactionsService
	{
		BaseActionResult<TransactionsModel> GetTransactions (string cardNumber);

		BaseActionResult<CheckTransactionModel> CheckLastTransaction (string unicardNumber, string merchantId, string amount, DateTime? transactionDate);

        Task<BaseActionResult<AccountStatementModel>> GetAccountStatement(int userId, int? rowCount, int? rowIndex);
	}
}

