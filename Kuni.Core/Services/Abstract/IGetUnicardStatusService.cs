using System;
using Kuni.Core.Models.DataTransferObjects;

namespace Kuni.Core.Services.Abstract
{
	public interface IGetUnicardStatusService
	{
		CardStatusDTO GetUnicardStatus (string number);
	}
}

