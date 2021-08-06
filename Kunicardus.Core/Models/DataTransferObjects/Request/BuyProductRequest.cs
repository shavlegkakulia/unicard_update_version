using System;
using Newtonsoft.Json;
using Kunicardus.Core.UnicardApiProvider;

namespace Kunicardus.Core
{
	public class BuyProductRequest:UnicardApiBaseRequest
	{
		[JsonProperty ("user_id")]
		public string UserId {
			get;
			set;
		}

		[JsonProperty ("product_id")]
		public int ProductId {
			get;
			set;
		}

		[JsonProperty ("delivery_method_id")]
		public int DeliveryMethodID {
			get;
			set;
		}

		[JsonProperty ("discount_id")]
		public int DiscountId {
			get;
			set;
		}

		[JsonProperty ("bonus_amount")]
		public string BonusAmount {
			get;
			set;
		}

		[JsonProperty ("guid")]
		public Guid GId {
			get;
			set;
		}

		[JsonProperty ("quantity")]
		public int Quantity {
			get;
			set;
		}

		[JsonProperty ("service_center_id")]
		public int? ServiceCenterID {
			get;
			set;
		}

		[JsonProperty ("recipient_full_name")]
		public string RecipientFullName {
			get;
			set;
		}

		[JsonProperty ("recipient_personal_id")]
		public string RecipientPersonalN {
			get;
			set;
		}

		[JsonProperty ("online_payment_identifier")]
		public string Identifier {
			get;
			set;
		}

		[JsonProperty ("amount")]
		public string Amount {
			get;
			set;
		}

		[JsonProperty ("recipient_address")]
		public string Address {
			get;
			set;
		}

		[JsonProperty ("recipient_phone")]
		public string Phone {
			get;
			set;
		}

		[JsonProperty ("tran_date")]
		public string TranDate {
			get;
			set;
		}

		[JsonProperty ("delivery_date")]
		public string DevileryDate {
			get;
			set;
		}

		[JsonProperty ("comment")]
		public string Comment {
			get;
			set;
		}

		[JsonProperty ("product_type")]
		public int ProductTypeID {
			get;
			set;
		}
	}
}


