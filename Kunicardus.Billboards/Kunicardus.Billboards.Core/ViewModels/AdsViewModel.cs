using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.DbModels;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.Enums;
using Kunicardus.Billboards.Core.Plugins;

namespace Kunicardus.Billboards.Core.ViewModels
{
	public class AdsViewModel
	{
		IBillboardsService _billboardsService;
		IAdsService _adsService;
        IConnectivityPlugin _connectivityPlugin;

		public string DisplayMessage { get; set; }

		public string ErrorText { get; set; }

		public List<BillboardHistory> Advertisments { get; set; }

		public List<AdsModel> LoadedAds { get; set; }

		public AdsViewModel (IBillboardsService billboardsService, IAdsService adsService, IConnectivityPlugin connectivityPlugin)
		{
            _connectivityPlugin = connectivityPlugin;
			_billboardsService = billboardsService;
			_adsService = adsService;
		}

		//public List<Advertisment> GetAdvertisments ()
		//{
			//var adIds = LoadedAds.Select(x => x.b);
			//var response = _adsService.GetAdvertisments ();
			//if (response != null) {
			//	DisplayMessage = response.DisplayMessage;
			//	ErrorText = response.ErrorMessage;
			//	if (response.Successful) {
			//		Advertisments = response.Result;
			//	}
			//}
			//return Advertisments;
		//}

		public bool GetLoadedAds ()
		{
			LoadedAds = _adsService.GetLoadedAdvertisments ();
			if (LoadedAds != null) {
				return true;
			}
			return false;
		}

		public BaseActionResult<AdvertismentStatus> SavePoints (int adId)
		{
			var status = _adsService.SavePoints (adId);
			if (status != null) {
				Advertisments.First (x => x.AdvertismentId == adId).Status = status.Result;
				return status;
			}
			return null;
		}

        public bool IsNetworkReachable
        {
            get
            {
                return _connectivityPlugin.IsNetworkReachable;
            }
        }
    }
}