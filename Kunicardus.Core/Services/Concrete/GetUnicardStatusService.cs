using System;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.Models.DataTransferObjects;

namespace Kunicardus.Core.Services.Concrete
{
	public class GetUnicardStatusService : IGetUnicardStatusService
	{
		#region IGetUnicardStatusService implementation

		public CardStatusDTO GetUnicardStatus (string number)
		{
			CardStatusDTO result = new CardStatusDTO ();
			result.Code = 200;
			result.HasTransaction = true;
			result.IsRegistered = false;
			return result;
		}

		#endregion
	}
}

