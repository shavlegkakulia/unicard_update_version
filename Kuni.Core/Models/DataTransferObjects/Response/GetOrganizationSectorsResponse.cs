using System;
using Kuni.Core.UnicardApiProvider;
using System.Collections.Generic;
using Kuni.Core.Models.DataTransferObjects;

namespace Kuni.Core.Models
{
	public class GetOrganizationSectorsResponse : UnicardApiBaseResponse
	{
		List<SectorDTO> Sectors;
	}

}

