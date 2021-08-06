using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunicardus.Billboards.Core.Services.Abstract;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core.Helpers;
using Newtonsoft.Json;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Core.Models.DTOs.Response;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using System.IO;

namespace Kunicardus.Billboards.Core.Services
{
	public class BillboardsService : IBillboardsService
	{
		BillboardsDb _db;
		IUnicardApiProvider _apiProvider;

		public BillboardsService (IUnicardApiProvider apiProvider)
		{
			_db = new BillboardsDb (BillboardsDb.path);
			_apiProvider = apiProvider;
		}

		public void InsertDummyDataForIOS ()
		{
			#if __IOS__ && DEBUG
			for (int k = 0; k < 2; k++) {
				_db.Insert (
					new BillboardHistory () {
						AdvertismentId = 4, 
						BillboardId = 4,
						PassDate = DateTime.Now,
						Status = Enums.AdvertismentStatus.NotLoaded
					});
				_db.Insert (
					new BillboardHistory () {
						AdvertismentId = 3, 
						BillboardId = 3,
						PassDate = DateTime.Now,
						Status = Enums.AdvertismentStatus.NotLoaded
					});
				_db.Insert (
					new BillboardHistory () {
						AdvertismentId = 5, 
						BillboardId = 5,
						PassDate = DateTime.Now,
						Status = Enums.AdvertismentStatus.NotLoaded
					});
				_db.Insert (
					new BillboardHistory () {
						AdvertismentId = 2, 
						BillboardId = 2,
						PassDate = DateTime.Now,
						Status = Enums.AdvertismentStatus.NotLoaded
					});
			}
			_db.Commit ();
			#endif
		}

		public List<Billboard> GetBillboards ()
		{  
			var billboards = new List<Billboard> ();
			var url = Path.Combine (Constants.NewUnicardServiceUrl, "GetBillboards");
			UnicardApiBaseRequestForMethods req = new UnicardApiBaseRequestForMethods ();
			var json = JsonConvert.SerializeObject (req, 
				           Formatting.None,
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });

			var response = _apiProvider.UnsecuredPost<GetBillboardsResponse> (url, null, json);

			if (response.Billboards != null) {
				var list = response.Billboards;
				string errorText = "";
				try {
					_db.BeginTransaction ();
					int i = 0;
					foreach (var item in list) {
						var billboard = new Billboard {
							AdvertismentId = item.AdvertismentId,
							AlertDistance = item.AlertDistance,
							BillboardId = item.BillboardId,
							Latitude = item.Latitude,
							Longitude = item.Longitude,
							StartBearing = item.StartBearing,
							EndBearing = item.EndBearing,
							MerchantLogo = item.MerchantLogo.ToBase64String (),
							MerchantName = item.MerchantName,
							Points = item.Points
						};
						#if __IOS__ && DEBUG
						billboard.StartBearing = -200;
						billboard.EndBearing = 400;
						billboard.AlertDistance = 50;
						if (i == 0) {
							billboard.Latitude = (decimal)37.33398819;
							billboard.Longitude = (decimal)-122.04886627;
						}
						if (i == 1) {
							billboard.Latitude = (decimal)37.33444977;
							billboard.Longitude = (decimal)-122.04149628;
						}
						if (i == 2) {
							billboard.Latitude = (decimal)37.33451462;
							billboard.Longitude = (decimal)-122.03845215;
						}
						if (i == 3) {
							billboard.Latitude = (decimal)37.3351212;
							billboard.Longitude = (decimal)-122.03256229;
						}
						if (i == 4) {
							billboard.Latitude = (decimal)37.33460316;
							billboard.Longitude = (decimal)-122.0350347;
						}
						if (i == 5) {
							billboard.Latitude = (decimal)37.33375445;
							billboard.Longitude = (decimal)-122.04992666;
						}
						#endif
						_db.InsertOrReplace (billboard);
						i++;
					}
					_db.Commit ();
				} catch (Exception ex) {
					_db.Rollback ();
					errorText = ex.Message;
				}
			} 
			var query = "select * from Billboard";
			billboards = _db.Query<Billboard> (query);
			return billboards;
		}

		public int GetLastPassedBillboardId ()
		{
			string query = @"select 
                                    b.BillboardId, 
                                    b.PassDate 
                             from BillboardHistory as b 
                             order by b.PassDate Desc
                             limit 1";
			try {
				var bhs = _db.Query<BillboardHistory> (query).FirstOrDefault ();
				if (bhs != null) {
					return bhs.BillboardId;
				}
				return -1;
			} catch (Exception ex) {
				return -1;
			}
		}

		public bool MarkBillboardAsSeen (int billboardId, int advertisementId)
		{
			BillboardHistory history = new BillboardHistory {
				AdvertismentId = advertisementId,
				BillboardId = billboardId,
				PassDate = DateTime.Now,
				Status = Enums.AdvertismentStatus.NotLoaded
			};
	
			string errorText = "";
			try {
				_db.Insert (history);
				return true;
			} catch (Exception ex) {
				errorText = ex.Message;
				return false;
			}
		}
	}
}