using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kunicardus.Billboards.Core.Services.Abstract;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.DbModels;

using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.Enums;
using Kunicardus.Billboards.Core.Plugins;
using Kunicardus.Billboards.Core.Helpers;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Core.Models.DTOs.Response;
using Newtonsoft.Json;
using Kunicardus.Billboards.Core.Models.DTOs.Request;
using System.Collections.Specialized;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using System.Net;
using System.IO;


namespace Kunicardus.Billboards.Core.Services
{
    public class AdsService : IAdsService
    {
        BillboardsDb _db;
        IUnicardApiProvider _apiProvider;

        public AdsService(IConnectivityPlugin networkService, IUnicardApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
            _db = new BillboardsDb(BillboardsDb.path);
        }

        public List<AdsModel> GetLoadedAdvertisments()
        {
            string errorText = "";
            var query = @"select 
                                a.HistoryId,
                                b.AdvertismentId, 
                                b.MerchantLogo,
                                b.MerchantName,
                                b.Points,
								b.BillboardId,
                                a.PassDate
                          from BillboardHistory as a
                          inner join Billboard as b
                          on a.BillboardId = b.BillboardId
						  order by a.PassDate";

            var adsQuery = @"select HistoryId from BillboardHistory where Status=3";
            var adIds = _db.Query<BillboardHistory>(adsQuery);

            try
            {
                var loadedAds = _db.Query<AdsModel>(query);
                foreach (var item in adIds)
                {
                    loadedAds.RemoveAll(x => x.HistoryId == item.HistoryId);
                }
                return loadedAds;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return null;
            }
        }

        public List<AdsModel> GetNewAdIds(int[] loadedBillboardIds)
        {
            string errorText = "";
            string billboardIds = string.Join(",", loadedBillboardIds);

            var query = string.Format(@"select 
                                              b.AdvertismentId,
                                              b.BillboardId
                                        from Billboard as b
                                        where b.IsLoaded=1 and b.BillboardId not in ({0})", loadedBillboardIds);

            var adsQuery = @"select AdvertismentId from BillboardHistory where Status = 2";
            var adIds = _db.Query<BillboardHistory>(adsQuery);

            try
            {
                var loadedAds = _db.Query<AdsModel>(query);
                foreach (var item in adIds)
                {
                    loadedAds.RemoveAll(x => x.AdvertismentId == item.AdvertismentId);
                }

                return loadedAds;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                //Android.Util.Log.Debug("LoadedAds Select Message: ", ex.ToString());
                return null;
            }
        }

        public List<HistoryModel> GetSeenAdvertisments(string userId)
        {
            var req = new GetHistoryRequest()
            {
                UserId = userId
            };

            var json = JsonConvert.SerializeObject(req, 
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });

            var url = Path.Combine(Kunicardus.Billboards.Core.Helpers.Constants.NewUnicardServiceUrl, "GetUniBoardStatements");

            var response = _apiProvider.UnsecuredPost<GetHistoryResponse>(url, null, json);
            if (response != null && response.Successful)
            {
                return response.Transactions;
            }
            else
                return null;
        }

        public BillboardsBaseResponse<List<BillboardHistory>> GetAdvertisments(List<int> billboardIds)
        {
            string errorText = "";
            var advertisements = new List<BillboardHistory>();
            BillboardsBaseResponse<List<BillboardHistory>> response = new BillboardsBaseResponse<List<BillboardHistory>>();

            var url = string.Format("{0}advertisements", Kunicardus.Billboards.Core.Helpers.Constants.UnicardBillboardsApi);

            var builder = new UriBuilder(url);

            string qString = "?";
            foreach (var item in billboardIds)
            {
                qString += string.Format("BillboardIds={0}&", item);
            }

            string fullUrl = url + qString;

            var result = _apiProvider.GetFromApi<GetAdvertismentsResponse>(fullUrl, null);
            if (result != null && result.Successful)
            {
                GetAdvertismentsResponse list = result.Result;
                try
                {
                    _db.BeginTransaction();
                    foreach (var item in list.Advertisements)
                    {
                        var advertisment = new BillboardHistory
                        {
                            AdvertismentId = item.AdvertismentId,
                            ExternalLink = item.ExternalLink,
                            Image = item.Image[0],
                            Status = AdvertismentStatus.Loaded,
                            Points = item.Points,
                            TimeOut = item.TimeOut
                        };

                        advertisements.Add(advertisment);
                        _db.InsertOrReplace(advertisment);
                    }
                    _db.Commit();

                    response.DisplayMessage = result.DisplayMessage;
                    response.ResultCode = result.ResultCode;
                    response.ErrorMessage = result.ErrorMessage;
                    response.Result = advertisements;

                    return response;
                }
                catch (Exception ex)
                {
                    errorText = ex.ToString();
                    _db.Rollback();

                    response.DisplayMessage = result.DisplayMessage;
                    response.ResultCode = result.ResultCode;
                    response.ErrorMessage = !string.IsNullOrEmpty(result.ErrorMessage) ? result.ErrorMessage : ex.ToString();
                    response.Result = advertisements;

                    return response;
                }
            }
            return null;
        }

