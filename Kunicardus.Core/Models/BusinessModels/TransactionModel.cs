using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunicardus.Core.Models.BusinessModels
{
    public class TransactionModel
    {
        public string Address { get; set; }
        public double PaymentAmount { get; set; }
        public string CardNumber { get; set; }
        public DateTime Date { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public double Score { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }
}
