using System;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class Merchant
	{
		public Merchant (string merchantName, string merchandId)
		{
			MerchantName = merchantName;
			MerchantId = merchandId;
		}

		public string MerchantName { get; set; }

		public string MerchantId { get; set; }

		public override string ToString ()
		{
			return MerchantName;
		}

		public override bool Equals (object obj)
		{
			var rhs = obj as Merchant;
			if (rhs == null)
				return false;
			return rhs.MerchantName == MerchantName;
		}

		public override int GetHashCode ()
		{
			if (MerchantName == null)
				return 0;
			return MerchantName.GetHashCode ();
		}

	}
}

