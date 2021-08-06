using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;







using System.Threading.Tasks;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.Enums;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using Kunicardus.Billboards.Core.Models.DTOs.Response;

namespace Kunicardus.Billboards.Core.Services.Abstract
{
    public interface IAdsService
    {
        List<AdsModel> GetLoadedAdvertisments();

        List<AdsModel> GetNewAdIds(int[] loadedBillboardIds);

        List<BillboardHistoryModel> GetBillboardHistory();

        List<HistoryModel> GetSeenAdvertisments(string userId);

        BillboardsBaseResponse<BillboardHistory> GetAdvertisment(string userId, int billboardId, DateTime passDate, int historyId);

        BillboardsBaseResponse<List<BillboardHistory>> GetAdvertisments(List<int> billboardIds);

        AccumulatePointsResponse SavePoints(int billboardId, 
                                       int adId, 
                                       int historyId,
                                       string userId, 
                                       string passDate, 
                                       string viewDate, 
                                       Guid guid);
    }
}