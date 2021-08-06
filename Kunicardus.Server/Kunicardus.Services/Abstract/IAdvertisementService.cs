using Kunicardus.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Services.Abstract
{
    public interface IAdvertisementService
    {
        GetBillboardsResponse GetBillboards(GetBillboardsRequest request);
        GetAdvertisementsResponse GetAdvertisements(GetAdvertisementsRequest requets);
        SaveAdvertisementPointsResponse SavePoints(SaveAdvertisementPointsRequest request);
    }
}
