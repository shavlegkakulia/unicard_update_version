using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Kunicardus.Billboards.Core.Services.Abstract;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;

namespace Kunicardus.Billboards.Core.ViewModels
{
	public class BillboardsViewModel
	{
		IBillboardsService _billboardsService;
		IAdsService _adsService;

		public int AdvertisementsCount{ get; set; }

		public List<Billboard> Billboards { get; set; }

		public BillboardsViewModel (IBillboardsService billboardsService, IAdsService adsService)
		{
			_billboardsService = billboardsService;
			_adsService = adsService;
		}

		public bool GetBillboards ()
		{
			Billboards = _billboardsService.GetBillboards ();
			if (Billboards != null) {
				return true;
			}
			return false;
		}

		public bool GetLoadedAdsCount ()
		{
			AdvertisementsCount = _adsService.GetLoadedAdvertisments ().Count;
			if (AdvertisementsCount != null) {
				return true;
			}
			return false;
		}

		public bool MarkBillboardAsSeen (int billboardId, int advertisementId)
		{
			int prev = _billboardsService.GetLastPassedBillboardId ();
			if (billboardId != prev) {
				return _billboardsService.MarkBillboardAsSeen (billboardId, advertisementId);
			}
			return false;
		}

		public void InsertDummyData ()
		{
			_billboardsService.InsertDummyDataForIOS ();
		}
	}
}