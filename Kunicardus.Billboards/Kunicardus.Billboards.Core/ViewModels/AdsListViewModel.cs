using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.Plugins;
using Kunicardus.Billboards.Core.Models;

//using NetworkExtension;
using Kunicardus.Billboards.Core.DbModels;

namespace Kunicardus.Billboards.Core.ViewModels
{
    public class AdsListViewModel
    {
        IAdsService _adsService;
        IConnectivityPlugin _connectivityPlugin;
        BillboardsDb _db;

        public List<AdvertismentViewModel> Advertisments { get; set; }

        public List<BillboardHistoryModel> LoadedBillboards { get; set; }

        public AdsListViewModel(IAdsService adsService, IConnectivityPlugin connectivityPlugin)
        {
            _connectivityPlugin = connectivityPlugin;
            _adsService = adsService;
            Advertisments = new List<AdvertismentViewModel>();
            _db = new BillboardsDb(BillboardsDb.path);
        }

        public bool GetBillboards()
        {
            LoadedBillboards = _adsService.GetBillboardHistory();
            if (LoadedBillboards != null && LoadedBillboards.Count > 0)
            {
                Advertisments = LoadedBillboards.Select(x => new AdvertismentViewModel(_adsService, x)).ToList();
                return true;
            }
            return false;
        }
    }
}