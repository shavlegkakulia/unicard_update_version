using System;
using Kunicardus.Core.UnicardApiProvider;
using System.Collections.Generic;
using Kunicardus.Core.Models.DataTransferObjects;

namespace Kunicardus.Core.Models
{
	public class GetOrganizationSectorsResponse : UnicardApiBaseResponse
	{
		List<SectorDTO> Sectors;
	}

}

