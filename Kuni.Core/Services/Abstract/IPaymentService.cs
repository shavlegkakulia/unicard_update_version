using System;
using Kuni.Core.Models.DB;
using System.Collections.Generic;
using Kuni.Core.Models;
using System.Threading.Tasks;
using Kuni.Core.Models.DataTransferObjects;
using Kuni.Core.UnicardApiProvider;

namespace Kuni.Core
{
	public interface IPaymentService
	{
		Task<BaseActionResult<List<DeliveryMethod>>> GetDeliveryMethods ();

		Task<BaseActionResult<GetServiceCentersResponse>> GetServiceCenters (int productId, int methodId);

		Task<BaseActionResult<GetOnlinePaymentInfoResponse>> GetOnlinePaymentInfo (int productId, string userID, string subscriberNumber);

		Task<BaseActionResult<UnicardApiBaseResponse>> BuyProduct 
		(int productId, string userID, int typeId, int methodId, int discountId, string address,
		 decimal? amount, decimal? bonusAmount, string comment, string identifier,
		 string phone, string fullName, string personalId, int? serviceCenterId, Guid id, DateTime tranDate, DateTime deliveryDate);
	}
}

