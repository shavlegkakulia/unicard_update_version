using System;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Models.DataTransferObjects;

namespace Kuni.Core.Services.Concrete
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

