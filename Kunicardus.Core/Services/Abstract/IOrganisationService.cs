using System;
using System.Threading.Tasks;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.BusinessModels;
using System.Collections.Generic;

namespace Kunicardus.Core.Services.Abstract
{
	public interface IOrganizationService
	{
		BaseActionResult<OrganizationsModel> GetOrganizations (int pageIndex, bool byScore, int rowCount, int? sectorID, int? subSectorId, bool latestOrg);

		BaseActionResult<OrganizationDetailsModel> GetOrganizationDetails (int organizationID);

		Task<BaseActionResult<List<MerchantModel>>> GetMerchants (int? cityID, int? districtID, int? organizationID);

	}
}

