using System;
using Kunicardus.Core.UnicardApiProvider;
using Kunicardus.Core.Helpers.AppSettings;
using System.Collections.Generic;
using Kunicardus.Core.Models;
using System.Threading.Tasks;
using Kunicardus.Core.Models.DB;
using Newtonsoft.Json;
using Kunicardus.Core.Models.DataTransferObjects;

namespace Kunicardus.Core
{
	public class PaymentService:IPaymentService
	{
		private IUnicardApiProvider _apiProvider;
		private IAppSettings _appSettings;

		public PaymentService (
			IUnicardApiProvider unicardApiProvider,
			IAppSettings appSettings)
		{
			_apiProvider = unicardApiProvider;
			_appSettings = appSettings;
		}

		#region IPaymentService implementation

		public async Task<BaseActionResult<List<DeliveryMethod>>> GetDeliveryMethods ()
		{
			BaseActionResult<List<DeliveryMethod>> result = new BaseActionResult<List<DeliveryMethod>> ();
			var url = string.Format ("{0}GetDeliveryMethods", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (new UnicardApiBaseRequest (), 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			
			var response = _apiProvider.Post<GetDeliveryMethodsResponse> (url, null, json).Result; 
			result.Result = response.Methods;

			return result;
		}

		public async Task<BaseActionResult<GetServiceCentersResponse>> GetServiceCenters (int productId, int methodId)
		{
			BaseActionResult<GetServiceCentersResponse> result = new BaseActionResult<GetServiceCentersResponse> ();
			var url = string.Format ("{0}GetServiceCenterList", _appSettings.UnicardServiceUrl);
			var requestModel = new GetServiceCentersRequest () {
				DeliveryMethodId = methodId,
				ProductId = productId
			}; 
			var json = JsonConvert.SerializeObject (requestModel, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			var response = _apiProvider.Post<GetServiceCentersResponse> (url, null, json).Result; 
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = response; 
				
			return result;
		}

		public async Task<BaseActionResult<GetOnlinePaymentInfoResponse>> GetOnlinePaymentInfo (int productId, string userID, string subscriberNumber)
		{
			BaseActionResult<GetOnlinePaymentInfoResponse> result = new BaseActionResult<GetOnlinePaymentInfoResponse> ();
			var url = string.Format ("{0}GetOnlinePaymentInfo", _appSettings.UnicardServiceUrl);
			var requestModel = new GetOnlinePaymentInfoRequest () {
				UserId = userID,
				ProductId = productId,
				SubscriberNumber = subscriberNumber
			}; 
			var json = JsonConvert.SerializeObject (requestModel, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			var response = _apiProvider.Post<GetOnlinePaymentInfoResponse> (url, null, json).Result; 
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = response; 

			return result;
		}

		public async Task<BaseActionResult<UnicardApiBaseResponse>> BuyProduct 
		(int productId, string userID, 
		 int typeId, int methodId, int discountId, string address,
		 decimal? amount, decimal? bonusAmount, string comment, string identifier,
		 string phone, string fullName, string personalId, 
		 int? serviceCenterId, Guid id,
		 DateTime tranDate, DateTime deliveryDate)
		{
			BaseActionResult<UnicardApiBaseResponse> result = new BaseActionResult<UnicardApiBaseResponse> ();
			var url = string.Format ("{0}BuyProduct", _appSettings.UnicardServiceUrl);

			var requestModel = new BuyProductRequest () {
				UserId = userID,
				ProductId = productId,
				ProductTypeID = typeId,
				DeliveryMethodID = methodId,
				DiscountId = discountId,
				BonusAmount = string.Format ("{0}", bonusAmount),
				GId = id,
				Quantity = 1,
				ServiceCenterID = serviceCenterId,
				RecipientFullName = fullName,
				RecipientPersonalN = personalId ?? "",
				Identifier = identifier ?? "",
				Phone = phone ?? "",
				Amount = string.Format ("{0}", amount),
				Address = address,
				Comment = comment,
				TranDate = string.Format ("{0}/{1}/{2}", tranDate.ToString ("dd"), tranDate.ToString ("MM"), tranDate.ToString ("yyyy")),
				DevileryDate = string.Format ("{0}/{1}/{2}", deliveryDate.ToString ("dd"), deliveryDate.ToString ("MM"), deliveryDate.ToString ("yyyy")),
			};
			var json = JsonConvert.SerializeObject (requestModel,
				           Formatting.None,
				           new JsonSerializerSettings {
					NullValueHandling = NullValueHandling.Ignore,
					Formatting = Formatting.None
				});
		
			var response = _apiProvider.Post<UnicardApiBaseResponse> (url, null, json).Result;
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
		
			return result;
		}

		#endregion
	}
}

