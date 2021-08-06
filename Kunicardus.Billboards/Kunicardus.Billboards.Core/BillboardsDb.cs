using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using System.IO;
using Kunicardus.Billboards.Core.DbModels;

#if __IOS__
using Foundation;
using UIKit;
#endif

namespace Kunicardus.Billboards.Core
{
	public class BillboardsDb : SQLiteConnection
	{

		#if __IOS__
		private static string _documentsPath;

		public static string DocumentsPath {
			get {
				return _documentsPath ?? (_documentsPath = SystemDocumentsPath ());
			}
		}

		private static string SystemDocumentsPath ()
		{
			int SystemVersion = Convert.ToInt16 (UIDevice.CurrentDevice.SystemVersion.Split ('.') [0]);
			if (SystemVersion >= 8) {
				return NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0].Path;
			}
			return Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
		}

		public static string DataPath {
			get {
				return Path.Combine (DocumentsPath, "uniboardapp.db");
			}
		}

		public static string path {
			get {
		
				return DataPath;
			}
		}
		#else
				public static string path {
				get {
		
				return Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "unicard.db");
				}
				}
		#endif




		public BillboardsDb (string path) : base (path)
		{			
			CreateTable<Billboard> ();
			CreateTable<BillboardHistory> ();
			CreateTable<UserInfo> ();

			var dataExists = Table<Billboard> ().Any ();
			if (!dataExists) {
				// InitDummyData();
			}
		}

		private void InitDummyData ()
		{
			//#region Billboards
			//List<Billboard> billboards = new List<Billboard>{
			//    new Billboard{
			//        AdvertismentId = 1,
			//        BillboardId = 1,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.68582898,
			//        Longitude = 44.86116886,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://api.unicard.ge/upload/e773957322416647cd4e5989b73d41b2.jpg\108x65.jpg",
			//        MerchantName = "სოკარი",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 2,
			//        BillboardId = 2,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.68459908,
			//        Longitude = 44.86769199,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://api.unicard.ge/upload/b317918db3a04c5696102baaef2a03a7.jpg\108x65.jpg",
			//        MerchantName = "GPC",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 3,
			//        BillboardId = 3,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.68397811,
			//        Longitude = 44.87672299,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://api.unicard.ge/upload/e773957322416647cd4e5989b73d41b2.jpg\108x65.jpg",
			//        MerchantName = "სოკარი",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 4,
			//        BillboardId = 4,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.68407626,
			//        Longitude = 44.88282233,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://www.socar.ge/img/logo.gif",
			//        MerchantName = "ბიბლუსი",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 5,
			//        BillboardId = 5,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.6844829,
			//        Longitude = 44.88765299,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://www.socar.ge/img/logo.gif",
			//        MerchantName = "სოკარი",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 6,
			//        BillboardId = 6,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.68528614,
			//        Longitude = 44.89025474,
			//        AlertDistance = 200,
			//        MerchantLogo = "http://www.socar.ge/img/logo.gif",
			//        MerchantName = "ბიბლუსი",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 7,
			//        BillboardId = 7,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.68773589,
			//        Longitude = 44.89936888,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://www.socar.ge/img/logo.gif",
			//        MerchantName = "GPC",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 8,
			//        BillboardId = 8,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.68836684,
			//        Longitude = 44.91835624,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://www.socar.ge/img/logo.gif",
			//        MerchantName = "სოკარი",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 9,
			//        BillboardId = 9,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.68842492,
			//        Longitude = 44.92688835,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://www.socar.ge/img/logo.gif",
			//        MerchantName = "სოკარი",
			//        IsAdSeen = false,
			//        Points = 10
			//    },
			//    new Billboard{
			//        AdvertismentId = 10,
			//        BillboardId = 10,
			//        Direction = Enums.Direction.East,
			//        IsLoaded = false,
			//        Latitude = 41.6890819,
			//        Longitude = 44.93070781,
			//        AlertDistance = 200,
			//        MerchantLogo = @"http://www.socar.ge/img/logo.gif",
			//        MerchantName = "სოკარი",
			//        IsAdSeen = false,
			//        Points = 10
			//    }
			//};
			//foreach (var item in billboards)
			//{
			//    this.InsertOrReplace(item);
			//}
			//#endregion
		}
	}
}