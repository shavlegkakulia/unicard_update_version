using System;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Models;
using System.Threading.Tasks;
using Kuni.Core.UnicardApiProvider;
using Kuni.Core.Helpers.AppSettings;
using Newtonsoft.Json;
using System.Collections.Generic;
using Kuni.Core.Models.DataTransferObjects.Request;
using Kuni.Core.Models.DataTransferObjects.Response;
using Kuni.Core.Models.BusinessModels;

namespace Kuni.Core
{
    public class OrganizationService : IOrganizationService
    {
        private IUnicardApiProvider _apiProvider;
        private IAppSettings _appSettings;

        public OrganizationService(IUnicardApiProvider apiProvider, IAppSettings appSettings)
        {
            _apiProvider = apiProvider;
            _appSettings = appSettings;
        }

        public BaseActionResult<OrganizationsModel> GetOrganizations(int pageIndex, bool byScore, int rowCount, int? sectorID, int? subSectorId, bool latestOrg)
        {
            BaseActionResult<OrganizationsModel> result = new BaseActionResult<OrganizationsModel>();

            var request = new GetOrganizationsRequest
            {
                PageIndex = pageIndex
                                                     ,
                ByScore = byScore
                                                     ,
                RowCount = rowCount
                                                     ,
                LatestOrg = latestOrg
                                                     ,
                SectorId = sectorID
                                                     ,
                SubSectorId = subSectorId
            };

            var url = string.Format("{0}GetOrganizations", _appSettings.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject(request,
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = _apiProvider.Post<GetOrganizationsResponse>(url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = new OrganizationsModel();

            result.Result.Organizations = new List<OrganizationModel>();

            result.Result.TotalCount = response.TotalCount;
            if (response.Organizations != null && response.Organizations.Count > 0)
            {
                foreach (var item in response.Organizations)
                {
                    var organization = new OrganizationModel
                    {
                        OrganizationId = item.OrganizationId,
                        Image = item.ImageUrl,
                        UnitScore = item.UnitScore,
                        Unit = item.Unit,
                        Name = item.Name,
                        SectorId = item.SectorId,
                        SubSectorId = item.SubSectorId,
                        UnitDescription = item.UnitDescription
                    };
                    result.Result.Organizations.Add(organization);
                }

            }

            return result;
        }

        public BaseActionResult<OrganizationDetailsModel> GetOrganizationDetails(int organizationID)
        {
            BaseActionResult<OrganizationDetailsModel> result = new BaseActionResult<OrganizationDetailsModel>();

            var request = new GetOrganizationDetailsRequest { OrganizationId = organizationID };

            var url = string.Format("{0}GetOrgDetails", _appSettings.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject(request,
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = _apiProvider.Post<GetOrganizationDetailsResponse>(url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = new OrganizationDetailsModel();
            if (response.Organisation != null)
            {
                result.Result.Description = response.Organisation.Description;
                result.Result.Email = response.Organisation.Email;
                result.Result.FbAddress = response.Organisation.FbAddress;
                result.Result.ImageUrl = response.Organisation.ImageUrl;
                result.Result.Name = response.Organisation.Name;
                result.Result.OrganizationId = response.Organisation.OrganizationId;
                result.Result.PhoneNumber = response.Organisation.PhoneNumber;
                result.Result.PhoneNumbers = response.Organisation.PhoneNumbers;
                result.Result.ServiceDescription = response.Organisation.ServiceDescription;
                result.Result.ShortDescription = response.Organisation.ShortDescription;
                result.Result.WorkingHours = response.Organisation.WorkingHours;
                result.Result.UnitScore = response.Organisation.UnitScore;
                result.Result.Unit = response.Organisation.Unit;
                result.Result.Website = response.Organisation.Website;
                result.Result.UnitDescription = response.Organisation.UnitDescription;

            }
            return result;
        }

        public async Task<BaseActionResult<List<MerchantModel>>> GetMerchants(int? cityID, int? districtID, int? organizationID)
        {
            BaseActionResult<List<MerchantModel>> result = new BaseActionResult<List<MerchantModel>>();

            var request = new GetMerchantsRequest
            {
                CityId = cityID
                                                  ,
                DistrictId = districtID
                                                  ,
                OrganizationId = organizationID
            };

            var url = string.Format("{0}GetMerchantList", _appSettings.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject(request,
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var response = _apiProvider.Post<GetMerchantsResponse>(url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = new List<MerchantModel>();

            if (response.Merchants != null && response.Merchants.Count > 0)
            {
                foreach (var item in response.Merchants)
                {
                    var merchant = new MerchantModel
                    {
                        OrganizationId = item.OrganizationId,
                        Image = item.Image,
                        MerchantName = item.MerchantName,
                        Address = item.Address,
                        CityId = item.CityId,
                        DistrictId = item.DistrictId,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        MerchantId = item.MerchantId,
                        OrgnizationPointsDesc = item.OrgnizationPointsDesc,
                        Unit = item.Unit,
                        UnitDescription = item.UnitDescription,
                        UnitScore = item.UnitScore
                    };
                    result.Result.Add(merchant);
                }
            }
            return result;
        }
    }
}

