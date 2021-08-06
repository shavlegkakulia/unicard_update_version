using System;

namespace Kuni.Core
{
	public enum DeliveryMethidIds:int
	{
		ServiceCenter = 1,
		PartnerOrganization = 2,
		Fetch = 3,
		OnlinePayment = 4,
		ShowRoom = 5

	}

	public enum ProductType:int
	{
		CTPark = 1,
		CTParkFee = 2,
		GiftCard = 3,
		Thing = 4,
//ნივთი :d
		OnlinePayment = 5,
		Service = 6,
		Ticket = 6,
		ComPayment = 8,
		Vaucher = 9

	}

}

