using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.Enums;
using System.Linq;
using Kunicardus.Billboards.Core.Plugins;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using System;
using Kunicardus.Billboards.Core.Models.DTOs.Response;

namespace Kunicardus.Billboards.Core.ViewModels
{
    public class AdvertismentViewModel
    {
        private IAdsService _adsService;
        private BillboardsDb _db;
        private string _userId;

        public int BillboardId { get; set; }

        public BillboardHistoryModel Advertisment { get; set; }

        public bool Downloaded { get; set; }

        public AdvertismentViewModel(IAdsService adsService, BillboardHistoryModel advertisement)
        {
            _adsService = adsService;
            Advertisment = advertisement;
            _db = new BillboardsDb(BillboardsDb.path);
            _userId = _db.Query<UserInfo>(@"select b.UserId from UserInfo as b").Select(x => x.UserId).FirstOrDefault();
        }

        public BillboardsBaseResponse<BillboardHistory> Load()
        {

            var response = _adsService.GetAdvertisment(_userId, Advertisment.BillboardId, Advertisment.PassDate, Advertisment.HistoryId);
            if (response != null)
            if (response.Successful)
            {
                //Advertisment = response.Result;
                Advertisment.AdvertismentId = response.Result.AdvertismentId;
                Advertisment.ExternalLink = response.Result.ExternalLink;
                Advertisment.Image = response.Result.Image;
                Advertisment.Points = response.Result.Points;
                Advertisment.Status = response.Result.Status;
                Advertisment.TimeOut = response.Result.TimeOut;
                Advertisment.ViewDate = response.Result.ViewDate;
                Downloaded = true;
                return response;
            }
            Downloaded = false;
            return response;
        }

        public AccumulatePointsResponse SavePoints()
        {
            var	guid = Guid.NewGuid();
            var response = _adsService.SavePoints(Advertisment.BillboardId, 
                      Advertisment.AdvertismentId,
                      Advertisment.HistoryId,
                      _userId,
                      "/Date(1444248000000-0000)/",
                      "/Date(1444248000000-0000)/", 
                      guid);
            if (response != null)
            {
                Advertisment.Status = response.Status;
                return response;
            }
            return null;
        }

        public bool SkipTheAd()
        {
            string query = "";
            if (Advertisment != null)
            {
                query = string.Format(@"update BillboardHistory set Status = 3 where HistoryId = {0}", this.Advertisment.HistoryId);
            }
            try
            {
                _db.Execute(query);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}