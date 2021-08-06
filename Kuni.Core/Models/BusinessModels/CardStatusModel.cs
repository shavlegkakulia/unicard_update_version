using System;

namespace Kuni.Core.Models
{
	public class CardStatusModel
	{
		public bool CardIsValid {
			get;
			set;
		}

		public bool HasTransactions {
			get;
			set;
		}

		public bool ExistsUserData {
			get;
			set;
		}
	}
}

