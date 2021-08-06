using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.BusinessModels
{
    public class AccountStatementModel
    {
        public int TotalCount { get; set; }
        public List<TransactionModel> Transactions { get; set; }
    }
}
