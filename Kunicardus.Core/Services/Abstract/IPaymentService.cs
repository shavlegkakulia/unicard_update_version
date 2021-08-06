using System;
using Kunicardus.Core.Models.DB;
using System.Collections.Generic;
using Kunicardus.Core.Models;
using System.Threading.Tasks;
using Kunicardus.Core.Models.DataTransferObjects;
using Kunicardus.Core.UnicardApiProvider;

namespace Kunicardus.Core
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