        public AccumulatePointsResponse SavePoints(int billboardId, 
                                                   int adId, 
                                                   int historyId,
                                                   string userId, 
                                                   string passDate, 
                                                   string viewDate, 
                                                   Guid guid)
        {
            string errorText = "";
            string query = "";
            var user = _db.Table<UserInfo>().FirstOrDefault();

            var req = new AccumulatePointsRequest()
            { 
                UserId = userId,
                BillBoardId = billboardId,
                AdvertisementId = adId,
                PassDate = passDate,
                ViewDate = viewDate,
                GuID = guid
            };

            var url = Path.Combine(Kunicardus.Billboards.Core.Helpers.Constants.NewUnicardServiceUrl, "UniBoardBonusAccumulation");

            var json = JsonConvert.SerializeObject(req, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var response = _apiProvider.UnsecuredPost<AccumulatePointsResponse>(url, null, json);
            if (response != null)
            {
                if (response.Successful)
                {
                    //Android.Util.Log.Debug("Saved Points", "Bla-Bla-Bla");
                    query = string.Format(@"update BillboardHistory set Status = 2 where AdvertismentId = {0} and HistoryId = {1}", adId, historyId);
                    response.Status = AdvertismentStatus.PointsAcumulated;
                }
                else
                {
                    //Android.Util.Log.Debug("Not Saved Points", "Bla-Bla-Bla");
                    query = string.Format(@"update BillboardHistory set Status = 1 where AdvertismentId = {0} and HistoryId = {1}", adId, historyId);
                    response.Status = AdvertismentStatus.Seen;
                }

                #region Exec. Query And Log
                try
                {
                    if (!string.IsNullOrEmpty(query))
                    {
                        _db.Execute(query);
                    }
                    //Android.Util.Log.Debug("Ads Update Message: ", "Advertisment Updated Successfully");
                }
                catch (Exception ex)
                {
                    errorText = ex.Message;
                    //Android.Util.Log.Debug("Billboards Select Message: ", ex.ToString());
                    response.Status = AdvertismentStatus.Seen;
                }
                return response;
                #endregion
            }

            return response;
        }

        public List<BillboardHistoryModel> GetBillboardHistory()
        {
            string errorText = "";
            var query = @"select 
                               bh.HistoryId as HistoryId,
                               bh.BillboardId as BillboardId,
                               bh.AdvertismentId as AdvertismentId,
                               b.MerchantLogo as OrganizationLogo,
                               b.MerchantName as OrganizationName,
                               bh.PassDate as PassDate,
                               bh.Image as Image,
                               bh.Points as Points,
                               bh.ExternalLink as ExternalLink,
                               bh.TimeOut as TimeOut,
                               bh.Status as Status,
                               bh.ViewDate as ViewDate,
                               bh.UserId as UserId 
                          from BillboardHistory as bh
                          inner join Billboard as b
                          on bh.BillboardId = b.BillboardId
                          where bh.Status!=3";

            //var adsQuery = @"select AdvertismentId from BillboardHistory where Status = 3";

            //var adIds = _db.Query<BillboardHistory> (adsQuery);

            try
            {
                var loadedAds = _db.Query<BillboardHistoryModel>(query);
                return loadedAds;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                //Android.Util.Log.Debug("LoadedAds Select Message: ", ex.ToString());
                return null;
            }
        }

        public BillboardsBaseResponse<BillboardHistory> GetAdvertisment(string userId, int billboardId, DateTime passDate, int historyId)
        {
            string errorText = "";
            BillboardHistory advertisement = new BillboardHistory();
            BillboardsBaseResponse<BillboardHistory> response = new BillboardsBaseResponse<BillboardHistory>();

            var dbQuery = string.Format(@"select 
                                                a.BillboardId,
                                          		a.AdvertismentId, 
                                          		a.HistoryId,
                                          		a.ExternalLink,
                                          		a.Image,
                                          		a.Status,
                                          		a.Points,
                                          		a.TimeOut,
												a.PassDate	
                                          from BillboardHistory as a 
                                          where a.HistoryId = {0}", historyId);

            advertisement = _db.Query<BillboardHistory>(dbQuery).FirstOrDefault();
            if (advertisement.Status == AdvertismentStatus.NotLoaded)
            {
                #region Download From Server

                var url = Path.Combine(Kunicardus.Billboards.Core.Helpers.Constants.NewUnicardServiceUrl, "GetAdvertisementDetails");
                GetAdvertisementRequest req = new GetAdvertisementRequest()
                {
                    BillboardId = billboardId,
                    UserId = userId,
                    PassDate = "/Date(1444248000000-0000)/"
                };

                var json = JsonConvert.SerializeObject(req, 
                               Formatting.None,
                               new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });

                var result = _apiProvider.UnsecuredPost<GetAdvertisementResponse>(url, null, json);
                if (result != null && result.Successful && result.Advertisement != null)
                {
                    try
                    {
                        advertisement.AdvertismentId = result.Advertisement.AdvertismentId;
                        advertisement.ExternalLink = result.Advertisement.ExternalLink;
                        if (result.Advertisement.Image != null && result.Advertisement.Image.Count > 0)
                            advertisement.Image = result.Advertisement.Image[0].ToBase64String();
                        advertisement.Status = AdvertismentStatus.Loaded;
                        advertisement.Points = result.Advertisement.Points;
                        advertisement.TimeOut = result.Advertisement.TimeOut;

                        _db.Update(advertisement);
                    }
                    catch (Exception ex)
                    {
                        errorText = ex.ToString();
                        _db.Rollback();
                    }
                }
                #endregion
                response.DisplayMessage = result.DisplayMessage;
                response.ResultCode = result.ResultCode;
            } 
            response.Result = advertisement;

            return response;
        }
    }
}