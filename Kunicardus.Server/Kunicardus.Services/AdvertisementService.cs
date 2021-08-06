using Kunicardus.Domain;
using Kunicardus.Domain.Db;
using Kunicardus.Services.Abstract;
using Kunicardus.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Configuration;

namespace Kunicardus.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        KunicardusDbContext _db;
        private readonly string _privateKey;
        private readonly string _publicKey;

        public AdvertisementService(KunicardusDbContext db)
        {

            _privateKey = ConfigurationManager.AppSettings["PrivateKey"]; //"UnicardPublicAPIPrivateKey";
            _publicKey = ConfigurationManager.AppSettings["PublicKey"];  //"UnicardKey";
            _db = db;
        }

        public GetBillboardsResponse GetBillboards(GetBillboardsRequest request)
        {
            var resultBillboardsQuery =
                _db.Set<Billboard>()
                .Select(billboard => new
                {
                    BillboardId = billboard.BillboardId,
                    AlertDistance = billboard.AlertDistance,
                    StartBearing = billboard.StartBearing,
                    EndBearing = billboard.EndBearing,
                    Latitude = (double)billboard.Location.Latitude,
                    Longitude = (double)billboard.Location.Longitude,
                    MerchantLogoContent = billboard.MerchantLogo.Content,
                    MerchantName = billboard.MerchantName,
                    Points = billboard.Points,
                    AdvertisementId = billboard.Advertisement.AdvertisementId
                });

            var results = resultBillboardsQuery.ToList()
                .Select(billboard => new BillboardModel
                {
                    BillboardId = billboard.BillboardId,
                    AlertDistance = billboard.AlertDistance,
                    StartBearing = billboard.StartBearing,
                    EndBearing = billboard.EndBearing,
                    Latitude = billboard.Latitude,
                    Longitude = billboard.Longitude,
                    MerchantLogo = Convert.ToBase64String(billboard.MerchantLogoContent),
                    MerchantName = billboard.MerchantName,
                    Points = billboard.Points,
                    AdvertismentId = billboard.AdvertisementId
                });

            return new GetBillboardsResponse
            {
                Billboards = results
            };
        }

        public GetAdvertisementsResponse GetAdvertisements(GetAdvertisementsRequest requets)
        {
            IQueryable<Advertisement> advertisementsSet = _db.Set<Advertisement>();
            if (requets != null)
            {
                advertisementsSet = advertisementsSet.Where(advertisement => requets.BillboardIds.Contains(advertisement.Billboard.BillboardId));
            }
            var advertisementsQuery = advertisementsSet
            .Select(advertisement => new
            {
                AdvertismentId = advertisement.AdvertisementId,
                ExternalLink = advertisement.ExternalUrl,
                Image = advertisement.Image.Content,
                Points = advertisement.Points,
                Timeout = advertisement.Timeout
            });

            var advertisements = advertisementsQuery.ToList()
                .Select(advertisement => new AdvertisementModel
                {
                    AdvertismentId = advertisement.AdvertismentId,
                    ExternalLink = advertisement.ExternalLink,
                    Image = Convert.ToBase64String(advertisement.Image),
                    Points = advertisement.Points,
                    TimeOut = advertisement.Timeout
                });

            return new GetAdvertisementsResponse
            {
                Advertisements = advertisements
            };
        }

        public SaveAdvertisementPointsResponse SavePoints(SaveAdvertisementPointsRequest request)
        {
            var advertisement = _db.Set<Advertisement>().Find(request.AdvertisementId);
            if (advertisement == null)
            {
                return new SaveAdvertisementPointsResponse
                {
                    Succeeded = false,
                    ErrorMessage = "not found"
                };
            }

            using (var pointsService = new UnicardApi.Service1Client())
            {
                var model = new UnicardApi.BonusAccumulationByUserModel();
                model.lang = "ka";
                model.app_source = "MOBAPP";
                model.user_id = request.UserId;
                model.amount = advertisement.Points;
                model.device_id = advertisement.Billboard.Code;
                model.tran_date = DateTime.Now.ToString("dd/MM/yyyy");
                model.respcode = "000";
                model.stan = Guid.NewGuid().ToString();
                using (HttpClient client = new HttpClient())
                {
                    var body = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });

                    HttpContent content = null;
                    content = new StringContent(body, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Clear();

                    var defaultHeaders = new Dictionary<string, string>();

                    var hash = CalculateHash(body);
                    defaultHeaders.Add("x-cmd5", hash);

                    var hmac = CalculateHMac(defaultHeaders.Values.ToList());
                    defaultHeaders.Add("x-authorization", string.Format("UNICARDAPI {0}:{1}", _publicKey, hmac));

                    defaultHeaders.Add("x-source", "MOBAPP");
                    foreach (var header in defaultHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                    var result = client.PostAsJsonAsync("http://109.238.238.194/UnicardPublicAPI.Service1.svc/BonusAccumulationByUser", model).Result;
                   
                    result.EnsureSuccessStatusCode();
                    var res = result.Content.ReadAsAsync<UnicardApi.BonusAccumulationByUserResult>().Result;

                    advertisement.UserPoints.Add(new AdvertisementPoint()
                    {
                        ErrorMessage = res.ErrorMessage,
                        Success = res.ResultCode == "200",
                        Points = advertisement.Points,
                        UserId = request.UserId.ToString()
                    });

                    _db.SaveChanges();

                }
            }

            return new SaveAdvertisementPointsResponse
            {
                Succeeded = true
            };
        }

        private string CalculateHash(string content)
        {
            using (MD5 hasher = MD5.Create())
            {
                byte[] data = hasher.ComputeHash(Encoding.Default.GetBytes(content));
                return Convert.ToBase64String(data);
            }
        }


        private string CalculateHMac(System.Collections.Generic.List<string> values)
        {
            string headers = values.Aggregate((a, b) => a + b) + _publicKey;

            using (var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey)))
            {
                byte[] data = hasher.ComputeHash(Encoding.UTF8.GetBytes(headers));
                return Convert.ToBase64String(data);
            }
        }

    }
}
