using System;
using Kunicardus.Core.Models.DataTransferObjects;

namespace Kunicardus.Core.Services.Abstract
{
	public interface IGetUnicardStatusService
	{
		CardStatusDTO GetUnicardStatus (string number);
	}
}

