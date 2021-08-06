using System;
using System.Threading.Tasks;
using Kuni.Core.Models;
using Kuni.Core.Models.BusinessModels;
using System.Collections.Generic;

namespace Kuni.Core.Services.Abstract
{
	public interface IOrganizationService
	{
		BaseActionResult<OrganizationsModel> GetOrganizations (int pageIndex, bool byScore, int rowCount, int? sectorID, int? subSectorId, bool latestOrg);

		BaseActionResult<OrganizationDetailsModel> GetOrganizationDetails (int organizationID);

		Task<BaseActionResult<List<MerchantModel>>> GetMerchants (int? cityID, int? districtID, int? organizationID);

	}
}

