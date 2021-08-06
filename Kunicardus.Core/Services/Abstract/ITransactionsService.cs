using System;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.BusinessModels;
using System.Threading.Tasks;

namespace Kunicardus.Core
{
	public interface ITransactionsService
	{
		BaseActionResult<TransactionsModel> GetTransactions (string cardNumber);

		BaseActionResult<CheckTransactionModel> CheckLastTransaction (string unicardNumber, string merchantId, string amount, DateTime? transactionDate);

        Task<BaseActionResult<AccountStatementModel>> GetAccountStatement(int userId, int? rowCount, int? rowIndex);
	}
}

